using HarmonyLib;
using UnityEngine;

namespace Dreamstalker.Patches;

[HarmonyPatch]
internal class FlashlightPatches
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(Flashlight), nameof(Flashlight.Start))]
	public static void Flashlight_Start(Flashlight __instance)
	{
		if (Main.DebugMode) return;

		foreach (var light in __instance._lights)
		{
			light.GetLight().color = new Color(0.1f, 0.2f, 0.1f, 1f);
		}
	}
}
