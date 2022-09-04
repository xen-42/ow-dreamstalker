using NewHorizons.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Utilities;

internal static class CampfireTinter
{
	public static void Tint(Campfire campfire)
	{
		try
		{
			if (campfire is DreamCampfire)
			{
				return;
			}

			foreach (var light in campfire._lightController._lights)
			{
				light.gameObject.GetComponent<Light>().color = new Color(0.1f, 0.4f, 0.1f);
			}

			var emberTexture = ImageUtilities.GetTexture(Main.Instance, "assets/Props_HEA_CampfireEmbers_d.png");
			var emberMaterial = campfire.transform.parent.Find("Props_HEA_Campfire/Campfire_Embers").GetComponent<MeshRenderer>().material;
			emberMaterial.SetTexture("_MainTex", emberTexture);
			emberMaterial.SetTexture("_EmissionMap", emberTexture);

			var ashTexture = ImageUtilities.GetTexture(Main.Instance, "assets/Props_HEA_CampfireAsh_e.png");
			var ashMaterial = campfire.transform.parent.Find("Props_HEA_Campfire/Campfire_Ash").GetComponent<MeshRenderer>().material;
			ashMaterial.SetTexture("_EmissionMap", ashTexture);

			campfire._flames.material.color = new Color(0f, 1f, 0f);
		}
		catch(Exception e)
		{
			Main.LogError($"Could not tint campfire {campfire.transform.GetPath()} : {e}");
		}
	}

	public static void TintMarshmallow()
	{
		GameObject.FindObjectOfType<Marshmallow>().GetComponent<MeshRenderer>().material.color = new Color(0f, 1f, 0f);
	}
}
