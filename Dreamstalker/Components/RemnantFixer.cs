using UnityEngine;

namespace Dreamstalker.Components;

internal class RemnantFixer : MonoBehaviour
{
	private void Awake()
	{
		gameObject.transform.Find("StellarRemnant").gameObject.SetActive(true);
		gameObject.transform.Find("StellarRemnant/Star/SunLight").GetComponent<Light>().range = 20000;
		this.enabled = false;
	}
}
