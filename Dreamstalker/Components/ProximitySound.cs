using Dreamstalker.Utility;
using UnityEngine;

namespace Dreamstalker.Components;

internal class ProximitySound : MonoBehaviour
{
	public AudioType audio;
	public Campfire linkedCampfire;
	public float radius = 15f;

	private OWAudioSource _audioSource;
	private Transform _player;
	private float _currentVolume;

	public void Start()
	{
		_audioSource = AudioUtility.Make(gameObject, audio, OWAudioMixer.TrackName.Environment, true);
		_audioSource.spatialBlend = 0f;

		_currentVolume = 0f;
		_audioSource.SetLocalVolume(0f);

		_player = Locator.GetPlayerTransform();
	}


	public void Update()
	{
		if (Main.DebugMode || linkedCampfire == null || linkedCampfire.GetState() == Campfire.State.LIT)
		{
			var inRange = (transform.position - _player.transform.position).sqrMagnitude < radius * radius;

			var delta = 2f * Time.deltaTime;
			_currentVolume = Mathf.Clamp01(_currentVolume + (inRange ? delta : -delta));
		}
		else
		{
			_currentVolume = 0f;
		}

		_audioSource.SetLocalVolume(_currentVolume);
	}
}
