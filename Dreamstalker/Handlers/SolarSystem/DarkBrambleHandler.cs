using Dreamstalker.Components;
using Dreamstalker.Utility;
using NewHorizons.Builder.Props;
using NewHorizons.External.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class DarkBrambleHandler : SolarSystemHandler
{
	public static SpawnPoint EasterEggSpawnPoint { get; private set; }

	private RemoteDialogueTrigger _dialogueTrigger;
	private CharacterDialogueTree _dialogue;

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

		(_dialogue, _dialogueTrigger) = Main.Instance.NewHorizonsAPI.SpawnDialogue(Main.Instance, db.gameObject, "assets/xml/DarkBramble.xml", remoteTriggerRadius: 2);
		_dialogue.transform.parent = ernesto.transform;
		_dialogue.transform.localPosition = Vector3.zero;

		_dialogueTrigger.transform.localPosition = spawnPos;
		_dialogueTrigger._deactivateTriggerPostConversation = false;

		_dialogue.OnEndConversation += OnEndConversation;
	}

	private void OnEatMarshmallow(float _)
	{
		PlayerAttachPointController.Instance.Detatch();
		PlayerSpawnUtil.SpawnAt(AstroObject.Name.DarkBramble);
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
		PlayerSpawnUtil.SpawnAt(PlayerSpawnUtil.SecondLastSpawn);
		PropHandler.TurnOffCampFires();
		_dialogueTrigger.enabled = true;
	}
}
