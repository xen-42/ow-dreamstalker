using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Dreamstalker.Components;

internal class DreamstalkerEffectsController : MonoBehaviour
{
	private Animator _animator;
	private DreamstalkerController _controller;
	private OWAudioSource _oneShotAudioSource;

	private Vector2 _smoothedMoveSpeed = Vector2.zero;
	private float _smoothedTurnSpeed;
	private DampedSpring2D _moveSpeedSpring = new DampedSpring2D(50f, 1f);
	private DampedSpring _turnSpeedSpring = new DampedSpring(50f, 1f);

	private bool _moving;

	public void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<DreamstalkerController>();

		gameObject.AddComponent<AudioSource>().spatialBlend = 1f;
		_oneShotAudioSource = gameObject.AddComponent<OWAudioSource>();

		ToggleWalk(false, true);
	}

	public enum AnimationKeys
	{
		Default,
		Grab,
		SnapNeck,
		CallForHelp
	}

	public void PlayAnimation(AnimationKeys key)
	{
		switch (key)
		{
			case AnimationKeys.Default: _animator.SetTrigger(GhostEffects.AnimatorKeys.Trigger_Default); break;
			case AnimationKeys.Grab: _animator.SetTrigger(GhostEffects.AnimatorKeys.Trigger_Grab); break;
			case AnimationKeys.SnapNeck: _animator.SetTrigger(GhostEffects.AnimatorKeys.Trigger_SnapNeck); break;
			case AnimationKeys.CallForHelp: _animator.SetTrigger(GhostEffects.AnimatorKeys.Trigger_CallForHelp); break;
		};
	}

	public void ToggleWalk(bool move, bool forceUpdate = false)
	{
		if (!forceUpdate && move == _moving)
		{
			return;
		}

		if (move)
		{
			_animator.SetInteger(GhostEffects.AnimatorKeys.Int_MoveStyle, (int)GhostEffects.MovementStyle.Normal);
		}
		else
		{
			_animator.SetInteger(GhostEffects.AnimatorKeys.Int_MoveStyle, (int)GhostEffects.MovementStyle.Stalk);
		}

		_moving = move;
	}

	public void UpdateEffects()
	{
		Vector3 relativeVelocity = _controller.GetRelativeVelocity();
		float speed = 2f;

		Vector2 targetValue = new Vector2(relativeVelocity.x / speed, relativeVelocity.z / speed);
		_smoothedMoveSpeed = _moveSpeedSpring.Update(_smoothedMoveSpeed, targetValue, Time.deltaTime);
		_animator.SetFloat(GhostEffects.AnimatorKeys.Float_MoveDirectionX, _smoothedMoveSpeed.x);
		_animator.SetFloat(GhostEffects.AnimatorKeys.Float_MoveDirectionY, _smoothedMoveSpeed.y);
		_smoothedTurnSpeed = _turnSpeedSpring.Update(_smoothedTurnSpeed, _controller.GetAngularVelocity() / 90f, Time.deltaTime);
		_animator.SetFloat(GhostEffects.AnimatorKeys.Float_TurnSpeed, _smoothedTurnSpeed);

		ToggleWalk(relativeVelocity.ApproxEquals(Vector3.zero));
	}

	public void OnTeleport()
	{
		_oneShotAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
		_oneShotAudioSource.PlayOneShot(AudioType.Ghost_Identify_Curious, 1f);
	}
}
