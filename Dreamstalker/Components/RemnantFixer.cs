using UnityEngine;

namespace Dreamstalker.Components;

internal class RemnantFixer : MonoBehaviour
{
	private void Awake()
	{
		gameObject.transform.Find("StellarRemnant").gameObject.SetActive(true);
		var light = gameObject.transform.Find("StellarRemnant/Star/Prefab_SunLight").GetComponent<Light>();
		light.range = 20000;
		light.intensity = 0.4f;
		Destroy(this);
	}
}
