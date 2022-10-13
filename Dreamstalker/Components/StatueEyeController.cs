using UnityEngine;

namespace Dreamstalker.Components;

internal class StatueEyeController : SectoredMonoBehaviour
{
	public float radius = 15f;
	public float glowSpeed = 2f;

	public Campfire campfire;

	private Transform _player;
	private float _currentGlow = 0f;

	private MemoryUplinkTrigger _statue;
	private OWAudioSource _audioSource;

	public override void Awake()
	{
		base.Awake();

		_statue = GetComponent<MemoryUplinkTrigger>();

		_audioSource = gameObject.AddComponent<OWAudioSource>();
	}

	public void Start()
	{
		_player = Locator.GetPlayerTransform();

		foreach (var animator in _statue._lowerLidAnimators)
		{
			animator.RotateToOriginalLocalRotation(1f);
		}
		foreach (var animator in _statue._upperLidAnimators)
		{
			animator.RotateToOriginalLocalRotation(1f);
		}

		_audioSource.SetTrack(OWAudioMixer.TrackName.Environment);
		_audioSource.AssignAudioLibraryClip(AudioType.Flashback_Overlay_1_LP);
		_audioSource.minDistance = 10f;
		_audioSource.loop = true;
		_audioSource.spatialBlend = 1f;
		_audioSource.Play();
	}

	public void Update()
	{
		if (Main.DebugMode || campfire?.GetState() == Campfire.State.LIT)
		{
			var eyesGlow = (transform.position - _player.transform.position).sqrMagnitude < radius * radius;

			var delta = glowSpeed * Time.deltaTime;
			_currentGlow = Mathf.Clamp01(_currentGlow + (eyesGlow ? delta : -delta));

			_statue.SetEyeGlow(_currentGlow);
			_audioSource.SetLocalVolume(_currentGlow);
		}
		else
		{
			_statue.SetEyeGlow(0f);
			_audioSource.SetLocalVolume(0f);
		}
	}
}
