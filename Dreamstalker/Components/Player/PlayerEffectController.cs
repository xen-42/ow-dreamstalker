using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Dreamstalker.Components;

internal class PlayerEffectController : MonoBehaviour
{
	private PlayerCameraEffectController _cameraEffectController;
	private LightFlickerController _lightFlickerController;
	private PlayerAudioController _playerAudioController;

	public float timeModifier = 1.0f;
	public float maxFlicker = 0.5f;

	public static PlayerEffectController Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		_cameraEffectController = GameObject.FindObjectOfType<PlayerCameraEffectController>();
		_lightFlickerController = Locator.GetPlayerBody().gameObject.GetComponentInChildren<LightFlickerController>();
		_playerAudioController = Locator.GetPlayerAudioController();

		_playerAudioController._repairToolSource.AssignAudioLibraryClip(AudioType.ToolScopeStatic);
	}

	public void Blink(float time = 2f)
	{
		_cameraEffectController.CloseEyes(time / 2f);
		_cameraEffectController.OpenEyes(time / 2f, false);
	}

	public void WakeUp()
	{
		_cameraEffectController.CloseEyes(1f);
		_cameraEffectController.OpenEyes(_cameraEffectController._wakeLength, _cameraEffectController._calmWakeCurve);
		_playerAudioController._oneShotSleepingAtCampfireSource.PlayOneShot(AudioType.PlayerGasp_Light, 1f);
	}

	public void SetFlicker(float strength)
	{
		// Disable flickering in debug mode so we can actually see the creature
		if (Main.DebugMode) return;

		if (!_lightFlickerController.IsFlickering())
		{
			var flicker = 0.8f * Mathf.PerlinNoise(Time.time * 4, 0f) + 0.2f * Mathf.PerlinNoise(0f, Time.time + 100000f);
			var flickerScale = 1.5f * strength * Mathf.Clamp01(flicker * flicker);
			_lightFlickerController._bubbleRenderer.material.SetAlpha(Mathf.Clamp(flickerScale, 0f, 0.9f));
			_lightFlickerController._bubbleRenderer.enabled = true;
		}
	}

	public void SetStatic(float strength)
	{
		if (strength == 0)
		{
			if (_playerAudioController._repairToolSource.isPlaying)
			{
				_playerAudioController._repairToolSource.Stop();
			}
		}
		else
		{
			_playerAudioController._repairToolSource.SetLocalVolume(strength * strength * 10f);
			if (!_playerAudioController._repairToolSource.isPlaying)
			{
				_playerAudioController._repairToolSource.Play();
			}
		}
	}

	public void PlayOneShot(AudioType type) =>
		_playerAudioController.PlayOneShotInternal(type);
}
