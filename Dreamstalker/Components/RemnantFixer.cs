using UnityEngine;

namespace Dreamstalker.Components;

internal class RemnantFixer : MonoBehaviour
{
	private void Awake()
	{
		gameObject.transform.Find("StellarRemnant").gameObject.SetActive(true);
		var light = gameObject.transform.Find("StellarRemnant/Star/SunLight").GetComponent<Light>();
		light.range = 20000;
		light.intensity = 0.4f;
		this.enabled = false;
	}
}
