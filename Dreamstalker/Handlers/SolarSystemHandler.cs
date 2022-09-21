using System;
using UnityEngine;

namespace Dreamstalker.Handlers;

[RequireComponent(typeof(Main))]
public abstract class SolarSystemHandler : BaseHandler
{
    protected override void Awake()
    {
        base.Awake();

        _main.SolarSystemAwake.AddListener(TryOnSolarSystemAwake);
        _main.SolarSystemStart.AddListener(TryOnSolarSystemStart);
    }

    protected virtual void OnDestroy()
    {
        if (_main != null)
        {
            _main.SolarSystemAwake.RemoveListener(TryOnSolarSystemAwake);
            _main.SolarSystemStart.RemoveListener(TryOnSolarSystemStart);
        }
    }

    private void TryOnSolarSystemAwake()
    {
        try
        {
            OnSolarSystemAwake();
        }
        catch (Exception e)
        {
            Main.LogError($"{e}");
        }
    }

	private void TryOnSolarSystemStart()
	{
		try
		{
			OnSolarSystemStart();
		}
		catch (Exception e)
		{
			Main.LogError($"{e}");
		}
	}

	protected abstract void OnSolarSystemAwake();

    protected abstract void OnSolarSystemStart();
}
