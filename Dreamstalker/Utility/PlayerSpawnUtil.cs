using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Utility;

internal static class PlayerSpawnUtil
{
	private static PlayerSpawner GetPlayerSpawner()
	{
		if (_playerSpawner == null) _playerSpawner = GameObject.FindObjectOfType<PlayerSpawner>();
		return _playerSpawner;
	}
	private static PlayerSpawner _playerSpawner;

	private static SpawnPoint GetSpawnPoint(AstroObject.Name planet) => planet switch
	{
		AstroObject.Name.CaveTwin => GameObject.Find("CaveTwin_Body/SPAWNS/Spawn_ChertsCamp").GetComponent<SpawnPoint>(),
		AstroObject.Name.GiantsDeep => GameObject.Find("GabbroIsland_Body/Sector_GabbroIsland/Spawn_GabbroIsland").GetComponent<SpawnPoint>(),
		AstroObject.Name.BrittleHollow => GameObject.Find("BrittleHollow_Body/SPAWNS_PLAYER/SPAWN_Observatory").GetComponent<SpawnPoint>(),
		_ => GetPlayerSpawner()._spawnList.FirstOrDefault(x => x.GetSpawnLocation() == GetSpawnLocation(planet) && x.IsShipSpawn() == false)
	};

	private static SpawnLocation GetSpawnLocation(AstroObject.Name planet) => planet switch
	{
		AstroObject.Name.TimberHearth => SpawnLocation.TimberHearth,
		AstroObject.Name.CaveTwin => SpawnLocation.HourglassTwin_1,
		AstroObject.Name.TowerTwin => SpawnLocation.HourglassTwin_2,
		AstroObject.Name.GiantsDeep => SpawnLocation.GasGiant,
		AstroObject.Name.BrittleHollow => SpawnLocation.BrittleHollow,
		AstroObject.Name.DarkBramble => SpawnLocation.DarkBramble,
		_ => SpawnLocation.TimberHearth
	};

	public static void SpawnAt(AstroObject.Name planet)
	{
		var spawn = GetSpawnPoint(planet);

		var playerResources = GameObject.FindObjectOfType<PlayerResources>();

		var playerBody = Locator.GetPlayerBody();
		playerBody.WarpToPositionRotation(spawn.transform.position, spawn.transform.rotation);
		playerBody.SetVelocity(spawn.GetPointVelocity());
		spawn.AddObjectToTriggerVolumes(Locator.GetPlayerDetector().gameObject);
		spawn.AddObjectToTriggerVolumes(Locator.GetPlayerCamera().GetComponentInChildren<FluidDetector>().gameObject);
		spawn.OnSpawnPlayer();

		GetPlayerSpawner()._cameraController.SetDegreesY(80f);

		playerResources._isSuffocating = false;
		playerResources.DebugRefillResources();
		Main.FireOnNextUpdate(() => playerResources.enabled = true);
	}
}
