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
	private float _angularAcceleration = 360f;

	private Vector3 _velocity = Vector3.zero;
	private float _acceleration = 10f;

	private Vector3 _localTargetPosition = Vector3.zero;

	private Collider _playerCollider;
	private Collider _dreamstalkerCollider;

	public Transform RelativeTransform { private get; set; }

	public void Awake()
	{
		_effects = GetComponent<DreamstalkerEffectsController>();

		_dreamstalkerCollider = gameObject.AddComponent<CapsuleCollider>();
		gameObject.AddComponent<OWCollider>();
	}

	public void Start()
	{
		_playerCollider = Locator.GetPlayerCollider();
	}

	private void TurnTowardsLocalDirection(Vector3 localDirection, float targetDegreesPerSecond)
	{
		var up = LocalUp();

		var from = Vector3.ProjectOnPlane(WorldToLocalDirection(transform.forward), up);
		var to = Vector3.ProjectOnPlane(localDirection, up);
		var angleDiff = OWMath.Angle(from, to, up);

		var direction = Mathf.Sign(angleDiff);
		var deltaAngle = _angularVelocity * _angularVelocity / (2f * _angularAcceleration);
		var target = targetDegreesPerSecond * direction;
		if ((direction > 0f && angleDiff <= deltaAngle) || (direction < 0f && angleDiff >= -deltaAngle))
		{
			target = 0f;
		}
		_angularVelocity = Mathf.MoveTowards(_angularVelocity, target, _angularAcceleration * Time.fixedDeltaTime);
		var angleToRotate = _angularVelocity * Time.fixedDeltaTime;
		if ((direction > 0f && angleToRotate > angleDiff) || (direction < 0f && angleToRotate < angleDiff))
		{
			angleToRotate = angleDiff;
		}
		Quaternion localRotation = Quaternion.AngleAxis(angleToRotate, up) * transform.localRotation;
		transform.localRotation = localRotation;

		transform.LookAt(transform.position + Vector3.ProjectOnPlane(transform.forward, GlobalUp()), GlobalUp());
	}

	public Vector3 GetRelativeVelocity() =>
		transform.InverseTransformDirection(LocalToWorldDirection(_velocity));

	public float GetAngularVelocity() => 
		_angularVelocity;

	public Vector3 WorldToLocalDirection(Vector3 worldDir) =>
		RelativeTransform.InverseTransformDirection(worldDir);

	public Vector3 LocalToWorldDirection(Vector3 localDir) =>
		RelativeTransform.TransformDirection(localDir);

	public Vector3 WorldToLocalPosition(Vector3 worldPos) =>
		RelativeTransform.InverseTransformPoint(worldPos);

	public Quaternion WorldToLocalRotation(Quaternion worldRot) =>
		RelativeTransform.InverseTransformRotation(worldRot);

	public Vector3 LocalUp() =>
		WorldToLocalDirection(GlobalUp());

	public Vector3 GlobalUp() =>
		(transform.position - RelativeTransform.position).normalized;

	private void UpdatePositionFromVelocity()
	{
		Vector3 newPos = transform.localPosition + _velocity * Time.fixedDeltaTime;

		var localPlayerPos = WorldToLocalPosition(Locator.GetPlayerTransform().position);
		var localPlayerRot = WorldToLocalRotation(Locator.GetPlayerTransform().rotation);

		Vector3 direction;
		float distance;
		if (Physics.ComputePenetration(_dreamstalkerCollider, newPos, transform.localRotation, _playerCollider, localPlayerPos, localPlayerRot, out direction, out distance))
		{
			newPos += direction * distance;
		}

		transform.localPosition = newPos;

		/*
		// Stick to the ground
		var origin = RelativeTransform.position + GlobalUp() * 500f;
		var downwards = -GlobalUp();
		if (Physics.Raycast(origin, downwards, out var hitInfo, 500f, OWLayerMask.physicalMask))
		{
			transform.position = hitInfo.point;
		}
		*/
	}

	private void TeleportNearPlayer()
	{
		_effects.OnTeleport();

		var playerLocalPosition = WorldToLocalPosition(_playerCollider.transform.position);
		var planeVector = Quaternion.FromToRotation(Vector3.up, playerLocalPosition.normalized) * Vector3.left;

		var playerRelativePos = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), playerLocalPosition.normalized) * planeVector;

		transform.localPosition = playerLocalPosition + playerRelativePos * 30f;
	}

	private void StalkPlayer()
	{
		var displacement = (_localTargetPosition - transform.localPosition);
		var direction = displacement.normalized;
		var distance = displacement.magnitude;

		if (distance > 50f)
		{
			TeleportNearPlayer();
			return;
		}

		float speed = _velocity.magnitude;
		float deltaPos = speed * speed / (2f * _acceleration);

		if (distance > deltaPos)
		{
			var target = direction * 2f;
			_velocity = Vector3.MoveTowards(_velocity, target, _acceleration * Time.fixedDeltaTime);
		}
		else
		{
			_velocity = Vector3.MoveTowards(_velocity, Vector3.zero, _acceleration * Time.fixedDeltaTime);
		}

		UpdatePositionFromVelocity();
	}

	public void FixedUpdate()
	{
		_localTargetPosition = RelativeTransform.InverseTransformPoint(Locator.GetPlayerBody().transform.position);
		var localDisplacement = _localTargetPosition - transform.localPosition;

		var localDirection = localDisplacement.normalized;
		var distance = localDisplacement.magnitude;

		TurnTowardsLocalDirection(localDirection, 90f);

		// Move towards player
		StalkPlayer();

		var flickerIntensity = Mathf.Clamp01(1f - (distance / 30f));
		PlayerEffectController.Instance.SetFlicker(flickerIntensity);
		PlayerEffectController.Instance.SetStatic(flickerIntensity);

		_effects.UpdateEffects();
	}
}
