using Dreamstalker.Components;
using Dreamstalker.Components.Player;
using Dreamstalker.External;
using Dreamstalker.Handlers.SolarSystem;
using NewHorizons.Utility;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Dreamstalker.Utility;

internal static class PlayerSpawnUtil
{
	public class SpawnEvent : UnityEvent<AstroObject.Name> { }
	public static SpawnEvent Spawn = new();
	public static AstroObject.Name LastSpawn = AstroObject.Name.TimberHearth;

	private static PlayerSpawner GetPlayerSpawner()
	{
		if (_playerSpawner == null) _playerSpawner = GameObject.FindObjectOfType<PlayerSpawner>();
		return _playerSpawner;
	}
	private static PlayerSpawner _playerSpawner;

	public static SpawnPoint GetSpawnPoint(AstroObject.Name planet) => planet switch
	{
		AstroObject.Name.CaveTwin => GameObject.Find("CaveTwin_Body/SPAWNS/Spawn_ChertsCamp").GetComponent<SpawnPoint>(),
		AstroObject.Name.GiantsDeep => GameObject.Find("GabbroIsland_Body/Sector_GabbroIsland/Spawn_GabbroIsland").GetComponent<SpawnPoint>(),
		AstroObject.Name.BrittleHollow => GameObject.Find("BrittleHollow_Body/SPAWNS_PLAYER/SPAWN_Observatory").GetComponent<SpawnPoint>(),
		AstroObject.Name.DreamWorld => GameObject.Find("Dreaming_Body/Spawn").GetComponent<SpawnPoint>(),
		AstroObject.Name.DarkBramble => DarkBrambleHandler.EasterEggSpawnPoint,
		AstroObject.Name.QuantumMoon => GameObject.Find("CustomQuantumMoon_Body/Spawn").GetComponent<SpawnPoint>(),
		AstroObject.Name.Eye => GameObject.Find("AncientGlade_Body/Spawn").GetComponent<SpawnPoint>(),
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
		Main.Log($"Spawning at {planet}");

		PlayerAttachPointController.Instance.Detatch();

		OWInput.ChangeInputMode(InputMode.Character);
		Locator.GetPauseCommandListener().AddPauseCommandLock();

		// Have to do it next update idk why
		Delay.FireOnNextUpdate(() => {
			OWInput.ChangeInputMode(InputMode.Character);
			Locator.GetPauseCommandListener().RemovePauseCommandLock();
		});

		Spawn?.Invoke(planet);

		// Never respawn at DB
		if (planet != AstroObject.Name.DarkBramble && LastSpawn != planet)
		{
			LastSpawn = planet;
		}

		var spawn = GetSpawnPoint(planet);

		var playerResources = GameObject.FindObjectOfType<PlayerResources>();

		var playerBody = Locator.GetPlayerBody();
		playerBody.WarpToPositionRotation(spawn.transform.position, spawn.transform.rotation);
		playerBody.SetVelocity(spawn.GetPointVelocity());
		spawn.AddObjectToTriggerVolumes(Locator.GetPlayerDetector().gameObject);
		spawn.AddObjectToTriggerVolumes(Locator.GetPlayerCamera().GetComponentInChildren<FluidDetector>().gameObject);
		spawn.OnSpawnPlayer();

		if (planet != AstroObject.Name.Eye)
		{
			GetPlayerSpawner()._cameraController.SetDegreesY(80f);
		}

		playerResources._isSuffocating = false;
		playerResources.DebugRefillResources();
		Main.FireOnNextUpdate(() => playerResources.enabled = true);

		PlayerEffectController.Instance.SetStatic(0f);

		if (planet != AstroObject.Name.DarkBramble)
		{
			DreamstalkerData.Save();
		}

		foreach (var cullGroup in spawn.GetAttachedOWRigidbody().GetComponentsInChildren<SectorCullGroup>())
		{
			cullGroup.SetVisible(true, true);
		}
	}

	public static void Respawn()
	{
		SpawnAt(LastSpawn);
	}
}
