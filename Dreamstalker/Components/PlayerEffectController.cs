using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Components;

internal class PlayerEffectController : MonoBehaviour
{
	private PlayerCameraEffectController _cameraEffectController;
	private LightFlickerController _lightFlickerController;
	private PlayerAudioController _playerAudioController; 
	public static PlayerEffectController Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		_cameraEffectController = GameObject.FindObjectOfType<PlayerCameraEffectController>();
		_lightFlickerController = GameObject.FindObjectOfType<LightFlickerController>();
		_playerAudioController = Locator.GetPlayerAudioController();

		_playerAudioController._repairToolSource.AssignAudioLibraryClip(AudioType.ToolScopeStatic);
	}

	public void Blink(float time)
	{
		_cameraEffectController.CloseEyes(time / 2f);
		_cameraEffectController.OpenEyes(time / 2f, false);
	}

	public void SetFlicker(float strength)
	{
		_lightFlickerController.Flicker(strength, strength == 0 ? 0 : float.MaxValue, 0.05f * strength, 0.1f * strength, 0.2f * strength, 0.3f * strength);
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
			_playerAudioController._repairToolSource.SetLocalVolume(strength);
			if (!_playerAudioController._repairToolSource.isPlaying)
			{
				_playerAudioController._repairToolSource.Play();
			}
		}
	}
}
