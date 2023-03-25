using Dreamstalker.Components;
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
		// The time loop should be ignored
		if (deathType != DeathType.TimeLoop && deathType != DeathType.Supernova)
		{
			Main.Log($"Player died to {deathType}");

			PlayerEffectController.Instance.Blink(1f);
			PlayerSpawnUtil.Respawn();
			GeneralHandler.TurnOffCampFires();
		}

		return false;
	}
}
