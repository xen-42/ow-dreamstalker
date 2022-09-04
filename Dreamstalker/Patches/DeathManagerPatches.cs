using Dreamstalker.Utility;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Patches;

[HarmonyPatch]
internal static class DeathManagerPatches
{
	private static float _resurrectTime;
	private static bool _resurrectAfterDelay;
	private static bool _fakeDeath;

	private static void Reset()
	{
		_resurrectTime = 0f;
		_resurrectAfterDelay = false;
		_fakeDeath = false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(DeathManager), nameof(DeathManager.Start))]
	private static void DeathManager_Awake()
	{
		Reset();
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(DeathManager), nameof(DeathManager.KillPlayer))]
	private static void DeathManager_KillPlayer(DeathManager __instance, DeathType deathType)
	{
		// The time loop should be disabled but just in case
		if (deathType != DeathType.TimeLoop)
		{
			_fakeDeath = true;
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(DeathManager), nameof(DeathManager.FinishDeathSequence))]
	private static bool DeathManager_FinishDeathSequence(DeathManager __instance)
	{
		if (_fakeDeath)
		{
			_resurrectAfterDelay = true;
			_resurrectTime = Time.time + 2f;
			__instance.enabled = true;
			_fakeDeath = false;
			return false;
		}
		else
		{
			Reset();
			return true;
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(DeathManager), nameof(DeathManager.Update))]
	private static void DeathManager_Update(DeathManager __instance)
	{
		if (_resurrectAfterDelay && _resurrectTime < Time.time)
		{
			Main.Log($"Respawning player after death");
			_resurrectAfterDelay = false;

			__instance.enabled = false;

			__instance._isDying = false;
			__instance._isDead = false;
			__instance._fakeMeditationDeath = false;

			PlayerSpawnUtil.SpawnAt(AstroObject.Name.TimberHearth);

			GlobalMessenger.FireEvent("PlayerResurrection");
			Locator.GetPauseCommandListener().RemovePauseCommandLock();

			Reset();
		}
	}
}
