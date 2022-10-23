using UnityEngine;

namespace Dreamstalker.Components.AncientGlade;

internal class WrapAround : SectoredMonoBehaviour
{
	private Transform _focus;
	private OWRigidbody _playerBody;

	private Vector3 _axis;

	public float maxDistance = 20f;

	public void Start()
	{
		_playerBody = Locator.GetPlayerBody();
	}

	public void SetFocus(Transform focus)
	{
		_focus = focus;
		_axis = (transform.position - _focus.position).normalized; // World space
	}

	public void Update()
	{
		var displacement = _playerBody.transform.position - _focus.position;

		if (displacement.sqrMagnitude > maxDistance * maxDistance)
		{
			var newDisplacement = Quaternion.AngleAxis(180f, _axis) * displacement;

			var finalPosition = _focus.position + (newDisplacement * 0.8f);

			var worldUp = (finalPosition - transform.position).normalized;
			var forward = -newDisplacement.normalized;
			var finalRotation = Quaternion.LookRotation(forward, worldUp);

			_playerBody.SetVelocity(Vector3.zero);
			_playerBody.WarpToPositionRotation(finalPosition, finalRotation);

			if (!Physics.autoSyncTransforms)
			{
				Physics.SyncTransforms();
			}

			PlayerEffectController.Instance.Flicker();
		}
	}
}
