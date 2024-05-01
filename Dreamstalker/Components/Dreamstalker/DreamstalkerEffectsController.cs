using Dreamstalker.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Dreamstalker.Components.Dreamstalker;

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

	public UnityEvent SnapNeck = new();
	public UnityEvent LiftPlayer = new();
	public UnityEvent CallForHelpComplete = new();

	private float _callForHelpTime;

	private float _footstepTimer;
	private OWAudioSource _footstepAudio;
	private OWAudioSource _fearMusic;
	private OWAudioSource _suspenseMusic;

	private bool _musicPlaying = false;

	public void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<DreamstalkerController>();

		gameObject.AddComponent<AudioSource>().spatialBlend = 1f;
		_oneShotAudioSource = gameObject.AddComponent<OWAudioSource>();
		_oneShotAudioSource.SetTrack(OWAudioMixer.TrackName.Environment);

		_fearMusic = AudioUtility.Make(gameObject, AudioType.GhostSequence_Fear, OWAudioMixer.TrackName.Music);
		_fearMusic.SetLocalVolume(0.5f);
		_fearMusic.Stop();
		_fearMusic.spatialBlend = 0f;

		_suspenseMusic = AudioUtility.Make(gameObject, AudioType.GhostSequence_Suspense, OWAudioMixer.TrackName.Music);
		_suspenseMusic.SetLocalVolume(0.5f);
		_suspenseMusic.Stop();
		_suspenseMusic.spatialBlend = 0f;

		_footstepAudio = AudioUtility.MakeOneShot(gameObject, OWAudioMixer.TrackName.Environment);
		_footstepAudio.minDistance = 5f;

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
			case AnimationKeys.Default:
				_animator.SetTrigger(GhostEffects.AnimatorKeys.Trigger_Default);
				break;
			case AnimationKeys.Grab:
				_animator.SetTrigger(GhostEffects.AnimatorKeys.Trigger_Grab);
				break;
			case AnimationKeys.SnapNeck:
				_animator.SetTrigger(GhostEffects.AnimatorKeys.Trigger_SnapNeck);
				break;
			case AnimationKeys.CallForHelp:
				_animator.SetTrigger(GhostEffects.AnimatorKeys.Trigger_CallForHelp);
				_callForHelpTime = Time.time + 2f;
				break;
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

	public void UpdateEffects(float distance)
	{
		var relativeVelocity = _controller.GetRelativeVelocity();
		var speed = 2f;

		var targetValue = new Vector2(relativeVelocity.x / speed, relativeVelocity.z / speed);
		_smoothedMoveSpeed = _moveSpeedSpring.Update(_smoothedMoveSpeed, targetValue, Time.deltaTime);
		_animator.SetFloat(GhostEffects.AnimatorKeys.Float_MoveDirectionX, _smoothedMoveSpeed.x);
		_animator.SetFloat(GhostEffects.AnimatorKeys.Float_MoveDirectionY, _smoothedMoveSpeed.y);
		_smoothedTurnSpeed = _turnSpeedSpring.Update(_smoothedTurnSpeed, _controller.GetAngularVelocity() / 90f, Time.deltaTime);
		_animator.SetFloat(GhostEffects.AnimatorKeys.Float_TurnSpeed, _smoothedTurnSpeed);

		ToggleWalk(!relativeVelocity.ApproxEquals(Vector3.zero));

		if (_callForHelpTime != 0f && _callForHelpTime < Time.time)
		{
			CallForHelpComplete?.Invoke();
			_callForHelpTime = 0f;
			_musicPlaying = true;
		}

		var flickerIntensity = Mathf.Clamp01(1f - distance / 20f);
		PlayerEffectController.Instance.SetStatic(flickerIntensity * 4f);
		PlayerEffectController.Instance.SetFlicker(_controller.IsVisible() ? 6 * flickerIntensity * _controller.LineOfSightFraction() : flickerIntensity);

		if (_moving)
		{
			_footstepTimer -= Time.deltaTime;
			if (_footstepTimer <= 0f)
			{
				_footstepTimer = 1.2f;
				_footstepAudio.PlayOneShot(AudioType.Ghost_Footstep_Forest_Running, 0.5f);
			}
		}

		// Idk why this happens
		if (_suspenseMusic.isPlaying && !_suspenseMusic.IsFadingIn() && _suspenseMusic._localVolume == 0f)
		{
			_suspenseMusic.Stop();
		}

		if (_fearMusic.isPlaying && !_fearMusic.IsFadingIn() && _fearMusic._localVolume == 0f)
		{
			_fearMusic.Stop();
		}

		if (distance < 8f)
		{
			if (_suspenseMusic.isPlaying && !_suspenseMusic.IsFadingOut())
			{
				_suspenseMusic.FadeOut(0.5f, OWAudioSource.FadeOutCompleteAction.STOP);
			}

			if (!_fearMusic.isPlaying && _musicPlaying)
			{
				_fearMusic.SetLocalVolume(0f);
				_fearMusic.FadeIn(0.5f, true, true, 0.5f);
			}
		}

		if (distance > 10f || !_fearMusic.isPlaying)
		{
			if (_fearMusic.isPlaying && !_fearMusic.IsFadingOut())
			{
				_fearMusic.FadeOut(0.5f, OWAudioSource.FadeOutCompleteAction.STOP);
			}

			if (!_suspenseMusic.isPlaying && _musicPlaying)
			{
				_suspenseMusic.SetLocalVolume(0f);
				_suspenseMusic.FadeIn(0.5f, true, true, 0.5f);
			}
		}
	}

	public void OnDespawn()
	{
		PlayerEffectController.Instance.SetStatic(0f);
		PlayerEffectController.Instance.SetFlicker(0f);
	}

	public void OnTeleport()
	{
		PlayOneShot(AudioType.DBAnglerfishDetectTarget, 1f, Random.Range(0.9f, 1.1f));

		if (_controller.LineOfSightFraction() > 0.6f)
		{
			AudioUtility.PlayOneShot(AudioType.GhostSequence_Fear_Slam, 2f);
		}
	}

	public void PlayOneShot(AudioType type, float volume = 1f, float pitch = 1f)
	{
		_oneShotAudioSource.pitch = pitch;
		_oneShotAudioSource.PlayOneShot(type, volume);
	}

	public void OnGrab()
	{
		AudioUtility.PlayOneShot(AudioType.GhostSequence_Fear_Slam, 2f);
		PlayOneShot(AudioType.Ghost_DeathSingle);
		_musicPlaying = false;
		_fearMusic.FadeOut(0.2f);
		_suspenseMusic.FadeOut(0.2f);
	}

	#region animation callbacks or wtv
	private void Anim_SnapNeck() =>
		SnapNeck?.Invoke();

	private void Anim_SnapNeck_Audio()
	{
		PlayOneShot(AudioType.DBAnglerfishDetectDisturbance, 1f, 1.2f);
		Locator.GetPlayerAudioController().PlayOneShotInternal(AudioType.Ghost_NeckSnap);
		RumbleManager.PlayGhostNeckSnap();
	}

	private void Anim_LiftPlayer() =>
		LiftPlayer?.Invoke();

	private void Anim_LiftPlayer_Audio()
	{
		PlayOneShot(AudioType.GhostSequence_Fear_Slam, 1f, 1f);
		Locator.GetPlayerAudioController().PlayOneShotInternal(AudioType.Death_Crushed);
	}

	private void Anim_CallForHelp()
	{
		PlayOneShot(AudioType.Ghost_Grab_Scream, 1f, 1.2f);
	}
	#endregion
}
