using Dreamstalker.Components.Volumes;
using Dreamstalker.Utility;
using NewHorizons.Utility;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class DreamworldHandler : SolarSystemHandler
{
    protected override void OnSolarSystemAwake()
    {

    }

    protected override void OnSolarSystemStart()
    {
        Main.Log("Dream World handler invoked.");

        var dw = AstroObjectLocator.GetAstroObject("Dreaming");

        // Spawn point
        var spawnGO = new GameObject("Spawn");
        spawnGO.transform.parent = dw.transform;
        spawnGO.transform.localPosition = new Vector3(0, 200, 0);
        spawnGO.layer = 8;
        var spawn = spawnGO.AddComponent<SpawnPoint>();
        spawn._isShipSpawn = false;
        spawn._triggerVolumes = new OWTriggerVolume[] { dw.GetComponentInChildren<Sector>()._owTriggerVolume };

        // Spawn stuff
        /*
        var thCampfire = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Interactables_Village/LaunchTower/Effects_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();

		var thCompletionVolume = GameObject.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_Observatory/Volumes_Observatory/ObservatoryInteriorVolume/MuseumEntryway").AddComponent<CompletionVolume>();
		thCompletionVolume.enabled = false;
		thCompletionVolume.SetCampfire(thCampfire);
        thCompletionVolume.NextPlanet = AstroObject.Name.BrittleHollow;

		SpawnWrapper.SpawnDreamstalker(th, thCampfire, thCompletionVolume, Vector3.zero);
        */
	}
}
