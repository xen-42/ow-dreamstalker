using HarmonyLib;

namespace Dreamstalker.Patches;

[HarmonyPatch]
internal static class SubmitActionLoadScenePatches
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(SubmitActionLoadScene), nameof(SubmitActionLoadScene.ConfirmSubmit))]
	private static void SubmitActionLoadScene_Update(SubmitActionLoadScene __instance)
	{
		// Always redirect to the eye
		if (__instance._sceneToLoad == SubmitActionLoadScene.LoadableScenes.GAME)
			__instance._sceneToLoad = SubmitActionLoadScene.LoadableScenes.EYE;
	}

	// Stop NH from warping back to the eye
	[HarmonyPrefix]
	[HarmonyPatch(typeof(NewHorizons.Patches.EyeScenePatches.SubmitActionLoadScenePatches), nameof(NewHorizons.Patches.EyeScenePatches.SubmitActionLoadScenePatches.SubmitActionLoadScene_ConfirmSubmit))]
	private static bool EyeOfTheUniversePatches_SubmitActionLoadScene_ConfirmSubmit(out bool __result)
	{
		// replace patch with "return true" so it just runs the original method
		__result = true;
		return false;
	}
}
