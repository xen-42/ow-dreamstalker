﻿using Dreamstalker.Handlers.Credits;
using Dreamstalker.Handlers.EyeScene;
using Dreamstalker.Handlers.SolarSystem;
using Dreamstalker.Handlers.TitleScreen;
using HarmonyLib;
using NewHorizons.Handlers;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Dreamstalker;

[HarmonyPatch]
public class Main : ModBehaviour
{
	public INewHorizons NewHorizonsAPI;

	public static Main Instance;

	public UnityEvent BeforePlanetCreation = new();
	public UnityEvent SolarSystemAwake = new();
	public UnityEvent SolarSystemStart = new();
	public UnityEvent TitleScreenAwake = new();
	public UnityEvent EyeSceneAwake = new();
	public UnityEvent FastCredits = new();

	public static bool DebugMode { get; private set; } = false;

	internal void Awake()
	{
		Instance = this;
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
	}

	internal void Start()
	{
		NewHorizonsAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
		NewHorizonsAPI.LoadConfigs(this);

		SceneManager.sceneLoaded += OnSceneLoaded;

		// Add in the handlers
		gameObject.AddComponent<GeneralHandler>();
		gameObject.AddComponent<PlanetHandler>();
		gameObject.AddComponent<EyeHandler>();

		gameObject.AddComponent<TimberHearthHandler>();
		gameObject.AddComponent<BrittleHollowHandler>();
		gameObject.AddComponent<CaveTwinHandler>();
		gameObject.AddComponent<DreamworldHandler>();
		gameObject.AddComponent<GiantsDeepHandler>();
		gameObject.AddComponent<DarkBrambleHandler>();
		gameObject.AddComponent<QuantumMoonHandler>();
		gameObject.AddComponent<AncientGladeHandler>();
		gameObject.AddComponent<PauseMenuHandler>();

		gameObject.AddComponent<TitleScreenHandler>();

		gameObject.AddComponent<DreamstalkerCreditsHandler>();

		// Title screen is already loaded
		OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
	}

	internal void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		switch (scene.name)
		{
			case "TitleScreen":
				Log("TitleScreen loaded event invocation");
				TitleScreenAwake?.Invoke();
				break;
			case "SolarSystem":
				Log("SolarSystem loaded event invocation");
				SolarSystemAwake?.Invoke();
				FireOnNextUpdate(() => SolarSystemStart?.Invoke());
				break;
			case "EyeOfTheUniverse":
				Log("EyeOfTheUniverse loaded event invocation");
				EyeSceneAwake?.Invoke();
				break;
			case "Credits_Fast":
				Log("Credits_Fast loaded event invocation");
				FastCredits?.Invoke();
				break;
		}
	}

	internal void Update()
	{
		if (Keyboard.current[Key.L].isPressed && Keyboard.current[Key.Numpad0].wasReleasedThisFrame)
		{
			DebugMode = !DebugMode;
			Log($"Debug mode is {(DebugMode ? "on" : "off")}");
		}
	}

	public static void FireOnNextUpdate(Action action) =>
		Instance.ModHelper.Events.Unity.FireOnNextUpdate(action);

	public static void Log(string msg) =>
		Instance.ModHelper.Console.WriteLine($"Info: {msg}", MessageType.Info);

	public static void LogError(string msg) =>
		Instance.ModHelper.Console.WriteLine($"Error: {msg}", MessageType.Error);

	public static void RunAfterSeconds(Action action, float seconds) =>
		Instance.StartCoroutine(Coroutine_RunAfterSeconds(action, seconds));

	private static IEnumerator Coroutine_RunAfterSeconds(Action action, float seconds)
	{
		yield return new WaitForSeconds(seconds);
		action?.Invoke();
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(PlanetCreationHandler), nameof(PlanetCreationHandler.Init))]
	private static void PlanetCreationHandler_Init() => Instance.BeforePlanetCreation?.Invoke();
}
