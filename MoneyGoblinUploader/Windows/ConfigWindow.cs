using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace MoneyGoblin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    public ConfigWindow(Plugin plugin) : base(
        "Money Goblin Config", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        this.Size = new Vector2(400, 110);
        //this.SizeCondition = ImGuiCond.Always;

        this.Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text("Target Address:");
        var target = this.Configuration.TargetAddress;
        ImGui.InputText(string.Empty, ref target, 200);
        if (ImGui.IsItemDeactivatedAfterEdit())
        {
            this.Configuration.TargetAddress = target;
        }
        if(ImGui.Button("Save"))
        {
            this.Configuration.Save();
        }
        
    }
}
