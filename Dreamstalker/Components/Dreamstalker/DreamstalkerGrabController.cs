using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dreamstalker.Components.Dreamstalker;

internal class DreamstalkerGrabController : MonoBehaviour
{
    private DreamstalkerEffectsController _effects;
    private PlayerAttachPoint _attachPoint;

    private Transform _origParent;

    private bool _holdingInPlace;
    private bool _grabMoveComplete;

    private float _grabStartTime;
    private float _grabMoveDuration;

    private Vector3 _startLocalPos;
    private Quaternion _startLocalRot;

    private Transform _holdPoint;
    private Transform _liftPoint;

    public void Awake()
    {
        _attachPoint = gameObject.AddComponent<PlayerAttachPoint>();
        _origParent = _attachPoint.transform.parent;

        var holdPointGO = new GameObject("GrabHoldTarget");
        holdPointGO.transform.parent = transform.parent;
        holdPointGO.transform.localPosition = new Vector3(-1.1522f, 1.2407f, 1.3994f);
        holdPointGO.transform.localRotation = Quaternion.Euler(340.1717f, 128.0523f, 9.4284f);
        _holdPoint = transform;

        _liftPoint = transform.parent.Find("Ghostbird_Skin_01:Ghostbird_Rig_V01:Base/Ghostbird_Skin_01:Ghostbird_Rig_V01:Root/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine03/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine04/Ghostbird_Skin_01:Ghostbird_Rig_V01:ClavicleL/Ghostbird_Skin_01:Ghostbird_Rig_V01:ShoulderL/Ghostbird_Skin_01:Ghostbird_Rig_V01:ElbowL/Ghostbird_Skin_01:Ghostbird_Rig_V01:WristL/Ghostbird_Skin_01:Ghostbird_Rig_V01:HandAttachL/LiftHoldTarget").transform;

        _effects = transform.parent.GetComponent<DreamstalkerEffectsController>();
        _effects.SnapNeck.AddListener(OnSnapNeck);
        _effects.LiftPlayer.AddListener(OnStartLiftPlayer);

        enabled = false;
    }

    public void OnDestroy()
    {
        if (_effects != null)
        {
            _effects.SnapNeck.RemoveListener(OnSnapNeck);
            _effects.LiftPlayer.RemoveListener(OnStartLiftPlayer);
        }
    }

    public void GrabPlayer(float speed)
    {
        Main.Log($"Grabbing the player");

        if (PlayerState.IsAttached())
        {
            return;
        }

        enabled = true;
        _holdingInPlace = true;
        _grabMoveComplete = false;

        _attachPoint.transform.parent = _origParent;
        _attachPoint.transform.position = Locator.GetPlayerTransform().position;
        _attachPoint.transform.rotation = Locator.GetPlayerTransform().rotation;

        _startLocalPos = _attachPoint.transform.localPosition;
        _startLocalRot = _attachPoint.transform.localRotation;

        _attachPoint.AttachPlayer();
        OWInput.ChangeInputMode(InputMode.None);
        ReticleController.Hide();

        _grabStartTime = Time.time;
        _grabMoveDuration = Mathf.Min(Vector3.Distance(_startLocalPos, _holdPoint.localPosition) / speed, 2f);

        _effects.PlayAnimation(DreamstalkerEffectsController.AnimationKeys.SnapNeck);

        _effects.PlayOneShot(AudioType.Ghost_Grab_Contact);
        RumbleManager.PlayGhostGrab();
    }

    private void OnStartLiftPlayer()
    {
        _holdingInPlace = false;
        _attachPoint.transform.parent = _liftPoint;
        _startLocalPos = _attachPoint.transform.localPosition;
        _startLocalRot = _attachPoint.transform.localRotation;
        _grabStartTime = Time.time;
        _grabMoveDuration = Mathf.Min(Vector3.Distance(_startLocalPos, Vector3.zero) / 0.5f, 1f);
    }

    private void FixedUpdate()
    {
        float num = Mathf.InverseLerp(_grabStartTime, _grabStartTime + _grabMoveDuration, Time.time);
        num = Mathf.SmoothStep(0f, 1f, num);
        if (_holdingInPlace)
        {
            _attachPoint.transform.localPosition = Vector3.Lerp(_startLocalPos, _holdPoint.localPosition, num);
            _attachPoint.transform.localRotation = Quaternion.Slerp(_startLocalRot, _holdPoint.localRotation, num);
            return;
        }
        if (!_grabMoveComplete)
        {
            _attachPoint.transform.localPosition = Vector3.Lerp(_startLocalPos, Vector3.zero, num);
            _attachPoint.transform.localRotation = Quaternion.Slerp(_startLocalRot, Quaternion.identity, num);
            if (num >= 1f)
            {
                _grabMoveComplete = true;
            }
        }
    }

    private void OnSnapNeck()
    {
        Locator.GetDeathManager().KillPlayer(DeathType.CrushedByElevator);

        // Release them
        _holdingInPlace = false;
        _attachPoint.DetachPlayer();
        _attachPoint.transform.parent = _origParent;
        OWInput.ChangeInputMode(InputMode.Character);
        enabled = false;
    }
}
