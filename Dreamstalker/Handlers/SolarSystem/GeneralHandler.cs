using Dreamstalker.Components;
using Dreamstalker.Components.Player;
using Dreamstalker.External;
using Dreamstalker.Utility;
using NewHorizons.Components.SizeControllers;
using System;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

[RequireComponent(typeof(Main))]
internal class GeneralHandler : SolarSystemHandler
{
	protected override void Awake()
	{
		base.Awake();
	}

	protected override void BeforePlanetCreation()
	{
		// Remove all dialogue
		foreach (var dialogue in FindObjectsOfType<CharacterDialogueTree>())
		{
			dialogue.gameObject.SetActive(false);
		}

		foreach (var remoteTrigger in FindObjectsOfType<RemoteDialogueTrigger>())
		{
			remoteTrigger.gameObject.SetActive(false);
		}

		// Remove all signals (since some is played out loud)
		foreach (var signal in FindObjectsOfType<AudioSignal>())
		{
			signal.gameObject.SetActive(false);
		}

		// Remove all audio volumes
		foreach (var audioVolume in FindObjectsOfType<AudioVolume>())
		{
			GameObject.Destroy(audioVolume);
		}

		// Ice melt controller freaks out without the audio volumes but we don't want that to exist anyway
		GameObject.Destroy(GameObject.FindObjectOfType<IceMeltController>());

		// Remove all nomai text
		foreach (var nomaiText in FindObjectsOfType<NomaiWallText>())
		{
			nomaiText.gameObject.SetActive(false);
		}

		// Remove all scrolls
		foreach (var scrollItem in FindObjectsOfType<ScrollItem>())
		{
			scrollItem.gameObject.SetActive(false);
		}

		// Disable all sockets
		foreach (var itemSocket in FindObjectsOfType<OWItemSocket>())
		{
			itemSocket.EnableInteraction(false);
		}

		// Disable all supernova related garbage
		foreach (var supernova in FindObjectsOfType<SupernovaEffectController>())
		{
			supernova.enabled = false;
		}

		foreach (var supernova in FindObjectsOfType<SupernovaStreamersController>())
		{
			supernova.enabled = false;
		}

		foreach (var star in FindObjectsOfType<StarEvolutionController>())
		{
			star.enabled = false;
		}
	}

	protected override void OnSolarSystemAwake() { }

	protected override void OnSolarSystemStart()
	{
		// Doing these on Start so they can be found on Awake elsewhere
		// Remove all Hearthians
		foreach (var controller in FindObjectsOfType<CharacterAnimController>())
		{
			controller.gameObject.SetActive(false);
		}

		// Remove all travelers
		foreach (var traveler in FindObjectsOfType<TravelerController>())
		{
			traveler.gameObject.SetActive(false);
		}

		// Turn off all campfires (has to happen after campfire Awake
		TurnOffCampFires();

		Locator.GetPlayerBody().gameObject.AddComponent<PlayerEffectController>();
		Locator.GetPlayerBody().gameObject.AddComponent<PlayerAttachPointController>();
		Locator.GetPlayerBody().gameObject.AddComponent<DebugCommands>();

		DreamstalkerData.Load();
		if (Enum.TryParse<AstroObject.Name>(DreamstalkerData.ActiveProfile.LastPlanet, out var planet))
		{
			Main.Instance.ModHelper.Events.Unity.FireInNUpdates(() => PlayerSpawnUtil.SpawnAt(planet), 100);
		}
	}

	public static void TurnOffCampFires()
	{
		foreach (var campfire in FindObjectsOfType<Campfire>())
		{
			campfire.SetState(Campfire.State.SMOLDERING, false);
		}
	}
}
