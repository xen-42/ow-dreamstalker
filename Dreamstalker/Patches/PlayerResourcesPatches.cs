using HarmonyLib;

namespace Dreamstalker.Patches;

[HarmonyPatch]
internal class PlayerResourcesPatches
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(PlayerResources), nameof(PlayerResources.IsOxygenPresent))]
	private static bool PlayerResources_IsOxygenPresent(PlayerResources __instance, ref bool __result)
	{
		// Prevent random asphyxiation
		__result = !__instance.IsUnderwater();
		return false;
	}
}
