using Dreamstalker.Components.Volumes;
using Dreamstalker.Utility;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class BrittleHollowHandler : SolarSystemHandler
{
	protected override void BeforePlanetCreation() { }

	protected override void OnSolarSystemAwake()
	{
		// Before NH can add the audio volume
		var th = GameObject.Find("BrittleHollow_Body");
		foreach (var audio in th.GetComponentsInChildren<AudioVolume>()) audio.enabled = false;
	}

	protected override void OnSolarSystemStart()
	{
		var bh = Locator.GetAstroObject(AstroObject.Name.BrittleHollow);

		// Weaken lights
		foreach (var light in bh.GetComponentsInChildren<Light>())
		{
			light.color = new Color(0.4f, 1f, 1f);
			light.intensity = 0.8f;
		}

		bh.transform.Find("AmbientLight_BH_Surface").GetComponent<Light>().intensity = 0.4f;
		bh.transform.Find("AmbientLight_BH_Interior").GetComponent<Light>().intensity = 0.2f;

		// Spawn stuff
		var bhCampfire = GameObject.Find("BrittleHollow_Body/Sector_BH/Effects_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();

		var bhCompletionVolume = GameObject.Find("BrittleHollow_Body/BlackHole_BH/BlackHoleVolume").AddComponent<BlackHoleCompletionVolume>();

		bhCompletionVolume.gameObject.GetComponent<BlackHoleVolume>().enabled = false;

		bhCompletionVolume.enabled = false;
		bhCompletionVolume.SetCampfire(bhCampfire);
		bhCompletionVolume.NextPlanet = AstroObject.Name.CaveTwin;

		SpawnWrapper.SpawnDreamstalker(bh, bhCampfire, bhCompletionVolume, Vector3.zero);
	}
}
