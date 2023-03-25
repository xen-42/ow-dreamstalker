using Dreamstalker.Utility;
using System;
using System.Collections.Generic;

namespace Dreamstalker.External;

public class DreamstalkerData
{
	private static DreamstalkerSaveFile _saveFile;
	public static DreamstalkerProfile ActiveProfile { get; private set; }
	private static string _activeProfileName;
	private const string FileName = "save.json";

	public static string GetProfileName() => StandaloneProfileManager.SharedInstance?.currentProfile?.profileName ?? "Default";

	public static void Load()
	{
		_activeProfileName = GetProfileName();

		try
		{
			_saveFile = Main.Instance.ModHelper.Storage.Load<DreamstalkerSaveFile>(FileName);

			if (!_saveFile.Profiles.ContainsKey(_activeProfileName))
			{
				_saveFile.Profiles.Add(_activeProfileName, new DreamstalkerProfile());
			}

			ActiveProfile = _saveFile.Profiles[_activeProfileName];
			Main.Log($"Loaded save data for {_activeProfileName}");
		}
		catch (Exception)
		{
			try
			{
				Main.Log($"Couldn't load save data from {FileName}, creating a new file");

				_saveFile = new DreamstalkerSaveFile();
				_saveFile.Profiles.Add(_activeProfileName, new DreamstalkerProfile());
				ActiveProfile = _saveFile.Profiles[_activeProfileName];
				Main.Instance.ModHelper.Storage.Save(_saveFile, FileName);

				Main.Log($"Loaded save data for {_activeProfileName}");
			}
			catch (Exception e)
			{
				Main.LogError($"Couldn't create save data:\n{e}");
			}
		}

		Enum.TryParse(ActiveProfile.LastPlanet, out PlayerSpawnUtil.LastSpawn);
	}

	public static void Save()
	{
		if (_saveFile != null)
		{
			ActiveProfile.LastPlanet = PlayerSpawnUtil.LastSpawn.ToString();

			Main.Instance.ModHelper.Storage.Save(_saveFile, FileName);
		}
	}

	public static void Reset()
	{
		if (_saveFile == null || ActiveProfile == null)
		{
			Load();
		}
		Main.Log($"Resetting save data for {_activeProfileName}");

		ActiveProfile = new DreamstalkerProfile();
		_saveFile.Profiles[_activeProfileName] = ActiveProfile;

		Save();
	}

	private class DreamstalkerSaveFile
	{
		public Dictionary<string, DreamstalkerProfile> Profiles { get; }

		public DreamstalkerSaveFile()
		{
			Profiles = new();
		}
	}

	public class DreamstalkerProfile 
	{
		public string LastPlanet { get; set; }
	}
}
