using UnityEngine;

namespace Dreamstalker.Components.Streaming;

internal class StreamingLoadVolume : SectoredMonoBehaviour
{
	private StreamingGroup _streamingGroup;
	private OWTriggerVolume _streamingVolume;
	protected bool _playerInVolume;
	private bool _preloadingAssets;

	public void SetStreaming(StreamingGroup streamingGroup) => _streamingGroup = streamingGroup;

	protected virtual bool ShouldLoad => true;

	public override void Awake()
	{
		base.Awake();
		_streamingVolume = GetComponent<OWTriggerVolume>();

		_streamingVolume.OnEntry += OnEntry;
		_streamingVolume.OnExit += OnExit;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		_streamingVolume.OnEntry -= OnEntry;
		_streamingVolume.OnExit -= OnExit;
	}

	private void OnEntry(GameObject hitObj)
	{
		OWRigidbody attachedOWRigidbody = hitObj.GetAttachedOWRigidbody(false);
		if (attachedOWRigidbody != null && attachedOWRigidbody.CompareTag("Player"))
		{
			_playerInVolume = true;
		}
	}

	private void OnExit(GameObject hitObj)
	{
		OWRigidbody attachedOWRigidbody = hitObj.GetAttachedOWRigidbody(false);
		if (attachedOWRigidbody != null && attachedOWRigidbody.CompareTag("Player"))
		{
			_playerInVolume = false;
		}
	}

	private void FixedUpdate()
	{
		if (_streamingGroup != null)
		{
			bool shouldBeLoadingAssets = _playerInVolume && ShouldLoad;
			UpdatePreloadingState(shouldBeLoadingAssets);
		}
	}

	private void UpdatePreloadingState(bool shouldBeLoadingAssets)
	{
		if (!_preloadingAssets && shouldBeLoadingAssets)
		{
			Main.Log($"Loading assets for {_streamingGroup.transform.parent.name}");
			_streamingGroup.RequestRequiredAssets(0);
			_streamingGroup.RequestGeneralAssets(0);
			_preloadingAssets = true;
			return;
		}
		if (_preloadingAssets && !shouldBeLoadingAssets)
		{
			_streamingGroup.ReleaseRequiredAssets();
			_streamingGroup.ReleaseGeneralAssets();
			_preloadingAssets = false;
		}
	}
}
