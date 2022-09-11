using Dreamstalker.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Dreamstalker.Components.Volumes;

internal class CompletionVolume : MonoBehaviour
{
    public AstroObject.Name NextPlanet { set; private get; }

    private Campfire _campfire;

    public UnityEvent OnPlayerEnter = new();

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

        if (hitCollider.attachedRigidbody == Locator.GetPlayerBody())
        {
            OnPlayerEnter?.Invoke();
            PlayerEffectController.Instance.WakeUp();
            PlayerSpawnUtil.SpawnAt(NextPlanet);
        }
    }
}
