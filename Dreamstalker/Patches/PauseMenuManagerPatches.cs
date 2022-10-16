using HarmonyLib;

namespace Dreamstalker.Patches;

[HarmonyPatch(typeof(PauseMenuManager))]
internal class PauseMenuManagerPatches
{
	[HarmonyPostfix]
	[HarmonyPatch(nameof(PauseMenuManager.Start))]
	private static void PauseMenuManager_Start(PauseMenuManager __instance)
	{
		__instance._skipToNextLoopButton.SetActive(false);
		__instance._exitToMainMenuAction.gameObject.SetActive(false);
	}
}
