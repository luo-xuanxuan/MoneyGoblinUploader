using System.IO;
using Dalamud.Configuration;
using Dalamud.Plugin;
using Newtonsoft.Json;
using System;
using Dalamud.Logging;

namespace MoneyGoblin
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public string TargetAddress { get; set; } = "";

        public void Save()
        {
            PluginLog.Information("Saving to: " + Plugin.PluginInterface.ConfigFile.FullName);
            WriteAllTextSafe(Plugin.PluginInterface.ConfigFile.FullName, JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.Objects
            }));
        }

        internal static void WriteAllTextSafe(string path, string text)
        {
            try
            {
                var str = path + ".tmp";
                if (File.Exists(str))
                    File.Delete(str);
                File.WriteAllText(str, text);
                File.Move(str, path, true);
            }
            catch (Exception e)
            {
                Plugin.Log.Error(e.Message);
                Plugin.Log.Error(e.StackTrace ?? "Unknown");
            }
        }
    }
}
