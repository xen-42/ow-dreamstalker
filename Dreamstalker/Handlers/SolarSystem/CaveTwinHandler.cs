using Dreamstalker.Components.Volumes;
using Dreamstalker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class CaveTwinHandler : SolarSystemHandler
{
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

		var ctVolume = new GameObject("CompletionVolume");
		ctVolume.transform.parent = ct.GetRootSector().transform;
		ctVolume.transform.localPosition = Vector3.zero;
		ctVolume.layer = OWLayerMask.effectVolumeMask;

		var sphere = ctVolume.AddComponent<SphereCollider>();
		sphere.isTrigger = true;
		sphere.radius = 20f;

		var ctCompletionVolume = ctVolume.AddComponent<CompletionVolume>();
		ctCompletionVolume.enabled = false;
		ctCompletionVolume.SetCampfire(ctCampfire);
		ctCompletionVolume.NextPlanet = AstroObject.Name.GiantsDeep;

		SpawnWrapper.SpawnDreamstalker(ct, ctCampfire, ctCompletionVolume, Vector3.zero);
	}
}
