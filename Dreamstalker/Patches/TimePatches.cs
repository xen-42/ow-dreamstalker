using HarmonyLib;

namespace Dreamstalker.Patches;

internal class TimePatches
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(TempCometCollisionFix), nameof(TempCometCollisionFix.Update))]
	[HarmonyPatch(typeof(SunController), nameof(SunController.Update))]
	[HarmonyPatch(typeof(SunController), nameof(SunController.OnTriggerSupernova))]
	[HarmonyPatch(typeof(GlobalMusicController), nameof(GlobalMusicController.UpdateEndTimesMusic))]
	[HarmonyPatch(typeof(TimeLoop), nameof(TimeLoop.Update))]
	private static bool SkipMethod() => false;

	[HarmonyPrefix]
	[HarmonyPatch(typeof(TimeLoop), nameof(TimeLoop.GetSecondsElapsed))]
	[HarmonyPatch(typeof(TimeLoop), nameof(TimeLoop.GetMinutesElapsed))]
	[HarmonyPatch(typeof(TimeLoop), nameof(TimeLoop.GetFractionElapsed))]
	private static bool TimeLoop_GetTimeElapsed(ref float __result)
	{
		__result = 0f;
		return false;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(TimeLoop), nameof(TimeLoop.GetSecondsRemaining))]
	private static bool TimeLoop_GetSecondsRemaining(ref float __result)
	{
		__result = TimeLoop._loopDuration;
		return false;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(TimeLoop), nameof(TimeLoop.Start))]
	private static void TimeLoop_Start(TimeLoop __instance)
	{
		TimeLoop._isTimeFlowing = false;
		__instance.enabled = false;
	}
}
