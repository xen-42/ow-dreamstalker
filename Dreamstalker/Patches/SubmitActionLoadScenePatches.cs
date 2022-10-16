using HarmonyLib;
using NewHorizons.Patches;

namespace Dreamstalker.Patches;

[HarmonyPatch]
internal static class SubmitActionLoadScenePatches
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(SubmitActionLoadScene), nameof(SubmitActionLoadScene.Update))]
	[HarmonyPatch(typeof(SubmitActionLoadScene), nameof(SubmitActionLoadScene.ConfirmSubmit))]
	private static void SubmitActionLoadScene_Update(SubmitActionLoadScene __instance)
	{
		// Always redirect to the eye
		if (__instance._sceneToLoad == SubmitActionLoadScene.LoadableScenes.GAME)
			__instance._sceneToLoad = SubmitActionLoadScene.LoadableScenes.EYE;
	}

	// Stop NH from warping back to the eye
	[HarmonyPrefix]
	[HarmonyPatch(typeof(EyeOfTheUniversePatches), nameof(EyeOfTheUniversePatches.SubmitActionLoadScene_ConfirmSubmit))]
	private static bool EyeOfTheUniversePatches_SubmitActionLoadScene_ConfirmSubmit() => false;
}
