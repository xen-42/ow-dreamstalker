using NewHorizons.Builder.Props;
using NewHorizons.Components;
using NewHorizons.External.Modules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dreamstalker.Components
{
	internal class QuantumAmalgam : QuantumObject
	{
		private List<(GameObject character, bool isStrange)> _characters = new();
		private int _currentState;
		private bool _allowStrangeCharacters;
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
			else
			{
				// Don't want to scare somebody off from conversation with a strange character
				_allowStrangeCharacters = true;
			}

			var first = true;
			foreach (var character in GameObject.FindObjectsOfType<CharacterAnimController>())
			{
				if (character.gameObject.name == "Villager_HEA_Spinel_ANIM_Fishing_ROD" ||
					character.gameObject.name == "Villager_HEA_Tephra_ANIM_SitIdle") continue;

				if (character.gameObject.name == "Villager_HEA_Hal_ANIM_Museum" && character._animator.runtimeAnimatorController.name == "Villager_Hal_Outside") continue;
				if (character.gameObject.name == "Villager_HEA_Galena_ANIM_Idle" && character._animator.runtimeAnimatorController.name == "Village_Kid_Hiding") continue;
				if (character.gameObject.name == "Villager_HEA_Hornfels_ANIM_Working" && character._animator.runtimeAnimatorController.name == "Villager_Hornfels") continue;
				


				// TODO: Chair for chert, riebeck
				
				var detail = DetailBuilder.Make(gameObject, null, character.gameObject, new PropModule.DetailInfo() { });

				if (character.gameObject.name == "Villager_HEA_Esker")
				{
					detail.transform.localPosition = new Vector3(0, 1.4f, 0);
					detail.transform.localRotation = Quaternion.Euler(9.6f, 0, 0);
				}
				else if (character.gameObject.name == "Villager_HEA_Slate_ANIM_LogSit")
				{
					// Add a chair
					var chair = DetailBuilder.Make(detail, null, new PropModule.DetailInfo() { path = "Props_HEA_RockingChair:Props_HEA_RockingChair", position = new Vector3(0, 0, -0.6f) });
					Component.DestroyImmediate(chair.GetComponent<Animator>());
				}
				else if (character.gameObject.name == "NPC_Player")
				{
					detail.transform.Find("Traveller_Mesh_v01:Props_HEA_Jetpack").gameObject.SetActive(false);
					detail.transform.Find("Props_HEA_Lantern").gameObject.SetActive(false);
				}

				detail.SetActive(first);

				// Seemingly impossible to be null yet it is
				var animController = detail.GetComponent<CharacterAnimController>();
				if (animController != null) animController._dialogueTree = _dialogue;

				_characters.Add((detail.gameObject, false));
				first = false;
			}
			foreach(var character in GameObject.FindObjectsOfType<TravelerController>())
			{
				var detail = DetailBuilder.Make(gameObject, null, character.gameObject, new PropModule.DetailInfo() { });
				detail.SetActive(false);

				// Seemingly impossible to be null yet it is
				var controller = detail.GetComponent<TravelerController>();
				if (controller != null) controller._dialogueSystem = _dialogue;

				_characters.Add((detail.gameObject, false));
			}
			var nomai = DetailBuilder.Make(gameObject, null, new PropModule.DetailInfo() { 
				path = "QuantumMoon_Body/Sector_QuantumMoon/State_EYE/Interactables_EYEState/ConversationPivot/Character_NOM_Solanum/Nomai_ANIM_SkyWatching_Idle",
				scale = 0.9f
			});
			nomai.SetActive(false);
			var nomaiController = nomai.GetComponent<SolanumAnimController>();
			nomaiController.StartWatchingPlayer();
			_characters.Add((nomai.gameObject, true));

			var ghostBird = DetailBuilder.Make(gameObject, null, new PropModule.DetailInfo() { 
				path = "DreamWorld_Body/Sector_DreamWorld/Sector_DreamZone_2/Ghosts_DreamZone_2/GhostNodeMap_HornetHouse/Prefab_IP_GhostBird_Hornet/Ghostbird_IP_ANIM",
				scale = 0.9f
			});
			ghostBird.SetActive(false);
			_characters.Add((ghostBird.gameObject, true));

			var ernesto = DetailBuilder.Make(gameObject, null, new PropModule.DetailInfo() {
				path = "Anglerfish_Body/Beast_Anglerfish",
				scale = 0.02f,
				position = new Vector3(0, 0.9f, 0),
				removeChildren = new string[] {
						"B_angler_root/B_angler_body01/B_angler_body02/B_angler_antenna01/B_angler_antenna02/B_angler_antenna03/B_angler_antenna04/B_angler_antenna05/B_angler_antenna06/B_angler_antenna07/B_angler_antenna08/B_angler_antenna09/B_angler_antenna10/B_angler_antenna11/B_angler_antenna12_end"
				}
			});
			ernesto.SetActive(false);
			_characters.Add((ernesto, true));

			var geswaldo = DetailBuilder.Make(gameObject, null, new PropModule.DetailInfo() { 
				path = "GiantsDeep_Body/Sector_GD/Sector_GDInterior/Jellyfish_GDInterior/Jellyfish_Pivot (1)/Jellyfish_Body/Beast_GD_Jellyfish_v4", 
				scale = 0.05f ,
				position = new Vector3(0, 2, 0)
			});
			geswaldo.SetActive(false);
			_characters.Add((geswaldo, true));

			Main.Log($"Found {_characters.Count} characters");

			var visibilityTracker = new GameObject("VisibilityTracker_Sphere");
			visibilityTracker.transform.parent = transform;
			visibilityTracker.transform.localPosition = Vector3.up * 0.5f;

			var sphere = visibilityTracker.AddComponent<SphereShape>();
			sphere.radius = 1f;

			var tracker = visibilityTracker.AddComponent<ShapeVisibilityTracker>();
			_visibilityTrackers = new VisibilityTracker[] { tracker };
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

				var newState = _currentState;

				_characters[_currentState].character.SetActive(false);

				do
				{
					newState = Random.Range(0, _characters.Count - 1);
				}
				// Ghostbird, ernesto, geswaldo just have transforms
				while (newState == _currentState && (_allowStrangeCharacters || !_characters[newState].isStrange));

				_characters[newState].character.SetActive(true);

				_currentState = newState;
			}

			return true;
		}
	}
}
