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

	private Vector2 _smoothedMoveSpeed = Vector2.zero;
	private float _smoothedTurnSpeed;
	private DampedSpring2D _moveSpeedSpring = new DampedSpring2D(50f, 1f);
	private DampedSpring _turnSpeedSpring = new DampedSpring(50f, 1f);

	public void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<DreamstalkerController>();

		_animator.SetInteger(GhostEffects.AnimatorKeys.Int_MoveStyle, (int)GhostEffects.MovementStyle.Stalk);
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

	public void Update_Effects()
	{
		Vector3 relativeVelocity = this._controller.GetRelativeVelocity();
		float num = 2f;
		Vector2 targetValue = new Vector2(relativeVelocity.x / num, relativeVelocity.z / num);
		this._smoothedMoveSpeed = this._moveSpeedSpring.Update(this._smoothedMoveSpeed, targetValue, Time.deltaTime);
		this._animator.SetFloat(GhostEffects.AnimatorKeys.Float_MoveDirectionX, this._smoothedMoveSpeed.x);
		this._animator.SetFloat(GhostEffects.AnimatorKeys.Float_MoveDirectionY, this._smoothedMoveSpeed.y);
		this._smoothedTurnSpeed = this._turnSpeedSpring.Update(this._smoothedTurnSpeed, this._controller.GetAngularVelocity() / 90f, Time.deltaTime);
		this._animator.SetFloat(GhostEffects.AnimatorKeys.Float_TurnSpeed, this._smoothedTurnSpeed);
	}
}
