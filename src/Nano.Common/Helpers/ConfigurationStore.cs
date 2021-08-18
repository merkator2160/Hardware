using Nano.Common.Models;
using nanoFramework.Json;
using System;
using Windows.Storage;

namespace Nano.Common.Helpers
{
	public class ConfigurationStore
	{
		private readonly String _configFilePath;
		private readonly StorageFolder _configFolder;


		public ConfigurationStore(String path = "I:\\configuration.json")
		{
			_configFilePath = path;
			var internalDevices = KnownFolders.InternalDevices;
			var flashDevices = internalDevices.GetFolders();
			_configFolder = flashDevices[0];
		}


		// FUNCTIONS //////////////////////////////////////////////////////////////////////////////
		public void ClearConfig()
		{
			SaveConfig(new Configuration());
		}
		public Configuration GetConfig()
		{
			var configFile = StorageFile.GetFileFromPath(_configFilePath);
			var json = FileIO.ReadText(configFile);
			var config = (Configuration)JsonConvert.DeserializeObject(json, typeof(Configuration));

			return config;
		}
		public void SaveConfig(Configuration config)
		{
			var configJson = JsonConvert.SerializeObject(config);
			var configFile = _configFolder.CreateFile("configuration.json", CreationCollisionOption.ReplaceExisting);
			FileIO.WriteText(configFile, configJson);
		}
	}
}