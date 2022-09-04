using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Components;

internal class DreamstalkerController : MonoBehaviour
{
	private DreamstalkerEffectsController _effects;

	private float _angularVelocity;
	private float _angularAcceleration;
	private Vector3 _velocity = Vector3.zero;

	public Transform RelativeTransform { private get; set; }

	public void Awake()
	{
		_effects = GetComponent<DreamstalkerEffectsController>();
	}

	private void TurnTowardsLocalDirection(Vector3 localDirection, float targetDegreesPerSecond)
	{
		var from = localDirection;
		from.y = 0f;
		localDirection.y = 0f;
		var num = OWMath.Angle(WorldToLocalDirection(transform.forward), localDirection, Up());
		var num2 = Mathf.Sign(num);
		var num3 = _angularVelocity * _angularVelocity / (2f * _angularAcceleration);
		var target = targetDegreesPerSecond * num2;
		if ((num2 > 0f && num <= num3) || (num2 < 0f && num >= -num3))
		{
			target = 0f;
		}
		_angularVelocity = Mathf.MoveTowards(_angularVelocity, target, _angularAcceleration * Time.fixedDeltaTime);
		var num4 = _angularVelocity * Time.fixedDeltaTime;
		if ((num2 > 0f && num4 > num) || (num2 < 0f && num4 < num))
		{
			num4 = num;
		}
		Quaternion localRotation = Quaternion.AngleAxis(num4, Up()) * transform.localRotation;
		transform.localRotation = localRotation;
	}

	public Vector3 GetRelativeVelocity() =>
		transform.InverseTransformDirection(LocalToWorldDirection(_velocity));

	public float GetAngularVelocity() => 
		_angularVelocity;

	public Vector3 WorldToLocalDirection(Vector3 worldDir) =>
		RelativeTransform.InverseTransformDirection(worldDir);

	public Vector3 LocalToWorldDirection(Vector3 localDir) =>
		RelativeTransform.TransformDirection(localDir);

	public Vector3 Up() =>
		(transform.position - RelativeTransform.position).normalized;

	public void FixedUpdate()
	{
		var targetLocalPosition = RelativeTransform.InverseTransformPoint(Locator.GetPlayerBody().transform.position);
		var localDirection = targetLocalPosition - transform.localPosition;

		TurnTowardsLocalDirection(localDirection, 90f);
	}
}
