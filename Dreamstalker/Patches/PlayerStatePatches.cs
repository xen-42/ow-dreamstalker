using HarmonyLib;

namespace Dreamstalker.Patches;

[HarmonyPatch(typeof(PlayerState))]
internal class PlayerStatePatches
{
	[HarmonyPostfix]
	[HarmonyPatch(nameof(PlayerState.Reset))]
	private static void PlayerState_Reset()
	{
		PlayerState._inDarkZone = true;
	}

	// Always consider player to be in dark zone to keep flashlight prompt on
	[HarmonyPostfix]
	[HarmonyPatch(nameof(PlayerState.OnExitDarkZone))]
	private static void PlayerState_OnExitDarkZone() => PlayerState._inDarkZone = true;
}
