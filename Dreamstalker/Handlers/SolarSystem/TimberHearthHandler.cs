using Dreamstalker.Components;
using Dreamstalker.Components.Volumes;
using Dreamstalker.Utility;
using HarmonyLib;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

[HarmonyPatch]
internal class TimberHearthHandler : SolarSystemHandler
{
    protected override void BeforePlanetCreation() { }


	protected override void OnSolarSystemAwake()
    {
        // Before NH can add the audio volume
        var th = GameObject.Find("TimberHearth_Body");
        foreach (var audio in th.GetComponentsInChildren<AudioVolume>()) audio.enabled = false;
    }

    protected override void OnSolarSystemStart()
    {
        Main.Log("Timber Hearth handler invoked.");

        var th = Locator.GetAstroObject(AstroObject.Name.TimberHearth);

		// Dont want lights disabling when it becomes "day"
        // Has to go before deleting lights bc lights depend on night lights idk
		foreach (var nightLight in th.GetComponentsInChildren<NightLight>())
		{
			nightLight.OnSunrise();
			DestroyImmediate(nightLight);
		}

		// Weaken or turn out all lights
		foreach (var light in th.GetComponentsInChildren<Light>())
        {
            var parent = light.transform.parent;
            if (parent != null)
            {
                if (parent.name.Contains("Props_HEA_BlueLantern"))
                {
                    parent.gameObject.SetActive(false);
                }
                else if (parent.name.Contains("WindowPivot_Cabin"))
                {
                    // Turn off their window lights
                    var owLight = light.gameObject.GetComponent<OWLight>();
                    if (owLight != null) DestroyImmediate(owLight);

                    var owLight2 = light.gameObject.GetComponent<OWLight2>();
					if (owLight2 != null) DestroyImmediate(owLight2);

					DestroyImmediate(light);
                }
            }
            light.color = new Color(0.4f, 1f, 1f);
        }

        // Get rid of the ship
        Locator.GetShipBody().gameObject.SetActive(false);

        // Lower ambient light
        th.transform.Find("AmbientLight_TH").GetComponent<Light>().intensity = 0.6f;

        // Disable all elevators
        foreach (var elevator in th.GetComponentsInChildren<Elevator>())
        {
            elevator._interactVolume.SetInteractionEnabled(false);
        }

		// Spawn stuff
		var thCampfire = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Interactables_Village/LaunchTower/Effects_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();

		// Add statue eye controller
		var statue = th.GetComponentInChildren<MemoryUplinkTrigger>().gameObject.AddComponent<StatueEyeController>();
        statue.SetSector(th.GetRootSector());
        statue.campfire = thCampfire;

        var thCompletionVolume = CompletionVolume.MakeCompletionVolume(th, thCampfire, AstroObject.Name.BrittleHollow, Vector3.zero, 8f);
        thCompletionVolume.transform.parent = statue.transform;
        thCompletionVolume.transform.localPosition = Vector3.zero;

		SpawnWrapper.SpawnDreamstalker(th, thCampfire, thCompletionVolume, Vector3.zero);
	}

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MemoryUplinkTrigger), nameof(MemoryUplinkTrigger.OnStartOfTimeLoop))]
    private static bool MemoryUplinkTrigger_OnStartOfTimeLoop() => false; // Stops it from destroying the component
}
