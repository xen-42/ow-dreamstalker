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
	public float minFlicker = 0.0f;
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

	public void SetFlicker(float strength)
	{
		var flicker = strength * Mathf.Clamp(Mathf.PerlinNoise(Time.time * timeModifier, 0f), minFlicker, maxFlicker);
		_lightFlickerController._bubbleRenderer.material.SetAlpha(1f - Mathf.Clamp01(flicker));
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
			_playerAudioController._repairToolSource.SetLocalVolume(strength * strength * 4f);
			if (!_playerAudioController._repairToolSource.isPlaying)
			{
				_playerAudioController._repairToolSource.Play();
			}
		}
	}

	public void PlayOneShot(AudioType type) =>
		_playerAudioController.PlayOneShotInternal(type);
}
