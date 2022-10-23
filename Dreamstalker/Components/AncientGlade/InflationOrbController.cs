using Dreamstalker.External;
using Dreamstalker.Handlers.Credits;
using Dreamstalker.Utility;
using System;
using UnityEngine;

namespace Dreamstalker.Components.AncientGlade;

internal class InflationOrbController : MonoBehaviour 
{
	private Light _inflationLight, _possibilityLight;
	private float timer;
	private OWAudioSource _sfx;
	public Campfire campfire;
	private GameObject _volume;
	private OWTriggerVolume _triggerVolume;

	private AudioVolume[] _audioVolumes;

	private bool _hasMutedAudioVolumes;

	private bool fading;

	private float fadeStartTime, loadLength;

	public void Awake()
	{
		_inflationLight = gameObject.transform.Find("PossibilitySphereRoot/InflationLight").GetComponent<Light>();
		_possibilityLight = gameObject.transform.Find("PossibilitySphereRoot/Effects_EYE_PossibilityParticles2/PossibilityLight").GetComponent<Light>();
		_sfx = gameObject.transform.Find("SFXAudioSource").GetComponent<OWAudioSource>();
		_sfx._audioSource.enabled = true;

		_volume = new ("EndVolume")
		{
			layer = LayerMask.NameToLayer("BasicEffectVolume")
		};
		_volume.AddComponent<SphereShape>().radius = 3f;
		_triggerVolume = _volume.AddComponent<OWTriggerVolume>();
		_triggerVolume.OnEntry += TriggerVolume_OnEntry;
		_volume.transform.parent = gameObject.transform.Find("PossibilitySphereRoot");
		_volume.transform.localPosition = Vector3.zero;

		_audioVolumes = gameObject.GetAttachedOWRigidbody().GetComponentsInChildren<AudioVolume>();
	}

	public void OnDestroy()
	{
		if (_triggerVolume != null)
		{
			_triggerVolume.OnEntry -= TriggerVolume_OnEntry;
		}
	}

	public void Update()
	{
		if (fading)
		{
			// Fade to white then to black
			var fadeToWhiteLength = 1f;
			var num = Time.unscaledTime - fadeStartTime;

			if (num < fadeToWhiteLength)
			{
				LoadManager.s_instance._fadeImage.color = new Color(1f, 1f, 1f, Mathf.Clamp01(num / fadeToWhiteLength));
			}
			else
			{
				var num2 = 1f - Mathf.Clamp01((Time.unscaledTime - fadeStartTime - fadeToWhiteLength) / (loadLength - fadeToWhiteLength));
				LoadManager.s_instance._fadeImage.color = new Color(num2, num2, num2, 1f);
			}

			if (Time.unscaledTime > fadeStartTime + loadLength)
			{
				LoadManager.LoadSceneImmediate(OWScene.Credits_Fast);
			}
		}
		else
		{
			if (timer < Time.time)
			{
				timer = Time.time + 1f;
				_sfx.PlayOneShot(AudioType.EyeCosmicInflation, 1f);

				// Mute the background noise after the first whoooosh
				if (!_hasMutedAudioVolumes)
				{
					foreach (var audioVolume in _audioVolumes)
					{
						audioVolume._owAudioSrc.SetLocalVolume(0f);
					}
					_hasMutedAudioVolumes = true;
				}
			}
			var intensity = Mathf.Cos(20f * Time.time / Mathf.PI) / 2f + 0.5f;
			_inflationLight.intensity = intensity;
			_possibilityLight.intensity = intensity;
			if (campfire != null)
			{
				campfire._lightController._intensity = intensity;
			}
		}
	}

	private void TriggerVolume_OnEntry(GameObject hitObj)
	{
		if (fading) return;

		// Reset save
		PlayerSpawnUtil.LastSpawn = AstroObject.Name.TimberHearth;
		DreamstalkerData.Save();

		AudioUtility.PlayOneShot(AudioType.Death_BigBang, 1f);

		// If we actually freeze time the camera freaks out for some reason
		Time.timeScale = 0.01f;
		OWInput.ChangeInputMode(InputMode.None);

		fading = true;

		LoadManager.s_instance._fadeCanvas.enabled = true;
		LoadManager.s_instance._fadeImage.color = Color.clear;
		LoadManager.s_instance._fadeColor = Color.white;

		loadLength = 5f;
		fadeStartTime = Time.unscaledTime;

		GUIMode.SetRenderMode(GUIMode.RenderMode.Hidden);

		DreamstalkerCreditsHandler.flagWon = true;
	}
}
