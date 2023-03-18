using Dreamstalker.Utility;
using NewHorizons.Builder.Props;
using NewHorizons.External.Modules;
using NewHorizons.Utility;
using NewHorizons.Utility.OWMLUtilities;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dreamstalker.Components;

internal class QuantumAmalgam : QuantumObject
{
	private List<(GameObject character, bool isStrange)> _characters = new();
	private int _currentState;
	private bool _allowStrangeCharacters;
	public bool overrideAllowStrangeCharacters;
	private CharacterDialogueTree _dialogue;

	public override void Awake()
	{
		base.Awake();

		_dialogue = gameObject.GetComponentInChildren<CharacterDialogueTree>();

		if (_dialogue != null)
		{
			_dialogue.OnAdvancePage += OnAdvancePage;
			_dialogue.OnStartConversation += OnStartConversation;
			_dialogue.OnEndConversation += OnEndConversation;
		}

		var first = true;
		foreach (var character in GameObject.FindObjectsOfType<CharacterAnimController>())
		{
			if (character.gameObject.name.Equals("Villager_HEA_Spinel_ANIM_Fishing_ROD") ||
				character.gameObject.name.Equals("Villager_HEA_Tephra_ANIM_SitIdle")) continue;

			if (character.gameObject.name.Equals("Villager_HEA_Hal_ANIM_Museum") 
				&& character._animator.runtimeAnimatorController.name == "Villager_Hal_Outside") continue;
			if (character.gameObject.name.Equals("Villager_HEA_Galena_ANIM_Idle") 
				&& character._animator.runtimeAnimatorController.name == "Village_Kid_Hiding") continue;
			if (character.gameObject.name.Equals("Villager_HEA_Hornfels_ANIM_Working") 
				&& character._animator.runtimeAnimatorController.name == "Villager_Hornfels") continue;
			
			var detail = DetailBuilder.Make(gameObject, _sector, character.gameObject, new PropModule.DetailInfo() { keepLoaded = true });
			detail.transform.parent = gameObject.transform;
			detail.transform.localPosition = Vector3.zero;

			if (detail.name.Contains("Slate"))
			{
				var stump = DetailBuilder.Make(detail, _sector, new PropModule.DetailInfo()
				{
					path = "TimberHearth_Body/Sector_TH/Sector_QuantumGrove/Interactables_QuantumGrove/QuantumGrove_QuantumSockets/RootsSockets/Socket (3)/QuantumRoots_Proxy/OLD_Tree_TH_RootStump",
					scale = 0.2f,
					keepLoaded = true
				});
				stump.transform.parent = detail.transform;
				stump.transform.localPosition = new Vector3(0, 0, -0.7f);
			}

			detail.SetActive(first);

			var animController = detail.GetComponent<CharacterAnimController>();
			animController._dialogueTree = _dialogue;

			_characters.Add((detail.gameObject, false));
			first = false;
		}
		foreach(var character in GameObject.FindObjectsOfType<TravelerController>())
		{
			var detail = DetailBuilder.Make(gameObject, _sector, character.gameObject, new PropModule.DetailInfo() { keepLoaded = true });
			detail.transform.parent = gameObject.transform;
			detail.transform.localPosition = Vector3.zero;

			// TODO: Chair for chert, riebeck
			if (detail.gameObject.name.Contains("Villager_HEA_Esker"))
			{
				detail.transform.localPosition = new Vector3(0, 1.4f, 0);
				detail.transform.localRotation = Quaternion.Euler(9.6f, 0, 0);
				detail.GetComponentInChildren<EskerAnimController>().enabled = false;
			}
			else if (detail.name.Contains("Villager_HEA_Slate_ANIM_LogSit"))
			{
				// Add a chair
				var chair = DetailBuilder.Make(detail, _sector, new PropModule.DetailInfo() { 
					path = "Props_HEA_RockingChair:Props_HEA_RockingChair", position = new Vector3(0, 0, -0.6f),
					keepLoaded = true
				});
				chair.transform.parent = detail.transform;
				chair.transform.localPosition = Vector3.zero;

				Component.DestroyImmediate(chair.GetComponent<Animator>());
			}
			else if (detail.name.Contains("NPC_Player"))
			{
				detail.transform.Find("Traveller_Mesh_v01:Props_HEA_Jetpack").gameObject.SetActive(false);
				detail.transform.Find("Props_HEA_Lantern").gameObject.SetActive(false);
			}
			else if (detail.name.Contains("Traveller_HEA_Gabbro"))
			{
				detail.transform.localPosition = new Vector3(0, 1.1f, -1.9f);
				detail.transform.localRotation = Quaternion.Euler(90, 0, 0);
				detail.transform.Find("Traveller_LightingRig").gameObject.SetActive(false);
				detail.transform.Find("Props_HEA_Gabbro_ANIM_IdleFlute").gameObject.SetActive(false);
			}
			else if (detail.name.Contains("Riebeck"))
			{
				var stump = DetailBuilder.Make(detail, _sector, new PropModule.DetailInfo()
				{
					path = "TimberHearth_Body/Sector_TH/Sector_QuantumGrove/Interactables_QuantumGrove/QuantumGrove_QuantumSockets/RootsSockets/Socket (3)/QuantumRoots_Proxy/OLD_Tree_TH_RootStump",
					scale = 0.3f,
					keepLoaded = true
				});
				stump.transform.parent = detail.transform;
				stump.transform.localPosition = Vector3.zero;
			}
			else if (detail.name.Contains("Chert"))
			{
				var stump = DetailBuilder.Make(detail, _sector, new PropModule.DetailInfo()
				{
					path = "TimberHearth_Body/Sector_TH/Sector_QuantumGrove/Interactables_QuantumGrove/QuantumGrove_QuantumSockets/RootsSockets/Socket (3)/QuantumRoots_Proxy/OLD_Tree_TH_RootStump",
					keepLoaded = true
				});
				stump.transform.parent = detail.transform;
				stump.transform.localPosition = new Vector3(0, 0, -0.56f);
				stump.transform.localScale = new Vector3(0.2f, 0.1f, 0.2f);
			}

			detail.SetActive(false);

			var controller = detail.GetComponent<TravelerController>();
			controller._dialogueSystem = _dialogue;

			_characters.Add((detail.gameObject, false));
		}
		var nomai = DetailBuilder.Make(gameObject, _sector, new PropModule.DetailInfo() { 
			path = "QuantumMoon_Body/Sector_QuantumMoon/State_EYE/Interactables_EYEState/ConversationPivot/Character_NOM_Solanum/Nomai_ANIM_SkyWatching_Idle",
			scale = 0.9f,
			keepLoaded = true
		});
		nomai.transform.parent = gameObject.transform;
		nomai.transform.localPosition = Vector3.zero;

		nomai.SetActive(false);
		var nomaiController = nomai.GetComponent<SolanumAnimController>();
		nomaiController.StartWatchingPlayer();
		_characters.Add((nomai.gameObject, true));

		var ghostBird = DetailBuilder.Make(gameObject, _sector, new PropModule.DetailInfo() { 
			path = "DreamWorld_Body/Sector_DreamWorld/Sector_DreamZone_2/Ghosts_DreamZone_2/GhostNodeMap_HornetHouse/Prefab_IP_GhostBird_Hornet/Ghostbird_IP_ANIM",
			scale = 0.9f,
			keepLoaded = true
		});
		ghostBird.transform.parent = gameObject.transform;
		ghostBird.transform.localPosition = Vector3.zero;

		ghostBird.SetActive(false);
		_characters.Add((ghostBird.gameObject, true));

		var ernesto = DetailBuilder.Make(gameObject, _sector, new PropModule.DetailInfo() {
			path = "Anglerfish_Body/Beast_Anglerfish",
			scale = 0.015f,
			removeChildren = new string[] {
				"B_angler_root/B_angler_body01/B_angler_body02/B_angler_antenna01/B_angler_antenna02/B_angler_antenna03/B_angler_antenna04/B_angler_antenna05/B_angler_antenna06/B_angler_antenna07/B_angler_antenna08/B_angler_antenna09/B_angler_antenna10/B_angler_antenna11/B_angler_antenna12_end"
			},
			keepLoaded = true
		});
		ernesto.transform.parent = gameObject.transform;
		ernesto.transform.localPosition = new Vector3(0, 0.9f, 0);
		Delay.FireOnNextUpdate(() => ernesto.GetComponent<Animator>().enabled = true);

		ernesto.SetActive(false);
		_characters.Add((ernesto, true));

		var geswaldo = DetailBuilder.Make(gameObject, _sector, new PropModule.DetailInfo() { 
			path = "GiantsDeep_Body/Sector_GD/Sector_GDInterior/Jellyfish_GDInterior/Jellyfish_Pivot (1)/Jellyfish_Body/Beast_GD_Jellyfish_v4", 
			scale = 0.05f,
			keepLoaded = true
		});
		geswaldo.transform.parent = gameObject.transform;
		geswaldo.transform.localPosition = new Vector3(0, 2, 0);

		geswaldo.SetActive(false);
		_characters.Add((geswaldo, true));

		_visibilityTrackers = new VisibilityTracker[]
		{
			VisibilityUtility.AddVisibilityTracker(transform, 1f)
		};
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		if (_dialogue != null)
		{
			_dialogue.OnAdvancePage -= OnAdvancePage;
			_dialogue.OnStartConversation -= OnStartConversation;
			_dialogue.OnEndConversation -= OnEndConversation;
		}
	}

	public void OnStartConversation()
	{
		_allowStrangeCharacters = true;
	}

	public void OnEndConversation()
	{
		_allowStrangeCharacters = false;
		ChangeQuantumState(false);
		PlayerEffectController.Instance.Blink(0.5f);
	}

	public void OnAdvancePage(string _, int __)
	{
		ChangeQuantumState(false);
		PlayerEffectController.Instance.Blink(0.5f);
	}

	public override bool ChangeQuantumState(bool skipInstantVisibilityCheck)
	{
		if (_characters.Count != 1)
		{
			_characters[_currentState].character.SetActive(false);
			_characters[_currentState].character.SetActive(false);

			int newState;
			do
			{
				newState = Random.Range(0, _characters.Count - 1);
			}
			// Ghostbird, ernesto, geswaldo just have transforms
			while (newState == _currentState || (!(_allowStrangeCharacters || overrideAllowStrangeCharacters) && _characters[newState].isStrange));

			_characters[newState].character.SetActive(true);

			var animator = _characters[newState].character.GetComponent<Animator>();
			if (animator != null)
			{
				// Ernesto state is bugged
				animator.enabled = true;
			}

			_currentState = newState;
		}

		return true;
	}
}
