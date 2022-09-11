using Dreamstalker.Components;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class GiantsDeepHandler : SolarSystemHandler
{
	protected override void OnSolarSystemAwake()
	{
		var gabbroIsland = GameObject.Find("GabbroIsland_Body");
		var sector = gabbroIsland.transform.Find("Sector_GabbroIsland").GetComponent<Sector>();

		// Has to be created right away to get all the goofy characters
		var quantumCharacter = new GameObject("QuantumCharacter");
		quantumCharacter.transform.parent = sector.transform;
		quantumCharacter.transform.position = gabbroIsland.transform.TransformPoint(new Vector3(-13.33444f, 2.299529f, 5.556587f));
		quantumCharacter.transform.localRotation = Quaternion.identity;

		var (dialogue, _) = Main.Instance.NewHorizonsAPI.SpawnDialogue(Main.Instance, quantumCharacter, "assets/xml/RandomDialogue.xml", radius:2, range:2);
		dialogue.gameObject.transform.localPosition = new Vector3(0, 1, 0);
		dialogue.gameObject.AddComponent<DialogueRandomizer>();

		var amalgam = quantumCharacter.AddComponent<QuantumAmalgam>();
		amalgam.SetSector(sector);
	}

	protected override void OnSolarSystemStart()
	{

	}
}
