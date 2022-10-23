using HarmonyLib;

namespace Dreamstalker.Patches;

[HarmonyPatch(typeof(KeyInfoPromptController))]
internal class KeyInfoPromptControllerPatches
{
	[HarmonyPostfix]
	[HarmonyPatch(nameof(KeyInfoPromptController.Start))]
	public static void KeyInfoPromptController_Start(KeyInfoPromptController __instance)
	{
		// Don't show launch codes ever
		__instance._displayCodePrompt = false;
	}
}
