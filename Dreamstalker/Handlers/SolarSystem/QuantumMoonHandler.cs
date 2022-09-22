using Dreamstalker.Utility;
using NewHorizons.Utility;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class QuantumMoonHandler : SolarSystemHandler
{
    private AstroObject _quantumMoon;
	private GameObject _sectorRoot;

	protected override void BeforePlanetCreation() { }

	protected override void OnSolarSystemAwake() { }

    protected override void OnSolarSystemStart()
    {
        Main.Log("Quantum Moon handler invoked.");

		_quantumMoon = AstroObjectLocator.GetAstroObject("Custom Quantum Moon");

		// Spawn point
		var spawnGO = new GameObject("Spawn");
        spawnGO.transform.parent = _quantumMoon.transform;
		spawnGO.transform.localPosition = new Vector3(53.21917f, -39.7816f, -22.60495f);
        spawnGO.layer = 8;
        var spawn = spawnGO.AddComponent<SpawnPoint>();
        spawn._isShipSpawn = false;
        spawn._triggerVolumes = new OWTriggerVolume[] { _quantumMoon.GetComponentInChildren<Sector>()._owTriggerVolume };

		// Fix gravity
		_quantumMoon.GetGravityVolume().SetPriority(2);

		_sectorRoot = _quantumMoon.GetRootSector().gameObject;
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
		if (planet == AstroObject.Name.QuantumMoon)
		{
			_sectorRoot.SetActive(true);
		}
		else
		{
			_sectorRoot.SetActive(false);
		}
	}
}
