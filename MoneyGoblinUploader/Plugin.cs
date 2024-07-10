using System.Reflection;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using MoneyGoblin.Windows;
using MoneyGoblin.Utils;
using MoneyGoblin.IPC;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System.Text;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using MoneyGoblinUploader.Utils;
using System;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace MoneyGoblin
{
    public class Plugin : IDalamudPlugin
    {
        public string Name => "Money Goblin";
        [PluginService] public static IDataManager Data { get; private set; } = null!;
        [PluginService] public static IFramework Framework { get; private set; } = null!;
        [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] public static IClientState ClientState { get; private set; } = null!;
        [PluginService] public static ISigScanner SigScanner { get; private set; } = null!;
        [PluginService] public static IGameInteropProvider Hook { get; private set; } = null!;
        [PluginService] public static IPluginLog Log { get; private set; } = null!;

        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("Money Goblin");

        private ConfigWindow ConfigWindow { get; init; }

        public const string Authors = "Luo";
        public static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

        private static ExcelSheet<TerritoryType> TerritoryTypes = null!;

        public static HookManager HookManager = null!;
        public static AllaganToolsConsumer AllaganToolsConsumer = null!;

        public static uint[] ReturnTime = new uint[4] { 0, 0, 0, 0 };

        public Plugin()
        {

            HookManager = new HookManager(this);
            AllaganToolsConsumer = new AllaganToolsConsumer(this);


            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            ConfigWindow = new ConfigWindow(this);
            
            WindowSystem.AddWindow(ConfigWindow);

            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += OpenConfig;

            TerritoryTypes = Data.GetExcelSheet<TerritoryType>()!;

            Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            
            ConfigWindow.Dispose();
            HookManager.Dispose();
            AllaganToolsConsumer.Unsubscribe();

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

            if (Plugin.ClientState.LocalPlayer == null) //Check if player exists
            {
                ReturnTime = new uint[4] { 0, 0, 0, 0 }; //reset timer cache if no player
                return;
            }

            WorkshopTerritory* WorkshopTerritory = HousingManager.Instance()->WorkshopTerritory;
            if (WorkshopTerritory == null) //Check if workshop exists
                return;

            if (TerritoryTypes.GetRow(ClientState.TerritoryType)!.TerritoryIntendedUse == 49) //Check if we're in IS instead
                return;

            IntPtr submersiblePtr = new IntPtr(WorkshopTerritory->Submersible.DataPointers[0]);
            if (submersiblePtr == IntPtr.Zero) //check if subs loaded yet
                return;

            //check for sub data
            for (int i = 0; i < 4; i++)
            {
                HousingWorkshopSubmersibleSubData* sub = WorkshopTerritory->Submersible.DataPointers[i];

                if (sub->ReturnTime != ReturnTime[i])
                {
                    uint rt = sub->ReturnTime;

                    var fc = (InfoProxyFreeCompany*)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.FreeCompany);
                    string fcid = fc->Id.ToString();

                    ReadOnlySpan<byte> nameSpan = sub->Name.Slice(0, 20);

                    ReturnTimePacket p = new ReturnTimePacket(sub->ReturnTime, fcid, Encoding.UTF8.GetString(nameSpan).TrimEnd('\0'), i);

                    Upload.PostJson(p.getJSON(), this.Configuration.TargetAddress, "returns");
                    ReturnTime[i] = rt;
                }

                
            }
            
        }
    }
}
