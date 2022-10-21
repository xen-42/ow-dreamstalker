using Dreamstalker.Components;
using Dreamstalker.Components.Volumes;
using Dreamstalker.Utility;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class GiantsDeepHandler : SolarSystemHandler
{
	private CharacterDialogueTree _note;
	private GameObject[] _amalgams;

	protected override void BeforePlanetCreation() { }

	protected override void OnSolarSystemAwake()
	{
		var gabbroIsland = GameObject.Find("GabbroIsland_Body");
		var sector = gabbroIsland.transform.Find("Sector_GabbroIsland").GetComponent<Sector>();

		_amalgams = new GameObject[]
		{
			MakeAmalgam(gabbroIsland, sector, new Vector3(-13.33444f, 2.299529f, 5.556587f)),
			MakeAmalgam(gabbroIsland, sector, new Vector3(-18.42652f, 1.382271f, 23.15298f)),
			MakeAmalgam(gabbroIsland, sector, new Vector3(-9.566402f, 0.8526618f, 33.07628f))
		};

		// Stop all tornados
		foreach (var torando in GameObject.FindObjectsOfType<TornadoController>())
		{
			GameObject.Destroy(torando.gameObject);
		}

		// Ambient light
		var gd = GameObject.Find("GiantsDeep_Body");
		gd.transform.Find("AmbientLight_GD").GetComponent<Light>().intensity = 0.2f;

		var campfire = gabbroIsland.transform.Find("Sector_GabbroIsland/Interactables_GabbroIsland/Prefab_HEA_Campfire/Controller_Campfire");
		campfire.gameObject.AddComponent<CompletionCampfire>().destinationPlanet = AstroObject.Name.DreamWorld;

		var proximitySound = campfire.gameObject.AddComponent<ProximitySound>();
		proximitySound.audio = AudioType.PartyHouse_Vocals;
		proximitySound.radius = 10f;

		_note = sector.transform.Find("GabbroIslandNote").GetComponent<CharacterDialogueTree>();

		_note.OnEndConversation += OnReadNote;

		PlayerSpawnUtil.OnSpawn.AddListener(OnSpawn);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if (_note != null)
		{
			_note.OnEndConversation -= OnReadNote;
		}

		PlayerSpawnUtil.OnSpawn.RemoveListener(OnSpawn);
	}

	protected override void OnSolarSystemStart() { }

	private void OnSpawn(AstroObject.Name name)
	{
		foreach (var character in _amalgams)
		{
			character.SetActive(true);
		}
	}

	private void OnReadNote()
	{
		foreach (var character in _amalgams)
		{
			character.SetActive(false);
		}
	}

	private GameObject MakeAmalgam(GameObject rootObject, Sector sector, Vector3 position)
	{
		// Has to be created right away to get all the goofy characters
		var quantumCharacter = new GameObject("QuantumCharacter");
		quantumCharacter.SetActive(false);

		quantumCharacter.transform.parent = sector.transform;
		quantumCharacter.transform.position = rootObject.transform.TransformPoint(position);
		quantumCharacter.transform.localRotation = Quaternion.identity;

		var (dialogue, _) = Main.Instance.NewHorizonsAPI.SpawnDialogue(Main.Instance, quantumCharacter, "assets/xml/RandomDialogue.xml", radius: 1, range: 2);
		dialogue.gameObject.transform.localPosition = new Vector3(0, 1, 0);
		dialogue.gameObject.AddComponent<DialogueRandomizer>();

		var amalgam = quantumCharacter.AddComponent<QuantumAmalgam>();
		amalgam.SetSector(sector);

		quantumCharacter.SetActive(true);

		return quantumCharacter;
	}
}
