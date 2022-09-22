using Dreamstalker.Components.Volumes;
using Dreamstalker.Utility;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

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
                    DestroyImmediate(light);
                }
            }
            light.color = new Color(0.4f, 1f, 1f);
        }
        // Dont want lights disabling when it becomes "day"
        foreach (var nightLight in th.GetComponentsInChildren<NightLight>())
        {
            nightLight.OnSunrise();
            DestroyImmediate(nightLight);
        }

        // Get rid of the ship
        Locator.GetShipBody().gameObject.SetActive(false);

        // Lower ambient light
        th.transform.Find("AmbientLight_TH").GetComponent<Light>().intensity = 0.6f;

        // Remove music
        th.GetComponentInChildren<VillageMusicVolume>().gameObject.SetActive(false);

        // Disable all elevators
        foreach (var elevator in th.GetComponentsInChildren<Elevator>())
        {
            elevator._interactVolume.SetInteractionEnabled(false);
        }

        // Spawn stuff
        var thCampfire = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Interactables_Village/LaunchTower/Effects_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();

		var thCompletionVolume = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_Observatory/Volumes_Observatory/ObservatoryInteriorVolume/MuseumEntryway").AddComponent<CompletionVolume>();
		thCompletionVolume.enabled = false;
		thCompletionVolume.SetCampfire(thCampfire);
        thCompletionVolume.NextPlanet = AstroObject.Name.BrittleHollow;

		SpawnWrapper.SpawnDreamstalker(th, thCampfire, thCompletionVolume, Vector3.zero);
	}
}
