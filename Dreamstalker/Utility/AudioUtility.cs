﻿using UnityEngine;

namespace Dreamstalker.Utility;

internal static class AudioUtility
{
	public static OWAudioSource Make(GameObject parent, AudioType audio, OWAudioMixer.TrackName track, bool loop = true)
	{
		var go = new GameObject($"{audio}_AudioSource");
		go.transform.parent = parent.transform;
		go.transform.localPosition = Vector3.zero;
		go.SetActive(false);

		var audioSource = go.AddComponent<AudioSource>();
		var owAudioSource = go.AddComponent<OWAudioSource>();
		owAudioSource._audioSource = audioSource;
		owAudioSource.SetTrack(track);
		if (audio != AudioType.None)
		{
			owAudioSource._audioLibraryClip = audio;
		}
		owAudioSource.loop = loop;
		owAudioSource.playOnAwake = true;
		owAudioSource.spatialBlend = 1f;

		go.SetActive(true);

		return owAudioSource;
	}

	public static OWAudioSource MakeOneShot(GameObject parent, OWAudioMixer.TrackName track)
	{
		var go = new GameObject($"OneShot");
		go.transform.parent = parent.transform;
		go.transform.localPosition = Vector3.zero;
		go.SetActive(false);

		var audioSource = go.AddComponent<AudioSource>();
		var oneShotAudioSource = go.AddComponent<OWAudioSource>();
		oneShotAudioSource._audioSource = audioSource;
		oneShotAudioSource.spatialBlend = 1f;
		oneShotAudioSource.SetTrack(track);

		go.SetActive(true);

		return oneShotAudioSource;
	}

	public static void PlayOneShot(AudioType audio, float volume = 1f) =>
		Locator.GetPlayerAudioController()._oneShotSource.PlayOneShot(audio, volume);
}
