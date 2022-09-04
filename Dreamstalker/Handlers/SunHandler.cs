using Dreamstalker.Components;

namespace Dreamstalker.Handlers;

internal class SunHandler : SolarSystemHandler
{
	protected override void OnSolarSystemAwake() { }

	protected override void OnSolarSystemStart()
	{
		Locator.GetAstroObject(AstroObject.Name.Sun).gameObject.AddComponent<RemnantFixer>();
	}
}
