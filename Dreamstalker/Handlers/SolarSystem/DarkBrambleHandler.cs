using Dreamstalker.Components;
using Dreamstalker.Components.Player;
using Dreamstalker.Utility;
using NewHorizons.Builder.Props;
using NewHorizons.External.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class DarkBrambleHandler : SolarSystemHandler
{
	public static SpawnPoint EasterEggSpawnPoint { get; private set; }

	private StreamingGroup _streamingGroup;
	private CharacterDialogueTree _dialogue;

	private PlayerLockOnTargeting _lockOn;
	private Transform _ernesto;

	protected override void BeforePlanetCreation() { }

	protected override void OnSolarSystemAwake()
	{
		GlobalMessenger<float>.AddListener("EatMarshmallow", OnEatMarshmallow);
	}

	protected override void OnSolarSystemStart()
	{
		var db = Locator.GetAstroObject(AstroObject.Name.DarkBramble);

		var ernestoPos = new Vector3(-525.7968f, 617.6582f, -160.946f);
		var spawnPos = new Vector3(-524.4136f, 616.8699f, -164.5957f);


		// Spawn point
		var spawnGO = new GameObject("Spawn");
		spawnGO.transform.parent = db.transform;
		spawnGO.transform.localPosition = spawnPos;
		spawnGO.layer = 8;
		EasterEggSpawnPoint = spawnGO.AddComponent<SpawnPoint>();
		EasterEggSpawnPoint._isShipSpawn = false;
		EasterEggSpawnPoint._triggerVolumes = new OWTriggerVolume[] { db.GetRootSector()._owTriggerVolume };

		var ernesto = DetailBuilder.Make(db.gameObject, db.GetRootSector(), new PropModule.DetailInfo()
		{
			path = "Anglerfish_Body/Beast_Anglerfish",
			scale = 0.015f,
			removeChildren = new string[]
			{
				"B_angler_root/B_angler_body01/B_angler_body02/B_angler_antenna01/B_angler_antenna02/B_angler_antenna03/B_angler_antenna04/B_angler_antenna05/B_angler_antenna06/B_angler_antenna07/B_angler_antenna08/B_angler_antenna09/B_angler_antenna10/B_angler_antenna11/B_angler_antenna12_end"
			},
			keepLoaded = true,
			rotation = new Vector3(355.8887f, 156.5434f, 330.4391f)
		});

		ernesto.transform.position = db.transform.TransformPoint(ernestoPos);
		_ernesto = ernesto.transform;

		(_dialogue, _) = Main.Instance.NewHorizonsAPI.SpawnDialogue(Main.Instance, db.gameObject, "assets/xml/DarkBramble.xml", radius: 0);
		_dialogue.transform.parent = ernesto.transform;
		_dialogue.transform.localPosition = Vector3.zero;

		_dialogue.OnEndConversation += OnEndConversation;

		_lockOn = Locator.GetPlayerTransform().GetRequiredComponent<PlayerLockOnTargeting>();
	}

	private void OnEatMarshmallow(float _)
	{
		PlayerAttachPointController.Instance.Detatch();
		PlayerEffectController.Instance.WakeUp(gasp: false);
		PlayerSpawnUtil.SpawnAt(AstroObject.Name.DarkBramble);

		// Keep the previous location loaded
		var streamingGroup = Locator.GetAstroObject(PlayerSpawnUtil.SecondLastSpawn).GetComponentInChildren<StreamingGroup>();
		if (streamingGroup != null && PlayerSpawnUtil.SecondLastSpawn != AstroObject.Name.DreamWorld)
		{
			_streamingGroup = streamingGroup;
			_streamingGroup.RequestRequiredAssets(0);
			_streamingGroup.RequestGeneralAssets(0);
		}

		StartCoroutine(BeginErnestoSequence());
	}

	private IEnumerator BeginErnestoSequence()
	{
		OWInput.ChangeInputMode(InputMode.None);
		_lockOn.LockOn(_ernesto);
		yield return new WaitForSeconds(2f);
		_lockOn.BreakLock();
		OWInput.ChangeInputMode(InputMode.Character);
		_dialogue.StartConversation();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (_dialogue != null)
		{
			_dialogue.OnEndConversation -= OnEndConversation;
		}
		GlobalMessenger<float>.RemoveListener("EatMarshmallow", OnEatMarshmallow);
	}

	private void OnEndConversation()
	{
		if (_streamingGroup != null)
		{
			_streamingGroup.ReleaseRequiredAssets();
			_streamingGroup.ReleaseGeneralAssets();
			_streamingGroup = null;
		}

		PlayerSpawnUtil.SpawnAt(PlayerSpawnUtil.SecondLastSpawn);
		PlayerEffectController.Instance.Blink();
		GeneralHandler.TurnOffCampFires();
	}
}
