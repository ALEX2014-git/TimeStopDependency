using System.Collections.Generic;
using BepInEx.Logging;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace TimeStopDependency;

internal class PluginOptions : OptionInterface
{
    private readonly ManualLogSource Logger;

    public PluginOptions(TimeStopDependency modInstance, ManualLogSource loggerSource)
    {
        Logger = loggerSource;
        LogLevel = this.config.Bind<string>("LogLevel", "None");
    }

    private static readonly Dictionary<LoggerExtensions.LogLevel, int> LevelPriorities = new()
    {
        {LoggerExtensions.LogLevel.None, 0},
        {LoggerExtensions.LogLevel.Low, 1},
        {LoggerExtensions.LogLevel.Normal, 2},
        {LoggerExtensions.LogLevel.High, 3}
    };

    private static readonly Dictionary<string, LoggerExtensions.LogLevel> optionsToEnums = new()
    {
        {"None", LoggerExtensions.LogLevel.None},
        {"Low", LoggerExtensions.LogLevel.Low},
        {"Normal", LoggerExtensions.LogLevel.Normal },
        {"High", LoggerExtensions.LogLevel.High}
    };


    public readonly Configurable<string> LogLevel;
    private UIelement[] UIArrGeneral;
    private static readonly string[] LogLevels = { "None", "Low", "Normal", "High" };
    OpComboBox logLevelComboBox;
    OpLabel logLevelWarningText;
    OpTab opTab;

    public override void Initialize()
    {
        base.Initialize();
        opTab = new OpTab(this, "Options");
        this.Tabs = new[]
        {
            opTab
        };

        logLevelComboBox = new OpComboBox(LogLevel, new Vector2(80f, 520f), 100f, LogLevels);
        logLevelWarningText = new OpLabel(190f, 520f, "WARNING: High log level will log every update tick." + System.Environment.NewLine + "It's recommended to not use it unless needed.") { color = new Color(1f, 0f, 0f) };
        try
        {
            UIArrGeneral = new UIelement[]
            {
            new OpLabel(10f, 550f, "Options", true),
            new OpLabel(10f, 520f, "Log level"),
            logLevelComboBox,
            logLevelWarningText
            };
        }
        catch (System.Exception)
        {
            throw;
        }
        opTab.AddItems(UIArrGeneral);
    }

    public override void Update()
    {
        if (logLevelComboBox.value == "High")
        {
            logLevelWarningText.Show();
        }
        else
        {
            logLevelWarningText.Hide();
        }
    }

    internal static bool ShouldLog(LoggerExtensions.LogLevel arg_lLogLevel, string arg_sLogLevel = null)
    {
        arg_sLogLevel ??= TimeStopDependency.Instance.options.LogLevel.Value ?? "None";
        optionsToEnums.TryGetValue(arg_sLogLevel, out var convSLogLevel);
        LevelPriorities.TryGetValue(convSLogLevel, out var sLogLevel);
        if (sLogLevel == 0) return false;

        LevelPriorities.TryGetValue(arg_lLogLevel, out var lLogLevel);
        if (lLogLevel >= sLogLevel) return true;
        return false;
    }

}