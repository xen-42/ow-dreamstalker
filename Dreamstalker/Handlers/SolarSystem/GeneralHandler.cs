using Dreamstalker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

[RequireComponent(typeof(Main))]
internal class GeneralHandler : SolarSystemHandler
{
    protected override void OnSolarSystemAwake() { }

    protected override void OnSolarSystemStart()
    {
        // Remove all Hearthians
        foreach (var controller in FindObjectsOfType<CharacterAnimController>())
        {
            controller.gameObject.SetActive(false);
        }

        // Turn off all campfires
        foreach (var campfire in FindObjectsOfType<Campfire>())
        {
            campfire.SetState(Campfire.State.SMOLDERING, false);
            CampfireTinter.Tint(campfire);
        }

        // Remove all dialogue
        foreach (var dialogue in FindObjectsOfType<CharacterDialogueTree>())
        {
            dialogue.gameObject.SetActive(false);
        }

		// Tint marshmallow 
		CampfireTinter.TintMarshmallow();
	}
}
