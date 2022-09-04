using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Handlers;

[RequireComponent(typeof(Main))]
internal class DestructionHandler : SolarSystemHandler
{
	protected override void OnSolarSystemAwake() { }

	protected override void OnSolarSystemStart()
	{
		// Remove all Hearthians
		foreach (var controller in GameObject.FindObjectsOfType<CharacterAnimController>())
		{
			controller.gameObject.SetActive(false);
		}

		// Turn off all campfires
		foreach (var campfire in GameObject.FindObjectsOfType<Campfire>())
		{
			campfire.SetState(Campfire.State.SMOLDERING, false);
		}

		// Remove all dialogue
		foreach (var dialogue in GameObject.FindObjectsOfType<CharacterDialogueTree>())
		{
			dialogue.gameObject.SetActive(false);
		}
	}
}
