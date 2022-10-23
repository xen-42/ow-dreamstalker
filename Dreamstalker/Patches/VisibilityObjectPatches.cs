using HarmonyLib;

namespace Dreamstalker.Patches;

[HarmonyPatch(typeof(VisibilityObject))]
internal static class VisibilityObjectPatches
{
	[HarmonyPrefix]
	[HarmonyPatch(nameof(VisibilityObject.CheckEnabled))]
	private static void VisibilityObject_CheckEnabled(VisibilityObject __instance)
	{
		// Idk why this happens
		__instance._visibilityTrackers ??= new VisibilityTracker[0];
	}
}
