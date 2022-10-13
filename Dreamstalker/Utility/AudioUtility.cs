using UnityEngine;

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
			owAudioSource.AssignAudioLibraryClip(audio);
		}
		owAudioSource.loop = loop;
		owAudioSource.playOnAwake = true;
		owAudioSource.spatialBlend = 1f;

		go.SetActive(true);

		return owAudioSource;
	}

	public static void PlayOneShot(AudioType audio, float volume = 1f) =>
		Locator.GetPlayerAudioController()._repairToolSource.PlayOneShot(audio, volume);
}
