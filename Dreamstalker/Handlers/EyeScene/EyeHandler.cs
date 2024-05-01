using NewHorizons.Utility;
using System;
using UnityEngine;

namespace Dreamstalker.Handlers.EyeScene;

internal class EyeHandler : BaseHandler
{
	public static GameObject LightningPrefab { get; private set; }
	public static GameObject EyePrefab { get; private set; }
	public static GameObject InflationPrefab { get; private set; }
	public static GameObject QuantumCampfirePrefab { get; private set; }
	public static GameObject MiniGalaxyPrefab { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		_main.EyeSceneAwake.AddListener(OnEyeSceneAwake);
	}

	protected void OnDestroy()
	{
		if (_main != null)
		{
			_main.EyeSceneAwake.RemoveListener(OnEyeSceneAwake);
		}
	}

	private void OnEyeSceneAwake()
	{
		try
		{
			// Don't want to see that we went to the eye
			GameObject.Find("Player_Body").GetComponentInChildren<PlayerCameraEffectController>().CloseEyes(0f);
			foreach (var audio in GameObject.FindObjectsOfType<AudioSource>())
			{
				audio.enabled = false;
			}
			GUIMode.SetRenderMode(GUIMode.RenderMode.Hidden);

			// Grab prefabs
			LightningPrefab = GameObject.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/SixthPlanet_Root/Sector_EyeSurface/QuantumLightningObjects/Pivot/Prefab_EYE_QuantumLightningObject").InstantiateInactive();
			InflationPrefab = GameObject.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/InflationController").InstantiateInactive();
			QuantumCampfirePrefab = GameObject.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/Sector_Campfire/QuantumCampfire").InstantiateInactive();
			QuantumCampfirePrefab.transform.rotation = Quaternion.identity;
			MiniGalaxyPrefab = GameObject.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/ForestOfGalaxies_Root/Sector_ForestOfGalaxies/MiniGalaxyController/Prefab_EYE_MiniGalaxy(Clone)").InstantiateInactive();

			// Make eye prefab
			var eye = SearchUtilities.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/SixthPlanet_Root/Proxy_SixthPlanet").InstantiateInactive();
			var eyeClouds = SearchUtilities.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/SixthPlanet_Root/Effects_SixthPlanet/Effects_Eye_CloudLayer/CloudLayer").InstantiateInactive();
			var eyeLightning = SearchUtilities.Find("EyeOfTheUniverse_Body/Sector_EyeOfTheUniverse/SixthPlanet_Root/Effects_SixthPlanet/LightningGenerator_EYE_Primary").InstantiateInactive();

			EyePrefab = new GameObject("Eye");
			eye.transform.parent = EyePrefab.transform;
			eyeClouds.transform.parent = EyePrefab.transform;
			eyeLightning.transform.parent = EyePrefab.transform;

			eye.transform.localPosition = Vector3.zero;
			eye.transform.localRotation = Quaternion.identity;

			eyeClouds.transform.localPosition = Vector3.zero;
			eyeClouds.transform.localRotation = Quaternion.identity;

			eyeLightning.transform.localRotation = Quaternion.identity;
			eyeLightning.transform.localPosition = new Vector3(0f, -250f, 0f);

			EyePrefab.transform.rotation = Quaternion.Euler(0, 174, 0);

			SetLayersRecursively(EyePrefab.transform, LayerMask.NameToLayer("Sun"));

			eyeClouds.layer = LayerMask.NameToLayer("IgnoreSun");

			EyePrefab.SetActive(false);
			eye.SetActive(true);
			eyeClouds.SetActive(true);
			eyeLightning.SetActive(true);

			// Dont destroy on load
			DontDestroyOnLoad(LightningPrefab);
			DontDestroyOnLoad(InflationPrefab);
			DontDestroyOnLoad(QuantumCampfirePrefab);
			DontDestroyOnLoad(MiniGalaxyPrefab);
			DontDestroyOnLoad(EyePrefab);
		}
		catch (Exception e)
		{
			Main.LogError($"Something went wrong when loading eye props. Going to the solar system without them. {e}");
		}
		finally
		{
			Main.Instance.NewHorizonsAPI.ChangeCurrentStarSystem("SolarSystem");
		}
	}

	private void SetLayersRecursively(Transform transform, int layer)
	{
		transform.gameObject.layer = layer;

		foreach (Transform t in transform)
		{
			t.gameObject.layer = layer;
			if (t.childCount != 0)
			{
				SetLayersRecursively(t, layer);
			}
		}
	}
}
