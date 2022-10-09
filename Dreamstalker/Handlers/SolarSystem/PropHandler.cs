using Dreamstalker.Components;
using Dreamstalker.Components.Player;
using Dreamstalker.External;
using Dreamstalker.Utility;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Dreamstalker.Handlers.SolarSystem;

[RequireComponent(typeof(Main))]
internal class PropHandler : SolarSystemHandler
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
			Main.Instance.ModHelper.Events.Unity.FireOnNextUpdate(() => PlayerSpawnUtil.SpawnAt(planet));
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
