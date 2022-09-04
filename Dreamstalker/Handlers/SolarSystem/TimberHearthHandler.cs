using Dreamstalker.Components;
using Dreamstalker.Utilities;
using UnityEngine;

namespace Dreamstalker.Handlers.SolarSystem;

internal class TimberHearthHandler : SolarSystemHandler
{
    protected override void OnSolarSystemAwake()
    {
        // Before NH can add the audio volume
        var th = GameObject.Find("TimberHearth_Body");
        foreach (var audio in th.GetComponentsInChildren<AudioVolume>()) audio.enabled = false;
    }

    protected override void OnSolarSystemStart()
    {
        Main.Log("Timber Hearth handler invoked.");

        var th = Locator.GetAstroObject(AstroObject.Name.TimberHearth);

        foreach (var light in th.GetComponentsInChildren<Light>())
        {
            var parent = light.transform.parent;
            if (parent != null)
            {
                if (parent.name.Contains("Props_HEA_BlueLantern"))
                {
                    parent.gameObject.SetActive(false);
                }
                else if (parent.name.Contains("WindowPivot_Cabin"))
                {
                    parent.gameObject.SetActive(false);
                }
            }
            light.color = new Color(0.4f, 1f, 1f);
        }
        foreach (var nightLight in th.GetComponentsInChildren<NightLight>())
        {
            nightLight.enabled = false;
        }

        Locator.GetShipBody().gameObject.SetActive(false);

        th.transform.Find("AmbientLight_TH").GetComponent<Light>().intensity = 0.6f;

        // Remove music
        th.GetComponentInChildren<VillageMusicVolume>().gameObject.SetActive(false);
    }
}
