using Dreamstalker.Components;
using Dreamstalker.Components.Volumes;
using Dreamstalker.Utility;
using NewHorizons.Builder.Props;
using NewHorizons.External.Modules;
using NewHorizons.Utility;
using System.IO;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class DreamworldHandler : SolarSystemHandler
{
    private AstroObject dreamworld;

    protected override void OnSolarSystemAwake()
    {

    }

    protected override void OnSolarSystemStart()
    {
        Main.Log("Dream World handler invoked.");

        dreamworld = AstroObjectLocator.GetAstroObject("Dreaming");

        var farClipController = dreamworld.gameObject.AddComponent<CameraLayerCullController>();
        farClipController.SetSector(dreamworld.GetRootSector());

		// Spawn point
		var spawnGO = new GameObject("Spawn");
        spawnGO.transform.parent = dreamworld.transform;
        spawnGO.transform.localPosition = new Vector3(0, 100, 0);
        spawnGO.layer = 8;
        var spawn = spawnGO.AddComponent<SpawnPoint>();
        spawn._isShipSpawn = false;
        spawn._triggerVolumes = new OWTriggerVolume[] { dreamworld.GetComponentInChildren<Sector>()._owTriggerVolume };

        var islandPrefab = DZ1_A_Island_C_Prefab();

		Random.InitState(420); //haha funny number

		for (int i = 0; i < 100; i++)
		{
			var dir = Random.onUnitSphere;
			var island = MakeIsland(islandPrefab, dir * 100, Quaternion.FromToRotation(Vector3.up, dir).eulerAngles);
			island.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
		}

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

    private GameObject MakeIsland(GameObject prefab, Vector3 pos, Vector3? rot = null)
    {
        var island = new PropModule.DetailInfo()
        {
            path = prefab.name,
            position = pos,
            rotation = rot ?? Vector3.zero
		};
		return DetailBuilder.Make(dreamworld.gameObject, dreamworld.GetRootSector(), island);
	}

    private GameObject DZ1_A_Island_C_Prefab()
    {
		var name = "DZ1_A_Island_C";
		var side = "A";
		var pivotPos = new Vector3(0f, -1.85f, 0f);
		var pivotRot = new Vector3(344.405f, 14.216f, 324.3437f);
		var offsetPos = new Vector3(104.8446f, -153.5188f, -76.2674f);
		var offsetRot = new Vector3(8.7642f, 359.2007f, 13.4333f);
		var pos = new Vector3(-44.5468f, 192.7909f, 25.4459f);
		var rot = new Vector3(355.5043f, 192.5498f, 345.7785f);

		var root = new GameObject($"Prefab_{name}");

		var pivot = new GameObject("Pivot");
		pivot.transform.parent = root.transform;
		pivot.transform.localPosition = pivotPos;
		pivot.transform.localRotation = Quaternion.Euler(pivotRot);

		var offset = new GameObject("Offset");
		offset.transform.parent = pivot.transform;
		offset.transform.localPosition = offsetPos;
		offset.transform.localRotation = Quaternion.Euler(offsetRot);

		var wall = new PropModule.DetailInfo()
		{
			path = $"DreamWorld_Body/Sector_DreamWorld/Sector_DreamZone_1/Geo_DreamZone_1/Terrain_DreamZone1/Side_{side}/Wall_{name}",
			position = pos,
			rotation = rot
		};
		DetailBuilder.Make(offset, null, wall);

		var floor = new PropModule.DetailInfo()
		{
			path = $"DreamWorld_Body/Sector_DreamWorld/Sector_DreamZone_1/Geo_DreamZone_1/Terrain_DreamZone1/Side_{side}/Floor_{name}",
			position = pos,
			rotation = rot
		};
		DetailBuilder.Make(offset, null, floor);

		root.SetActive(false);

		return root;
	}
}
