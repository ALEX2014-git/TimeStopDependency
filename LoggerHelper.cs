using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using RWCustom;
using BepInEx;
using Debug = UnityEngine.Debug;
using System.Data.SqlClient;
using BepInEx.Logging;
using System.Globalization;
using CoralBrain;
using Expedition;
using HUD;
using JollyCoop;
using JollyCoop.JollyMenu;
using MoreSlugcats;
using Noise;
using static UpdatableAndDeletable;
using Menu;
using Rewired;
using UnityEngine.SocialPlatforms;
using UnityEngine.Rendering;
using System.Runtime.Serialization;
using Steamworks;
using IL.Menu.Remix.MixedUI;
using Menu.Remix.MixedUI;
using IL;
using On.Menu.Remix.MixedUI;
using System.Runtime.Remoting.Messaging;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using System.Runtime.Remoting.Lifetime;
using System.Xml;
using System.Reflection;
using MonoMod.RuntimeDetour.HookGen;
using System.Reflection.Emit;
using MonoMod;
using Unity.Collections.LowLevel.Unsafe;
using On;
using HarmonyLib.Tools;

namespace TimeStopDependency
{
    internal static class LoggerExtensions
    {
        public enum LogType { Info, Message, Debug, Warning, Error, Fatal }
        public enum LogLevel { None, Low, Normal, High }
        public static void CustomLog(this ManualLogSource logger, string message, LogType logType, LogLevel logLevel)
        {
            if (!PluginOptions.ShouldLog(logLevel, TimeStopDependency.Instance.options.LogLevel.Value)) return;
            switch (logType)
            {
                case LogType.Info:
                    TimeStopDependency.Logger.LogInfo(message);
                    break;
                case LogType.Message:
                    TimeStopDependency.Logger.LogMessage(message);
                    break;
                case LogType.Debug:
                    TimeStopDependency.Logger.LogDebug(message);
                    break;
                case LogType.Warning:
                    TimeStopDependency.Logger.LogWarning(message);
                    break;
                case LogType.Error:
                    TimeStopDependency.Logger.LogError(message);
                    break;
                case LogType.Fatal:
                    TimeStopDependency.Logger.LogFatal(message);
                    break;
                default:
                    TimeStopDependency.Logger.LogInfo(message);
                    break;
            }
        }
    }
}
