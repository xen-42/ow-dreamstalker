﻿using Dreamstalker.Components.Dreamworld;
using Dreamstalker.Handlers.EyeScene;
using Dreamstalker.Utility;
using NewHorizons.Builder.Props;
using NewHorizons.External.Modules;
using NewHorizons.Utility;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class AncientGladeHandler : SolarSystemHandler
{
    private AstroObject _ancientGlade;
	private GameObject _sectorRoot;

	private List<MiniGalaxy> _miniGalaxies;
	private Campfire _campfire;
	private GameObject _solanum;
	private CharacterDialogueTree _solanumConversation;
	
	private GameObject _inflationOrb;
	private Light _inflationLight, _possibilityLight;
	private OWAudioSource _sfx;
	private float timer;

	protected override void BeforePlanetCreation() { }

	protected override void OnSolarSystemAwake() { }

    protected override void OnSolarSystemStart()
    {
        Main.Log("Ancient Glade handler invoked.");

		_miniGalaxies = new();

		_ancientGlade = AstroObjectLocator.GetAstroObject("Ancient Glade");

        var farClipController = _ancientGlade.gameObject.AddComponent<CameraLayerCullController>();
        farClipController.SetSector(_ancientGlade.GetRootSector());

		// Eye stuff
		var quantumCampfire = DetailBuilder.Make(_ancientGlade.gameObject, _ancientGlade.GetRootSector(), EyeHandler.QuantumCampfirePrefab, new PropModule.DetailInfo()
		{
			keepLoaded = true,
			position = new Vector3(-11.1801f, -99.2024f, 6.6523f),
			alignToNormal = true
		});

		// Spawn point
		var spawnGO = new GameObject("Spawn");
        spawnGO.transform.parent = _ancientGlade.transform;
        spawnGO.transform.localPosition = new Vector3(-18.40387f, -98.72745f, -3.800648f);
		spawnGO.transform.LookAt(quantumCampfire.transform, spawnGO.transform.localPosition.normalized);
        spawnGO.layer = 8;
        var spawn = spawnGO.AddComponent<SpawnPoint>();
        spawn._isShipSpawn = false;
        spawn._triggerVolumes = new OWTriggerVolume[] { _ancientGlade.GetComponentInChildren<Sector>()._owTriggerVolume };

		// Change ground material
		var ground = _ancientGlade.transform.Find("Sector/GroundSphere");
		ground.localScale = Vector3.one * 100.1f; // Makes the scatter look a bit better
		ground.localRotation = Quaternion.Euler(90, 0, 0);

		ground.GetComponent<MeshRenderer>().material =
			GameObject.Find("QuantumMoon_Body/Sector_QuantumMoon/State_TH/Geometry_THState/BatchedGroup/BatchedMeshRenderers_4")
			.GetComponent<MeshRenderer>()
			.material;

		for (int i = 0; i < 200; i++)
		{
			var pos = Random.onUnitSphere * (103f + Random.Range(-2f, 2f));
			_miniGalaxies.Add(DetailBuilder.Make(_ancientGlade.gameObject, _ancientGlade.GetRootSector(), EyeHandler.MiniGalaxyPrefab, new PropModule.DetailInfo()
			{
				keepLoaded = true,
				position = pos,
				alignToNormal = true,
				scale = 0.5f
			}).GetComponent<MiniGalaxy>());
		}

		_solanum = DetailBuilder.Make(_ancientGlade.gameObject, _ancientGlade.GetRootSector(), new PropModule.DetailInfo()
		{
			path = "QuantumMoon_Body/Sector_QuantumMoon/State_EYE/Interactables_EYEState/ConversationPivot/Character_NOM_Solanum/Nomai_ANIM_SkyWatching_Idle",
			keepLoaded = true
		});

		(_solanumConversation, _) = Main.Instance.NewHorizonsAPI.SpawnDialogue(Main.Instance, _solanum, "assets/xml/AncientGlade.xml", radius: 1.5f, range: 3f);
		_solanumConversation.transform.localPosition = Vector3.up * 2f;

		_solanumConversation.OnEndConversation += OnEndConversation;

		_inflationOrb = DetailBuilder.Make(_ancientGlade.gameObject, _ancientGlade.GetRootSector(), 
			EyeHandler.InflationPrefab, new PropModule.DetailInfo() { keepLoaded = true, removeComponents = true });

		_inflationOrb.transform.Find("PossibilitySphereRoot").localPosition = Vector3.zero;
		_inflationOrb.transform.Find("Effects_EYE_SmokeSphere").localPosition = new Vector3(0f, 6f, 0f);
		_inflationOrb.transform.Find("Effects_EYE_SmokeSphere").localRotation = Quaternion.Euler(0f, 180f, 180f);

		var norm = quantumCampfire.transform.localPosition.normalized;
		_inflationOrb.transform.localPosition = norm * 106f;
		_inflationOrb.transform.localRotation = Quaternion.FromToRotation(Vector3.up, -norm);

		_inflationLight = _inflationOrb.transform.Find("PossibilitySphereRoot/InflationLight").GetComponent<Light>();
		_possibilityLight = _inflationLight.transform.Find("PossibilitySphereRoot/Effects_EYE_PossibilityParticles2/PossibilityLight").GetComponent<Light>();
		_sfx = _inflationOrb.transform.Find("SFXAudioSource").GetComponent<OWAudioSource>();
		_sfx._audioSource.enabled = true;

		_inflationOrb.SetActive(false);

		_campfire = quantumCampfire.GetComponentInChildren<Campfire>();
		_campfire.OnCampfireStateChange += OnCampfireStateChange;

		// Fix gravity
		_ancientGlade.GetGravityVolume().SetPriority(2);

		_sectorRoot = _ancientGlade.GetRootSector().gameObject;
		_sectorRoot.SetActive(false);
		PlayerSpawnUtil.OnSpawn.AddListener(OnSpawn);
	}

	private void OnEndConversation()
	{
		_inflationOrb.SetActive(true);

		_solanumConversation.GetInteractVolume().SetInteractionEnabled(false);

		foreach (var miniGalaxy in _miniGalaxies)
		{
			miniGalaxy.DieAfterSeconds(Random.Range(0.5f, 4f), true, AudioType.EyeGalaxyBlowAway);
		}
	}

	private void OnCampfireStateChange(Campfire fire)
	{
		if (fire._state == Campfire.State.LIT)
		{
			foreach (var miniGalaxy in _miniGalaxies)
			{
				miniGalaxy.AppearAfterSeconds(Random.Range(0.5f, 4f));
			}

			_solanum.SetActive(true);
			var player = Locator.GetPlayerTransform();
			var globalPos = player.position - player.forward * 9f;
			var localPosNorm = _ancientGlade.transform.InverseTransformPoint(globalPos).normalized;
			_solanum.transform.localPosition = localPosNorm * 100.1f;
			_solanum.transform.LookAt(player, localPosNorm);
			_solanum.GetComponent<SolanumAnimController>().StartWatchingPlayer();

			fire.SetInteractionEnabled(false);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PlayerSpawnUtil.OnSpawn.RemoveListener(OnSpawn);
		_campfire.OnCampfireStateChange -= OnCampfireStateChange;
	}

	private void OnSpawn(AstroObject.Name planet)
	{
		if (planet == AstroObject.Name.Eye)
		{
			_sectorRoot.SetActive(true);
		}
		else
		{
			_sectorRoot.SetActive(false);
		}

		Main.Instance.ModHelper.Events.Unity.FireOnNextUpdate(() => Locator.GetFlashlight().TurnOn());
	}

	// TODO: this stuff should be on a component in the scene
	private void Update()
	{
		if (_inflationOrb != null && _inflationOrb.activeInHierarchy)
		{
			if (timer < Time.time)
			{
				timer = Time.time + 1f;
				_sfx.PlayOneShot(AudioType.EyeCosmicInflation, 1f);
			}
			var intensity = Mathf.Cos(20f * Time.time / Mathf.PI) / 2f + 0.5f;
			_inflationLight.intensity = intensity;
			_possibilityLight.intensity = intensity;
			_campfire._lightController._intensity = intensity;
		}
	}
}