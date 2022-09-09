using Dreamstalker.Components;
using NewHorizons.Builder.Atmosphere;
using NewHorizons.External.Configs;
using NewHorizons.External.Modules;

namespace Dreamstalker.Handlers.SolarSystem;

internal class PlanetHandler : SolarSystemHandler
{
    protected override void OnSolarSystemAwake() { }

    protected override void OnSolarSystemStart()
    {
        // Fix the remnant on the sun, doesn't immediately show if lifespan is 0
        Locator.GetAstroObject(AstroObject.Name.Sun).gameObject.AddComponent<RemnantFixer>();

		// Add oxygen to all planets
		AddPlanetEffects(AstroObject.Name.TimberHearth, false, true, 400, 180);
		AddPlanetEffects(AstroObject.Name.BrittleHollow, true, false, 400, 0);
		AddPlanetEffects(AstroObject.Name.TowerTwin, true, true, 300, 30);
		AddPlanetEffects(AstroObject.Name.CaveTwin, true, true, 300, 30);
		AddPlanetEffects(AstroObject.Name.GiantsDeep, true, false, 1000, 0);
    }

    private static void AddPlanetEffects(AstroObject.Name planetName, bool oxygen, bool rain, float maxHeight, float surfaceHeight)
    {
		var config = new PlanetConfig()
		{
			Atmosphere = new AtmosphereModule()
			{
				hasOxygen = oxygen,
				hasRain = rain,
				size = maxHeight
			}
		};
		var planet = Locator.GetAstroObject(planetName);
		AirBuilder.Make(planet.gameObject, planet.GetRootSector(), config);
		EffectsBuilder.Make(planet.gameObject, planet.GetRootSector(), config, surfaceHeight);
	}
}
