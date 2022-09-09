using Dreamstalker.Components;
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

    protected override void OnSolarSystemAwake() 
    {
		// Want to do this before NH starts creating any of it

		// Remove all dialogue
		foreach (var dialogue in FindObjectsOfType<CharacterDialogueTree>())
		{
			// Super hacky, but if the parent is a sector then NH made it. I'm too lazy to make this good.
			if (dialogue.transform.parent.GetComponent<Sector>() != null) continue;
			dialogue.gameObject.SetActive(false);
		}

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

		// Remove all signals (since some is played out loud)
		foreach (var signal in FindObjectsOfType<AudioSignal>())
		{
			signal.gameObject.SetActive(false);
		}
	}

    protected override void OnSolarSystemStart()
    {
		// Turn off all campfires (has to happen after campfire Awake
		TurnOffCampFires();

		Locator.GetPlayerBody().gameObject.AddComponent<PlayerEffectController>();
		Locator.GetPlayerBody().gameObject.AddComponent<DebugCommands>();
	}

	public static void TurnOffCampFires()
	{
		foreach (var campfire in FindObjectsOfType<Campfire>())
		{
			campfire.SetState(Campfire.State.SMOLDERING, false);
		}
	}
}
