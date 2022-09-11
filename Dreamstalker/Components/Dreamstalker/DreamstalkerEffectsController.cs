using Dreamstalker.Handlers.SolarSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

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

    public void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<DreamstalkerController>();

        gameObject.AddComponent<AudioSource>().spatialBlend = 1f;
        _oneShotAudioSource = gameObject.AddComponent<OWAudioSource>();
        _oneShotAudioSource.SetTrack(OWAudioMixer.TrackName.Environment);

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

    public void UpdateEffects()
    {
        var relativeVelocity = _controller.GetRelativeVelocity();
        var speed = 2f;

        var targetValue = new Vector2(relativeVelocity.x / speed, relativeVelocity.z / speed);
        _smoothedMoveSpeed = _moveSpeedSpring.Update(_smoothedMoveSpeed, targetValue, Time.deltaTime);
        _animator.SetFloat(GhostEffects.AnimatorKeys.Float_MoveDirectionX, _smoothedMoveSpeed.x);
        _animator.SetFloat(GhostEffects.AnimatorKeys.Float_MoveDirectionY, _smoothedMoveSpeed.y);
        _smoothedTurnSpeed = _turnSpeedSpring.Update(_smoothedTurnSpeed, _controller.GetAngularVelocity() / 90f, Time.deltaTime);
        _animator.SetFloat(GhostEffects.AnimatorKeys.Float_TurnSpeed, _smoothedTurnSpeed);

        ToggleWalk(relativeVelocity.ApproxEquals(Vector3.zero));

        if (_callForHelpTime != 0f && _callForHelpTime < Time.time)
        {
            CallForHelpComplete?.Invoke();
            _callForHelpTime = 0f;
        }

        var distance = (Locator.GetPlayerTransform().position - transform.position).magnitude;

        var flickerIntensity = Mathf.Clamp01(1f - distance / 20f);
        PlayerEffectController.Instance.SetStatic(flickerIntensity);
        PlayerEffectController.Instance.SetFlicker(_controller.IsVisible() ? 6 * flickerIntensity * _controller.LineOfSightFraction() : 0f);
    }

    public void OnTeleport()
    {
        PlayOneShot(AudioType.Ghost_Identify_Curious, 1f, UnityEngine.Random.Range(0.9f, 1.1f));

        if (_controller.LineOfSightFraction() > 0.5f)
        {
            PlayerEffectController.Instance.Blink();
        }
    }

    public void PlayOneShot(AudioType type, float volume = 1f, float pitch = 1f)
    {
        _oneShotAudioSource.pitch = pitch;
        _oneShotAudioSource.PlayOneShot(type, volume);
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
        PlayOneShot(AudioType.DBAnglerfishDetectTarget, 1f, 1.2f);
    }
    #endregion
}
