using UnityEngine;

namespace Dreamstalker.Handlers;

[RequireComponent(typeof(Main))]
public abstract class SolarSystemHandler : BaseHandler
{
    protected override void Awake()
    {
        base.Awake();

        _main.SolarSystemAwake.AddListener(OnSolarSystemAwake);
        _main.SolarSystemStart.AddListener(OnSolarSystemStart);
    }

    protected void OnDestroy()
    {
        if (_main != null)
        {
            _main.SolarSystemAwake.RemoveListener(OnSolarSystemAwake);
            _main.SolarSystemStart.RemoveListener(OnSolarSystemStart);
        }
    }

    protected abstract void OnSolarSystemAwake();

    protected abstract void OnSolarSystemStart();
}
