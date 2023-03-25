using Dreamstalker.Components.Streaming;
using Dreamstalker.Components.Volumes;
using Dreamstalker.Utility;
using HarmonyLib;
using NewHorizons.Utility;
using NewHorizons.Utility.Files;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dreamstalker.Patches;

[HarmonyPatch]
internal static class FirePatches
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(Campfire), nameof(Campfire.Start))]
	private static void Campfire_Start(Campfire __instance)
	{
        try
        {
            if (__instance is DreamCampfire || __instance.transform.parent.name.Contains("IP_DreamArrivalPoint"))
            {
                return;
            }

            foreach (var light in __instance._lightController._lights)
            {
                light.gameObject.GetComponent<Light>().color = new Color(0.1f, 0.4f, 0.1f);
            }

            var emberMaterial = __instance.transform.parent.Find("Props_HEA_Campfire/Campfire_Embers").GetComponent<MeshRenderer>().material;
            emberMaterial.SetTexture("_MainTex", ImageUtilities.GetTexture(Main.Instance, "assets/Props_HEA_CampfireEmbers_d.png"));
            emberMaterial.SetTexture("_EmissionMap", ImageUtilities.GetTexture(Main.Instance, "assets/Props_HEA_CampfireEmbers_e.png"));

            var ashMaterial = __instance.transform.parent.Find("Props_HEA_Campfire/Campfire_Ash").GetComponent<MeshRenderer>().material;
            ashMaterial.SetTexture("_EmissionMap", ImageUtilities.GetTexture(Main.Instance, "assets/Props_HEA_CampfireAsh_e.png"));

			__instance._flames.material.color = new Color(0f, 1f, 0f);

            if (SceneManager.GetActiveScene().name == "SolarSystem")
            {
				var streamingVolume = new GameObject("CampfireStreamingVolume");
				streamingVolume.transform.parent = __instance.transform;
				streamingVolume.transform.localPosition = Vector3.zero;
				streamingVolume.layer = LayerMask.NameToLayer("BasicEffectVolume");

				streamingVolume.AddComponent<SphereShape>().radius = 5f;
				streamingVolume.AddComponent<OWTriggerVolume>();
				var streamingController = streamingVolume.AddComponent<CampfireStreamingLoadVolume>();
				streamingController.SetSector(__instance.GetSector());
				streamingController.SetStreaming(StreamingGroups.Get(AstroObject.Name.DarkBramble));
				streamingController.campfire = __instance;
			}
		}
        catch (Exception e)
        {
            Main.LogError($"Could not tint campfire {__instance.transform.GetPath()} : {e}");
        }
    }

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Campfire), nameof(Campfire.CanSleepHereNow))]
	private static bool Campfire_CanSleepHereNow(Campfire __instance, ref bool __result)
	{
		__result = __instance.GetComponent<CompletionCampfire>() != null && __instance._state == Campfire.State.LIT && OWInput.IsInputMode(InputMode.Character);
		return false;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Campfire), nameof(Campfire.Update))]
	private static void Campfire_Update(Campfire __instance)
	{
		// Only show sleep prompt when you actually can
		if (__instance._sleepPrompt != null)
		{
			__instance._sleepPrompt.SetVisibility(__instance._sleepPrompt.IsDisplayState(ScreenPrompt.DisplayState.Normal));
		}
	}

	[HarmonyPostfix]
    [HarmonyPatch(typeof(Marshmallow), nameof(Marshmallow.Start))]
    private static void Marshmallow_Start(Marshmallow __instance)
    {
        __instance._fireRenderer.material.color = Color.green;
    }
}
