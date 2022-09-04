using Dreamstalker.Handlers.SolarSystem;
using Dreamstalker.Handlers.TitleScreen;
using OWML.Common;
using OWML.ModHelper;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Dreamstalker;

public class Main : ModBehaviour
{
    public INewHorizons NewHorizonsAPI;

    public static Main Instance;

    public UnityEvent SolarSystemAwake = new();
    public UnityEvent SolarSystemStart = new();

    public UnityEvent TitleScreenAwake = new();

    private void Start()
    {
        Instance = this;

		NewHorizonsAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
        NewHorizonsAPI.LoadConfigs(this);

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Add in the handlers
        gameObject.AddComponent<GeneralHandler>();
        gameObject.AddComponent<SunHandler>();
        gameObject.AddComponent<TimberHearthHandler>();
        gameObject.AddComponent<SpawnHandler>();

        gameObject.AddComponent<TitleScreenHandler>();

        // Title screen is already loaded
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScreen")
        {
			Log("TitleScreen loaded event invocation");
			TitleScreenAwake?.Invoke();
		}

        if (scene.name == "SolarSystem")
        {
            Log("SolarSystem loaded event invocation");
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
