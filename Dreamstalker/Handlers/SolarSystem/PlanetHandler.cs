using Dreamstalker.Components;
using NewHorizons.Builder.Atmosphere;
using NewHorizons.External.Configs;
using NewHorizons.External.Modules;
using NewHorizons.Utility;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class PlanetHandler : SolarSystemHandler
{
	protected override void BeforePlanetCreation() { }

	protected override void OnSolarSystemAwake() { }

    protected override void OnSolarSystemStart()
    {
		// Turn prompts back on
		GUIMode.SetRenderMode(GUIMode.RenderMode.FPS);

		// Add eye to the sun
		var sun = Locator.GetAstroObject(AstroObject.Name.Sun);
		sun.gameObject.AddComponent<EyeRotationController>();

		// Add oxygen to all planets
		AddPlanetEffects(AstroObject.Name.TimberHearth, false, true, 400, 180);
		AddPlanetEffects(AstroObject.Name.BrittleHollow, true, false, 400, 0);
		AddPlanetEffects(AstroObject.Name.TowerTwin, true, true, 300, 30);
		AddPlanetEffects(AstroObject.Name.CaveTwin, true, true, 300, 30);
		AddPlanetEffects(AstroObject.Name.GiantsDeep, true, false, 1000, 0);
		AddPlanetEffects(AstroObject.Name.DarkBramble, true, false, 1000, 0);

		AddPlanetFog(AstroObject.Name.GiantsDeep, 9f, Color.black, 1000);
	}

    private static void AddPlanetEffects(AstroObject.Name planetName, bool oxygen, bool rain, float maxHeight, float surfaceHeight)
    {
		var config = new PlanetConfig()
		{
			Atmosphere = new AtmosphereModule()
			{
				hasOxygen = oxygen,
				hasRain = rain,
				size = maxHeight,
			}
		};

		var planet = Locator.GetAstroObject(planetName);
		AirBuilder.Make(planet.gameObject, planet.GetRootSector(), config);
		EffectsBuilder.Make(planet.gameObject, planet.GetRootSector(), config, surfaceHeight);
	}

	private static void AddPlanetFog(AstroObject.Name planetName, float fogDensity, Color fogColour, float size)
	{
		var atmosphere = new AtmosphereModule()
		{
			fogDensity = fogDensity,
			fogSize = size,
			fogTint = new MColor((int)(fogColour.r * 255), (int)(fogColour.g * 255), (int)(fogColour.b * 255))
		};

		var planet = Locator.GetAstroObject(planetName);
		FogBuilder.Make(planet.gameObject, planet.GetRootSector(), atmosphere);
	}
}
