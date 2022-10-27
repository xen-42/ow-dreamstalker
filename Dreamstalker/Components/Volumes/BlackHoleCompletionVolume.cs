using UnityEngine;

namespace Dreamstalker.Components.Volumes;

internal class BlackHoleCompletionVolume : CompletionVolume
{
	public override void Start()
	{
		base.Start();
		if (_streamingSphere != null)
		{
			_streamingSphere.radius = 100f;
		}
	}

	public override void OnTriggerEnter(Collider hitCollider)
	{
		if (!enabled)
		{
			if (hitCollider.attachedRigidbody == Locator.GetPlayerBody()._rigidbody)
			{
				// If not enabled, kill player when they enter the black hole
				Locator.GetDeathManager().KillPlayer(DeathType.Impact);
				PlayerEffectController.Instance.PlayOneShot(AudioType.BH_BlackHoleEmission);
			}
		}
		else
		{
			base.OnTriggerEnter(hitCollider);
		}
	}
}
