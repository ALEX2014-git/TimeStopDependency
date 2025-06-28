using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using BepInEx.Logging;
using MonoMod.RuntimeDetour;
using System.IO;
using UnityEngine;
using RWCustom;

#pragma warning disable CS0618
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace TimeStopDependency;


[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
public partial class TimeStopDependency : BaseUnityPlugin
{
    public const string PLUGIN_GUID = "ALEX2014.TimeStopDependency";
    public const string PLUGIN_NAME = "Time Stop Dependency";
    public const string PLUGIN_VERSION = "1.1.0";

    internal PluginOptions options;

    public Player player;

    public static ManualLogSource Logger { get; private set; }

    private static TimeStopDependency _instance;
    internal static List<Type> TimeStopImmuneTypes = new List<Type> { typeof(Player) };
    private static readonly object immuneLogLock = new object();
    private static string ImmuneLogFilePath;
    public static TimeStopDependency Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new Exception("Instance of PebblesBlastsCaramelldansen is not created yet!");
            }
            return _instance;
        }
    }

    public TimeStopDependency()
    {
        try
        {
            _instance = this;
            //options = new PluginOptions(this, Logger);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }

    private void OnEnable()
    {
        TimeStopDependency.Logger = base.Logger;
        On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
        if (File.Exists(ImmuneLogFilePath))
        {
            File.Delete(ImmuneLogFilePath);
        }
    }

    BindingFlags propFlags = BindingFlags.Instance | BindingFlags.Public;
    BindingFlags myMethodFlags = BindingFlags.Static | BindingFlags.NonPublic;
    private bool IsInit;
    private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsInit) return;
            Logger.LogInfo("Starting initialization");
            Logger.LogInfo("Starting MonoMod hooks");
            On.RainWorldGame.ShutDownProcess += RainWorldGame_ShutDownProcess;
            On.GameSession.ctor += GameSession_ctor;
            On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
            On.PhysicalObject.Update += PhysicalObject_Update;
            On.Weapon.Grabbed += Weapon_Grabbed;
            On.RainWorldGame.AllowRainCounterToTick += RainWorldGame_AllowRainCounterToTick;
            Logger.LogInfo("Finished MonoMod hooks");

            Logger.LogInfo("Starting IL hooks");
            IL.RainWorldGame.GrafUpdate += RainWorldGame_GrafUpdate;
            IL.RainWorldGame.Update += RainWorldGame_Update;
            IL.Room.Update += Room_Update;
            Logger.LogInfo("Finished IL hooks");

            Logger.LogInfo("Starting manual hooks");
            Hook rainWorldGameTimeSpeedFac = new Hook(
            typeof(RainWorldGame).GetProperty("TimeSpeedFac", propFlags).GetGetMethod(),
            typeof(TimeStopDependency).GetMethod("RainWorldGame_TimeSpeedFac_get", myMethodFlags)
            );
            Logger.LogInfo("Finished manual hooks");

            //MachineConnector.SetRegisteredOI("ALEX2014.TimeStopDependency", options);
            IsInit = true;
            Logger.LogInfo("Finished initialization");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }

    internal static void LogToImmuneFile(string message)
    {
        if (String.IsNullOrEmpty(ImmuneLogFilePath))
        {
            ImmuneLogFilePath = Path.Combine(Custom.RootFolderDirectory(), "timeStopImmuneListLog.txt");
        }
        lock (immuneLogLock)
        {
            File.AppendAllText(ImmuneLogFilePath, message + Environment.NewLine);          
        }
    }


    private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
    {
        orig(self, newlyDisabledMods);

    }

    private void RainWorldGame_ShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, global::RainWorldGame self)
    {
        orig(self);
        ClearMemory();
    }

    private void GameSession_ctor(On.GameSession.orig_ctor orig, GameSession self, global::RainWorldGame game)
    {
        orig(self, game);
        ClearMemory();
    }

    #region Helper Methods

    private void ClearMemory()
    {
        //If you have any collections (lists, dictionaries, etc.)
        //Clear them here to prevent a memory leak
        //YourList.Clear();
    }

    #endregion
}
