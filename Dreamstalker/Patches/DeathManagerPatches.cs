﻿using Dreamstalker.Components;
using Dreamstalker.Handlers.SolarSystem;
using Dreamstalker.Utility;
using HarmonyLib;

namespace Dreamstalker.Patches;

[HarmonyPatch]
internal static class DeathManagerPatches
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(DeathManager), nameof(DeathManager.KillPlayer))]
	private static bool DeathManager_KillPlayer(DeathManager __instance, DeathType deathType)
	{
		// The time loop should be disabled but just in case
		if (deathType != DeathType.TimeLoop)
		{
			PlayerEffectController.Instance.Blink(1f);
			PlayerSpawnUtil.SpawnAt(AstroObject.Name.TimberHearth);
			PropHandler.TurnOffCampFires();
			return false;
		}

		return true;
	}
}