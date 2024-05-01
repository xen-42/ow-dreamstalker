using Dreamstalker.Components.Dreamstalker;
using Dreamstalker.Components.Volumes;
using Dreamstalker.Utility;
using System;
using UnityEngine;

namespace Dreamstalker.Components.QuantumMoon;

internal class QuantumMoonController : SectoredMonoBehaviour
{
	private GameObject BHState, DBState, EyeState, GDState, HTState, THState;
	private GameObject BHNomai, DBNomai, GDNomai, HTNomai, THNomai;
	private GameObject BHShrine, DBShrine, GDShrine, HTShrine, THShrine;
	private GameObject[] _shrines;

	private Campfire BHCampfire, DBCampfire, GDCampfire, HTCampfire, THCampfire;
	private Campfire[] _campfires;

	private CharacterDialogueTree _note;

	private DreamstalkerController _dreamstalker;

	public enum QMState
	{
		BrittleHollow,
		DarkBramble,
		Eye,
		GiantsDeep,
		HourglassTwins,
		TimberHearth
	}

	public override void Awake()
	{
		base.Awake();

		try
		{
			BHState = transform.Find("Sector/State_BH")?.gameObject;
			DBState = transform.Find("Sector/State_DB")?.gameObject;
			EyeState = transform.Find("Sector/State_EYE")?.gameObject;
			GDState = transform.Find("Sector/State_GD")?.gameObject;
			HTState = transform.Find("Sector/State_HT")?.gameObject;
			THState = transform.Find("Sector/State_TH")?.gameObject;

			BHNomai = transform.Find("Sector/State_BH/Interactables_BHState/QuantumDeadNomaiSuit")?.gameObject;
			DBNomai = transform.Find("Sector/State_DB/Interactables_DBState/QuantumDeadNomaiSuit")?.gameObject;
			GDNomai = transform.Find("Sector/State_GD/Interactables_GDState/QuantumDeadNomaiSuit")?.gameObject;
			HTNomai = transform.Find("Sector/State_HT/Interactables_HTState/QuantumDeadNomaiSuit")?.gameObject;
			THNomai = transform.Find("Sector/State_TH/Interactables_THState/QuantumDeadNomaiSuit")?.gameObject;

			BHCampfire = BHState.GetComponentInChildren<Campfire>();
			DBCampfire = DBState.GetComponentInChildren<Campfire>();
			GDCampfire = GDState.GetComponentInChildren<Campfire>();
			HTCampfire = HTState.GetComponentInChildren<Campfire>();
			THCampfire = THState.GetComponentInChildren<Campfire>();

			SetUpShrine(2, QMState.Eye, BHState, ref BHShrine);
			SetUpShrine(4, QMState.GiantsDeep, DBState, ref DBShrine);
			SetUpShrine(3, QMState.BrittleHollow, GDState, ref GDShrine);
			SetUpShrine(0, QMState.DarkBramble, HTState, ref HTShrine);
			SetUpShrine(1, QMState.HourglassTwins, THState, ref THShrine);

			_campfires = GetComponentsInChildren<Campfire>();
			_shrines = new GameObject[]
			{
				BHShrine,
				DBShrine,
				GDShrine,
				HTShrine,
				THShrine
			};

			_note = GetComponentInChildren<CharacterDialogueTree>();

			BHCampfire.OnCampfireStateChange += BHCampfire_OnCampfireStateChange;
			DBCampfire.OnCampfireStateChange += DBCampfire_OnCampfireStateChange;
			GDCampfire.OnCampfireStateChange += GDCampfire_OnCampfireStateChange;
			HTCampfire.OnCampfireStateChange += HTCampfire_OnCampfireStateChange;
			THCampfire.OnCampfireStateChange += THCampfire_OnCampfireStateChange;

			var quantumMoon = gameObject.GetComponent<AstroObject>();

			var completionVolume = CompletionVolume.MakeCompletionVolume(quantumMoon, null,
				AstroObject.Name.Eye, new Vector3(-5.315461f, -68.58476f, 5.373294f), 10f).transform.parent = EyeState.transform;

			var proximitySound = completionVolume.gameObject.AddComponent<ProximitySound>();
			proximitySound.audio = AudioType.EyeVortex_LP;
			proximitySound.radius = 20f;

			_dreamstalker = SpawnWrapper.SpawnDreamstalker(gameObject.GetComponent<AstroObject>(), null, null, Vector3.zero);

			BHCampfire.OnCampfireStateChange += _dreamstalker.OnCampfireStateChange;
			DBCampfire.OnCampfireStateChange += _dreamstalker.OnCampfireStateChange;
			GDCampfire.OnCampfireStateChange += _dreamstalker.OnCampfireStateChange;
			HTCampfire.OnCampfireStateChange += _dreamstalker.OnCampfireStateChange;
			THCampfire.OnCampfireStateChange += _dreamstalker.OnCampfireStateChange;

			DestroyImmediate(GetComponentInChildren<NomaiConversationManager>());

			PlayerSpawnUtil.Spawn.AddListener(OnSpawn);
		}
		catch (Exception e)
		{
			Main.LogError($"Couldn't awake quantum moon controller : {e}");
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();

		BHCampfire.OnCampfireStateChange -= BHCampfire_OnCampfireStateChange;
		DBCampfire.OnCampfireStateChange -= DBCampfire_OnCampfireStateChange;
		GDCampfire.OnCampfireStateChange -= GDCampfire_OnCampfireStateChange;
		HTCampfire.OnCampfireStateChange -= HTCampfire_OnCampfireStateChange;
		THCampfire.OnCampfireStateChange -= THCampfire_OnCampfireStateChange;

		if (_dreamstalker != null)
		{
			BHCampfire.OnCampfireStateChange -= _dreamstalker.OnCampfireStateChange;
			DBCampfire.OnCampfireStateChange -= _dreamstalker.OnCampfireStateChange;
			GDCampfire.OnCampfireStateChange -= _dreamstalker.OnCampfireStateChange;
			HTCampfire.OnCampfireStateChange -= _dreamstalker.OnCampfireStateChange;
			THCampfire.OnCampfireStateChange -= _dreamstalker.OnCampfireStateChange;
		}

		PlayerSpawnUtil.Spawn.RemoveListener(OnSpawn);
	}

	private void OnSpawn(AstroObject.Name planet)
	{
		if (planet == AstroObject.Name.QuantumMoon)
		{
			SetState(QMState.TimberHearth);
		}
	}

	private void BHCampfire_OnCampfireStateChange(Campfire fire)
	{
		BHNomai.SetActive(fire._state != Campfire.State.LIT);
		BHShrine.SetActive(fire._state == Campfire.State.LIT);
	}

	private void DBCampfire_OnCampfireStateChange(Campfire fire)
	{
		DBNomai.SetActive(fire._state != Campfire.State.LIT);
		DBShrine.SetActive(fire._state == Campfire.State.LIT);
	}

	private void GDCampfire_OnCampfireStateChange(Campfire fire)
	{
		GDNomai.SetActive(fire._state != Campfire.State.LIT);
		GDShrine.SetActive(fire._state == Campfire.State.LIT);
	}

	private void HTCampfire_OnCampfireStateChange(Campfire fire)
	{
		HTNomai.SetActive(fire._state != Campfire.State.LIT);
		HTShrine.SetActive(fire._state == Campfire.State.LIT);
	}

	private void THCampfire_OnCampfireStateChange(Campfire fire)
	{
		THNomai.SetActive(fire._state != Campfire.State.LIT);
		THShrine.SetActive(fire._state == Campfire.State.LIT);
	}

	private void SetUpShrine(int index, QMState nextState, GameObject state, ref GameObject shrine)
	{
		try
		{
			var compass = state.GetComponentInChildren<QuantumMoonCompass>();
			compass._degrees = index * 180f / 5f;
			shrine = compass.transform.parent.gameObject;
			shrine.SetActive(false);
			DestroyImmediate(compass);

			var volume = new GameObject("Volume")
			{
				layer = LayerMask.NameToLayer("BasicEffectVolume")
			};

			volume.AddComponent<SphereShape>().radius = 5f;

			var trigger = volume.AddComponent<OWTriggerVolume>();

			trigger.OnEntry += (GameObject hitObj) => Trigger_OnEntry(nextState, hitObj);
			volume.transform.parent = shrine.transform;
			volume.transform.localPosition = Vector3.zero;
		}
		catch (Exception e)
		{
			Main.LogError($"Failed to setup shrine {state.name} : {e}");
		}
	}

	private void Trigger_OnEntry(QMState nextState, GameObject hitObj)
	{
		if (hitObj == Locator.GetPlayerDetector())
		{
			SetState(nextState);

			PlayerEffectController.Instance.Blink();

			AudioUtility.PlayOneShot(AudioType.EyeVortexExit);

			_dreamstalker?.DespawnImmediate();
		}
	}

	public void SetState(QMState state)
	{
		BHState.SetActive(state == QMState.BrittleHollow);
		DBState.SetActive(state == QMState.DarkBramble);
		EyeState.SetActive(state == QMState.Eye);
		GDState.SetActive(state == QMState.GiantsDeep);
		HTState.SetActive(state == QMState.HourglassTwins);
		THState.SetActive(state == QMState.TimberHearth);

		_note.gameObject.SetActive(state == QMState.TimberHearth);

		foreach (var campfire in _campfires)
		{
			campfire.SetState(Campfire.State.UNLIT, true);
		}

		foreach (var shrine in _shrines)
		{
			shrine.SetActive(false);
		}
	}
}
