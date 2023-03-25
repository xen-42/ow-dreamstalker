using Dreamstalker.Components;
using Dreamstalker.Components.Volumes;
using Dreamstalker.Utility;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class CaveTwinHandler : SolarSystemHandler
{
	protected override void BeforePlanetCreation() { }

	protected override void OnSolarSystemAwake()
	{
		// Before NH can add the audio volume
		var ct = GameObject.Find("CaveTwin_Body");
		foreach (var audio in ct.GetComponentsInChildren<AudioVolume>()) audio.enabled = false;
	}

	protected override void OnSolarSystemStart()
	{
		var ct = Locator.GetAstroObject(AstroObject.Name.CaveTwin);

		// Weaken lights
		foreach (var light in ct.GetComponentsInChildren<Light>())
		{
			light.color = new Color(0.4f, 1f, 1f);
			light.intensity = 0.8f;
		}

		ct.transform.Find("AmbientLight_CaveTwin").GetComponent<Light>().intensity = 0.4f;

		// Spawn stuff
		var ctCampfire = GameObject.Find("CaveTwin_Body/Sector_CaveTwin/Sector_NorthHemisphere/Sector_NorthSurface/Sector_Lakebed/Interactables_Lakebed/Lakebed_VisibleFrom_Far/Prefab_HEA_Campfire/Controller_Campfire").GetComponent<Campfire>();

		var ctCompletionVolume = CompletionVolume.MakeCompletionVolume(ct, ctCampfire, AstroObject.Name.GiantsDeep,
			new Vector3(-93.88305f, -71.32361f, 60.62481f), 20f);
		ctCompletionVolume.killWithoutLitCampfire = true;

		var proximitySound = ctCompletionVolume.gameObject.AddComponent<ProximitySound>();
		proximitySound.audio = AudioType.NomaiEscapePodDistressSignal_LP;
		proximitySound.linkedCampfire = ctCampfire;
		proximitySound.radius = 30f;

		SpawnWrapper.SpawnDreamstalker(ct, ctCampfire, ctCompletionVolume, Vector3.zero);
	}
}
