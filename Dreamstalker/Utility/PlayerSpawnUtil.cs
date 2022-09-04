using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Utility;

internal static class PlayerSpawnUtil
{
	public static void SpawnAt(AstroObject.Name planet)
	{
		var spawner = GameObject.FindObjectOfType<PlayerSpawner>();

		var spawn = spawner._spawnList.FirstOrDefault(x => x.GetSpawnLocation() == SpawnLocation.TimberHearth && x.IsShipSpawn() == false);

		var playerResources = GameObject.FindObjectOfType<PlayerResources>();

		var playerBody = Locator.GetPlayerBody();
		playerBody.WarpToPositionRotation(spawn.transform.position, spawn.transform.rotation);
		playerBody.SetVelocity(spawn.GetPointVelocity());
		spawn.AddObjectToTriggerVolumes(Locator.GetPlayerDetector().gameObject);
		spawn.AddObjectToTriggerVolumes(Locator.GetPlayerCamera().GetComponentInChildren<FluidDetector>().gameObject);
		spawn.OnSpawnPlayer();

		spawner._cameraController.SetDegreesY(80f);

		playerResources._isSuffocating = false;
		playerResources.DebugRefillResources();
		Main.FireOnNextUpdate(() => playerResources.enabled = true);
	}
}
