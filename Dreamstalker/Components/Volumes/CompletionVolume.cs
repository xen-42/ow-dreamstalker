using Dreamstalker.Components.Player;
using Dreamstalker.Components.Streaming;
using Dreamstalker.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Dreamstalker.Components.Volumes;

internal class CompletionVolume : MonoBehaviour
{
    public AstroObject.Name NextPlanet { set; private get; }

    private Campfire _campfire;

    public UnityEvent OnPlayerEnter = new();

    protected SphereShape _streamingSphere;

    public virtual void Start()
    {
		var streamingVolume = new GameObject("StreamingVolume");
		streamingVolume.transform.parent = transform;
		streamingVolume.transform.localPosition = Vector3.zero;
		streamingVolume.layer = LayerMask.NameToLayer("BasicEffectVolume");

        _streamingSphere = streamingVolume.AddComponent<SphereShape>();
		_streamingSphere.radius = 50f;

		streamingVolume.AddComponent<OWTriggerVolume>();
		var streamingController = streamingVolume.AddComponent<StreamingLoadVolume>();
		streamingController.SetSector(gameObject.GetAttachedOWRigidbody().GetComponent<AstroObject>().GetRootSector());
		streamingController.SetStreaming(StreamingGroups.Get(NextPlanet));
	}

    public void SetCampfire(Campfire campfire)
    {
        _campfire = campfire;

        campfire.OnCampfireStateChange += OnCampfireStateChange;
    }

    public void OnDestroy()
    {
        if (_campfire != null)
        {
            _campfire.OnCampfireStateChange -= OnCampfireStateChange;
        }
    }

    private void OnCampfireStateChange(Campfire campfire) =>
        enabled = campfire.GetState() == Campfire.State.LIT;

    public virtual void OnTriggerEnter(Collider hitCollider)
    {
        // This gets called even when disabled I guess that makes sense
        if (!enabled) return;

        if (hitCollider.attachedRigidbody == Locator.GetPlayerBody()._rigidbody)
        {
            Main.Log("Player entered a completion volume");
            OnPlayerEnter?.Invoke();
            PlayerEffectController.Instance.WakeUp();
            PlayerSpawnUtil.SpawnAt(NextPlanet);
        }
    }
}
