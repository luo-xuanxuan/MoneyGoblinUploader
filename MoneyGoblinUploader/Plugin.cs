using System.Reflection;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using MoneyGoblin.Windows;
using MoneyGoblin.Utils;

namespace MoneyGoblin
{
    public class Plugin : IDalamudPlugin
    {
        public string Name => "Money Goblin";
        [PluginService] public static IFramework Framework { get; private set; } = null!;
        [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] public static IClientState ClientState { get; private set; } = null!;
        [PluginService] public static ISigScanner SigScanner { get; private set; } = null!;
        [PluginService] public static IGameInteropProvider Hook { get; private set; } = null!;
        [PluginService] public static IPluginLog Log { get; private set; } = null!;

        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("Money Goblin");

        private ConfigWindow ConfigWindow { get; init; }

        public const string Authors = "Luo";
        public static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

        public static HookManager HookManager = null!;

        public Plugin()
        {

            HookManager = new HookManager(this);


            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            ConfigWindow = new ConfigWindow(this);
            
            WindowSystem.AddWindow(ConfigWindow);

            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += OpenConfig;

            Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            
            ConfigWindow.Dispose();
            HookManager.Dispose();

            PluginInterface.UiBuilder.Draw -= DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi -= OpenConfig;

            Framework.Update -= OnFrameworkUpdate;
        }

        public void OpenConfig() => ConfigWindow.IsOpen = true;

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public unsafe void OnFrameworkUpdate(IFramework _)
        {
            //debug = "";

            //PluginLog.Log($"Housing Manager: {(nint)HousingManager.Instance():X}");
            //var info = InfoModule.Instance();
            //var fc = (InfoProxyFreeCompany*)info->GetInfoProxyById(InfoProxyId.FreeCompany);
            //var playerAddress = Plugin.ClientState.LocalPlayer.Address;
            //var party = (InfoProxyParty*)info->GetInfoProxyById(InfoProxyId.Party);
            //var id = party->InfoProxyCommonList.CharDataSpan[0].ContentId;
            //var id = info->LocalContentId;
            //Plugin.Log.Information(fc->ID.ToString()); //lodestone fc id
            //Plugin.Log.Information($"Local Player: {(nint)playerAddress:X}");
            //Plugin.Log.Information($"Local Player: {id}");
        }
    }
}
