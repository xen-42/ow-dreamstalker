using HarmonyLib;

namespace Dreamstalker.Patches;

[HarmonyPatch]
internal static class SubmitActionLoadScenePatches
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(SubmitActionLoadScene), nameof(SubmitActionLoadScene.Update))]
	private static void SubmitActionLoadScene_Update(SubmitActionLoadScene __instance)
	{
		// always redirect to eye lol
		if (__instance._sceneToLoad == SubmitActionLoadScene.LoadableScenes.GAME)
			__instance._sceneToLoad = SubmitActionLoadScene.LoadableScenes.EYE;
	}
}
