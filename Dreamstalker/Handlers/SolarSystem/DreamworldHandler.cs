using Dreamstalker.Components.Dreamworld;
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
        spawnGO.transform.localPosition = new Vector3(93.9882f, 11.37577f, -30.61145f);
        spawnGO.layer = 8;
        var spawn = spawnGO.AddComponent<SpawnPoint>();
        spawn._isShipSpawn = false;
        spawn._triggerVolumes = new OWTriggerVolume[] { dreamworld.GetComponentInChildren<Sector>()._owTriggerVolume };

		// Change floorbed material
		dreamworld.transform.Find("Sector/GroundSphere").GetComponent<MeshRenderer>().material =
			GameObject.Find("DreamWorld_Body/Sector_DreamWorld/Sector_DreamZone_1/Geo_DreamZone_1/Terrain_IP_Dreamworld_Floorbed/Terrain_Dreamworld_Floorbed_Z1")
			.GetComponent<MeshRenderer>()
			.material;

		// Fix gravity
		dreamworld.GetGravityVolume().SetPriority(2);

		// Change fireplace
		// TODO: make the light green too :)
		var fireRoot = dreamworld.transform.Find("Sector/Party_House/Interactibles_PartyHouse/Prefab_IP_LodgeFire/Structure_DW_LodgeFireplace/LodgeFireplace_Fire");
		fireRoot.transform.Find("LodgeFireplace_Ash");

		var emberMaterial = fireRoot.transform.Find("LodgeFireplace_Embers").GetComponent<MeshRenderer>().material;
		emberMaterial.SetTexture("_MainTex", ImageUtilities.GetTexture(Main.Instance, "assets/Props_HEA_CampfireEmbers_d.png"));
		emberMaterial.SetTexture("_EmissionMap", ImageUtilities.GetTexture(Main.Instance, "assets/Props_HEA_CampfireEmbers_e.png"));

		var ashMaterial = fireRoot.transform.Find("LodgeFireplace_Ash").GetComponent<MeshRenderer>().material;
		ashMaterial.SetTexture("_EmissionMap", ImageUtilities.GetTexture(Main.Instance, "assets/Props_HEA_CampfireAsh_e.png"));

		fireRoot.transform.Find("LodgeFireplace_Flames").GetComponent<MeshRenderer>().material.color = new Color(0f, 1f, 0f);

		// Doors
		foreach (var rotatingDoor in dreamworld.GetComponentsInChildren<RotatingDoor>())
		{
			// some of them are open so CLOSE THEM
			rotatingDoor.Close();
			
			var doorTrigger = new GameObject("DoorTrigger");
			doorTrigger.layer = LayerMask.NameToLayer("BasicEffectVolume");
			doorTrigger.transform.parent = rotatingDoor.transform;
			doorTrigger.transform.localPosition = Vector3.zero;

			var col = doorTrigger.AddComponent<SphereCollider>();
			col.isTrigger = true;
			col.radius = 10f;

			doorTrigger.AddComponent<OWTriggerVolume>();

			doorTrigger.AddComponent<AutoDoorTrigger>();
		}

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
			rotation = rot,
			keepLoaded = true
		};
		DetailBuilder.Make(offset, null, wall);

		var floor = new PropModule.DetailInfo()
		{
			path = $"DreamWorld_Body/Sector_DreamWorld/Sector_DreamZone_1/Geo_DreamZone_1/Terrain_DreamZone1/Side_{side}/Floor_{name}",
			position = pos,
			rotation = rot,
			keepLoaded = true
		};
		DetailBuilder.Make(offset, null, floor);

		root.SetActive(false);

		return root;
	}
}
