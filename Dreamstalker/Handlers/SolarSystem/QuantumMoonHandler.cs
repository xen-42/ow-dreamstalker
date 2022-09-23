using Dreamstalker.Components.QuantumMoon;
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

		_quantumMoon.gameObject.AddComponent<QuantumMoonController>().SetSector(_quantumMoon.GetRootSector());

		// Spawn point
		var spawnGO = new GameObject("Spawn");
        spawnGO.transform.parent = _quantumMoon.transform;
		spawnGO.transform.localPosition = new Vector3(-12.61486f, -73.44263f, -2.316277f);
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
