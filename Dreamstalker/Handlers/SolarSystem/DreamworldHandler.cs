using Dreamstalker.Components.Dreamworld;
using Dreamstalker.Components.Volumes;
using Dreamstalker.Utility;
using NewHorizons.Builder.Props;
using NewHorizons.External.Modules;
using NewHorizons.Utility;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class DreamworldHandler : SolarSystemHandler
{
    private AstroObject _dreamworld;
	private GameObject _sectorRoot;

	protected override void BeforePlanetCreation() { }

	protected override void OnSolarSystemAwake() { }

    protected override void OnSolarSystemStart()
    {
        Main.Log("Dream World handler invoked.");

        _dreamworld = AstroObjectLocator.GetAstroObject("Dreaming");

        var farClipController = _dreamworld.gameObject.AddComponent<CameraLayerCullController>();
        farClipController.SetSector(_dreamworld.GetRootSector());

		// Spawn point
		var spawnGO = new GameObject("Spawn");
        spawnGO.transform.parent = _dreamworld.transform;
        spawnGO.transform.localPosition = new Vector3(93.9882f, 11.37577f, -30.61145f);
		spawnGO.transform.localRotation = Quaternion.FromToRotation(Vector3.up, spawnGO.transform.localPosition.normalized);
		spawnGO.layer = 8;
        var spawn = spawnGO.AddComponent<SpawnPoint>();
        spawn._isShipSpawn = false;
        spawn._triggerVolumes = new OWTriggerVolume[] { _dreamworld.GetComponentInChildren<Sector>()._owTriggerVolume };

		// Change floorbed material
		_dreamworld.transform.Find("Sector/GroundSphere").GetComponent<MeshRenderer>().material =
			GameObject.Find("DreamWorld_Body/Sector_DreamWorld/Sector_DreamZone_1/Geo_DreamZone_1/Terrain_IP_Dreamworld_Floorbed/Terrain_Dreamworld_Floorbed_Z1")
			.GetComponent<MeshRenderer>()
			.material;

		// Change fireplace
		var fireRoot = _dreamworld.transform.Find("Sector/Party_House/Interactibles_PartyHouse/Prefab_IP_LodgeFire/Structure_DW_LodgeFireplace/LodgeFireplace_Fire");

		foreach (var light in fireRoot.parent.parent/*lol*/.GetComponentsInChildren<Light>())
		{
			light.color = new Color(0.1f, 0.4f, 0.1f);
		}

		var emberMaterial = fireRoot.Find("LodgeFireplace_Embers").GetComponent<MeshRenderer>().material;
		emberMaterial.SetTexture("_MainTex", ImageUtilities.GetTexture(Main.Instance, "assets/Props_HEA_CampfireEmbers_d.png"));
		emberMaterial.SetTexture("_EmissionMap", ImageUtilities.GetTexture(Main.Instance, "assets/Props_HEA_CampfireEmbers_e.png"));

		var ashMaterial = fireRoot.Find("LodgeFireplace_Ash").GetComponent<MeshRenderer>().material;
		ashMaterial.SetTexture("_EmissionMap", ImageUtilities.GetTexture(Main.Instance, "assets/Props_HEA_CampfireAsh_e.png"));

		fireRoot.Find("LodgeFireplace_Flames").GetComponent<MeshRenderer>().material.color = new Color(0f, 1f, 0f);

		// Doors
		foreach (var rotatingDoor in _dreamworld.GetComponentsInChildren<RotatingDoor>())
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
		
		// make light sensors (on elevator) work
		foreach (var lightSensor in _dreamworld.GetComponentsInChildren<SingleLightSensor>())
		{
			lightSensor._sector.OnSectorOccupantsUpdated -= lightSensor.OnSectorOccupantsUpdated;
			lightSensor._sector = _dreamworld.GetRootSector();
			lightSensor._sector.OnSectorOccupantsUpdated += lightSensor.OnSectorOccupantsUpdated;
		}
		// set elevator destination
		var cageElevator = _dreamworld.GetComponentInChildren<CageElevator>();
		var destination = new GameObject(nameof(ElevatorDestination));
		destination.transform.parent = _dreamworld.transform;
		destination.transform.position = cageElevator.elevatorBody.GetPosition() - cageElevator.elevatorBody.transform.up * 20;
		destination.transform.rotation = cageElevator.elevatorBody.GetRotation();
		cageElevator._destinations[0] = destination.AddComponent<ElevatorDestination>();

        var islandPrefab = DZ1_A_Island_C_Prefab();

		Random.InitState(420); //haha funny number

		for (int i = 0; i < 100; i++)
		{
			var dir = Random.onUnitSphere;
			var island = MakeIsland(islandPrefab, dir * 100, Quaternion.FromToRotation(Vector3.up, dir).eulerAngles);
			island.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
		}

		// Spawn stuff
        var dwCampfire = _dreamworld.GetComponentInChildren<Campfire>();

		dwCampfire.SetState(Campfire.State.UNLIT, true);

		var dwVolume = new GameObject("CompletionVolume");
		dwVolume.transform.parent = _dreamworld.GetRootSector().transform;
		dwVolume.transform.localPosition = new Vector3(-5.937442f, 20.00692f, 98.94357f);
		dwVolume.layer = LayerMask.NameToLayer("BasicEffectVolume");

		var sphere = dwVolume.AddComponent<SphereCollider>();
		sphere.isTrigger = true;
		sphere.radius = 1f;

		var dwCompletionVolume = dwVolume.AddComponent<CompletionVolume>();
		dwCompletionVolume.enabled = false;
		dwCompletionVolume.SetCampfire(dwCampfire);
		dwCompletionVolume.NextPlanet = AstroObject.Name.QuantumMoon;

		var dreamstalker = SpawnWrapper.SpawnDreamstalker(_dreamworld, dwCampfire, dwCompletionVolume, Vector3.zero);
		
		// make elevator work only when it should
		cageElevator._ghostInterface.OnDownSelected -= cageElevator.GoDown;
		cageElevator._ghostInterface.OnDownSelected += () =>
		{
			// dont go if campfire aint lit
			if (dwCampfire.GetState() != Campfire.State.LIT) return;
			Main.Log("elevator activate! KILL DREAM MAN");
			dreamstalker.DespawnImmediate();
			cageElevator.GoDown();
		};

		_sectorRoot = _dreamworld.GetRootSector().gameObject;
		_sectorRoot.SetActive(false);
		PlayerSpawnUtil.OnSpawn.AddListener(OnSpawn);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PlayerSpawnUtil.OnSpawn.RemoveListener(OnSpawn);
	}

	private void OnSpawn(AstroObject.Name planet)
	{
		if (planet == AstroObject.Name.DreamWorld)
		{
			_sectorRoot.SetActive(true);
		}
		else
		{
			_sectorRoot.SetActive(false);
		}
	}

    private GameObject MakeIsland(GameObject prefab, Vector3 pos, Vector3? rot = null)
    {
        var island = new PropModule.DetailInfo()
        {
            path = prefab.name,
            position = pos,
            rotation = rot ?? Vector3.zero
		};
		return DetailBuilder.Make(_dreamworld.gameObject, _dreamworld.GetRootSector(), island);
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
