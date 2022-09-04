using OWML.ModHelper;
using OWML.Common;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using Dreamstalker.Handlers;

namespace Dreamstalker;

public class Main : ModBehaviour
{
    public INewHorizons NewHorizonsAPI;

    private static Main Instance;

    public UnityEvent SolarSystemAwake = new();
    public UnityEvent SolarSystemStart = new();

    private void Start()
    {
        Instance = this;

		NewHorizonsAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
        NewHorizonsAPI.LoadConfigs(this);

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Add in the handlers
        gameObject.AddComponent<DestructionHandler>();
        gameObject.AddComponent<SunHandler>();
        gameObject.AddComponent<TimberHearthHandler>();
        gameObject.AddComponent<SpawnHandler>();

        // Title screen is already loaded
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SolarSystem")
        {
            Log("Solar system loaded event invocation");
			SolarSystemAwake?.Invoke();
            FireOnNextUpdate(() => SolarSystemStart?.Invoke());
		}
    }

    public static void FireOnNextUpdate(Action action) =>
        Instance.ModHelper.Events.Unity.FireOnNextUpdate(action);

    public static void Log(string msg) =>
        Instance.ModHelper.Console.WriteLine($"Info: {msg}", MessageType.Info);

	public static void LogError(string msg) =>
	    Instance.ModHelper.Console.WriteLine($"Error: {msg}", MessageType.Error);
}
