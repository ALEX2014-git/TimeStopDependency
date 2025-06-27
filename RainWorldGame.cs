using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using UnityEngine;
using Watcher;
using System.Diagnostics;

namespace TimeStopDependency
{
    
    public partial class TimeStopDependency
    {
        private bool RainWorldGame_AllowRainCounterToTick(On.RainWorldGame.orig_AllowRainCounterToTick orig, RainWorldGame self)
        {
            if (self.GetCustomData().isTimeStopActive) return false;
            return orig(self);
        }

        internal delegate float orig_TimeSpeedFac(RainWorldGame self);

        internal static float RainWorldGame_TimeSpeedFac_get(orig_TimeSpeedFac orig, RainWorldGame self)
        {
            if (self.GetCustomData().isTimeStopActive && !self.GetCustomData().allowEffectsGraohicUpdate)
            {            
                return 0f;
            }
            return orig(self);
        }

        private void RainWorldGame_GrafUpdate(MonoMod.Cil.ILContext il)
        {
            var c = new ILCursor(il);

            c.GotoNext(
                MoveType.After,
                x => x.MatchLdarg(1),
                y => y.MatchStloc(0),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld(typeof(RainWorldGame).GetField("pauseUpdate"))
                );

            c.MoveAfterLabels();
            c.Emit(OpCodes.Ldarg_0);
            bool CheckForTimeStopInRainWorlGameGrafUpdate(bool pauseUpdate, RainWorldGame this_arg)
            {
                if (pauseUpdate || this_arg.GetCustomData().isTimeStopActive) return true;
                return false;
            }
            c.EmitDelegate<Func<bool, RainWorldGame, bool>>(CheckForTimeStopInRainWorlGameGrafUpdate);

            Logger.LogInfo("Finisning RainWorlGame.GrafUpdate IL modification.");
        }

        private void RainWorldGame_Update(MonoMod.Cil.ILContext il)
        {
            var c = new ILCursor(il);
            var d = new ILCursor(il);

            c.GotoNext(
            MoveType.After,
            x => x.MatchLdarg(0),
            x => x.MatchLdfld(typeof(RainWorldGame).GetField("pauseUpdate"))
            );

            c.MoveAfterLabels();
            c.Emit(OpCodes.Ldarg_0);
            bool CheckForTimeStopInRainWorlGameUpdateForGlobalRain(bool pauseUpdate, RainWorldGame this_arg)
            {
                if (pauseUpdate || this_arg.GetCustomData().isTimeStopActive) return true;
                return false;
            }
            c.EmitDelegate<Func<bool, RainWorldGame, bool>>(CheckForTimeStopInRainWorlGameUpdateForGlobalRain);

            d.GotoNext(
             MoveType.After,
             x => x.MatchLdarg(0),
             x => x.MatchLdfld(typeof(RainWorldGame).GetField("desintegrationTracker")),
             x => x.MatchBrfalse(out _),
             x => x.MatchLdarg(0),
             x => x.MatchLdfld(typeof(RainWorldGame).GetField("pauseUpdate"))
             );

            d.MoveAfterLabels();
            d.Emit(OpCodes.Ldarg_0);
            bool CheckForTimeStopInRainWorlGameUpdateForDesintegrationTracker(bool pauseUpdate, RainWorldGame this_arg)
            {
                if (pauseUpdate || this_arg.GetCustomData().isTimeStopActive) return true;
                return false;
            }
            d.EmitDelegate<Func<bool, RainWorldGame, bool>>(CheckForTimeStopInRainWorlGameUpdateForDesintegrationTracker);

            Logger.LogInfo("Finisning RainWorlGame.GameUpdate IL modification.");
        }
    }

    public static class RainWorlGameCWT
    {
        static ConditionalWeakTable<RainWorldGame, Data> table = new ConditionalWeakTable<RainWorldGame, Data>();

        public static Data GetCustomData(this RainWorldGame self) => table.GetOrCreateValue(self);

        public class Data
        {
            internal bool isTimeStopActive;
            internal bool allowEffectsGraohicUpdate;
        }
    }

    public static class RainWorldGameExtensions
    {
        public static void SwitchTimeState(this RainWorldGame rwg)
        {
            if (!rwg.GetCustomData().isTimeStopActive)
            {
                FreezeTime(rwg);
            }
            else
            {
                UnfreezeTime(rwg);
            }
        }

        public static void FreezeTime(this RainWorldGame rwg)
        {
            if (rwg.GetCustomData().isTimeStopActive) return;
            rwg.GetCustomData().isTimeStopActive = true;
        }

        public static void UnfreezeTime(this RainWorldGame rwg)
        {
            if (!rwg.GetCustomData().isTimeStopActive) return;
            rwg.GetCustomData().isTimeStopActive = false;
        }

        public static bool IsTimeStopActive(this RainWorldGame rwg)
        {
            return rwg.GetCustomData().isTimeStopActive;
        }

        /// <summary>
        /// Allows some decorative graphics and sound controllers to update in Time Stop state.
        /// Sets TimeSpeedFac to original value.
        /// </summary>
        public static void AllowEffectsUpdateDuringTimeStop(this RainWorldGame rwg)
        {
            if (rwg.GetCustomData().allowEffectsGraohicUpdate) return;
            rwg.GetCustomData().allowEffectsGraohicUpdate = true;
        }

        /// <summary>
        /// Prevents some decorative graphics and sound controllers from updating in Time Stop state.
        /// Sets TimeSpeedFac to 0f.
        /// </summary>
        public static void DisallowEffectsUpdateDuringTimeStop(this RainWorldGame rwg)
        {
            if (!rwg.GetCustomData().allowEffectsGraohicUpdate) return;
            rwg.GetCustomData().allowEffectsGraohicUpdate = false;
        }

        /// <summary>
        /// Returns whenever or not some decorative graphics and sound controllers can update.
        /// </summary>
        public static bool IsEffectsUpdateAllowed(this RainWorldGame rwg)
        {
            return rwg.GetCustomData().allowEffectsGraohicUpdate;
        }

        /// <summary>
        /// Adds class type to the TimeStopImmuneTypes list
        /// </summary>
        public static void AddImmuneType(this RainWorldGame rwg, Type type)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            string assemblyName = callingAssembly?.GetName().Name ?? "Unknown";
            string assemblyVersion = callingAssembly?.GetName().Version?.ToString() ?? "Unknown";

            if (!TimeStopDependency.TimeStopImmuneTypes.Contains(type))
            {
                TimeStopDependency.TimeStopImmuneTypes.Add(type);
                TimeStopDependency.Logger.LogMessage($"Added immune type: {type.Name} " +
                  $"| Caller: {callingAssembly?.FullName} " +
                  $"| Mod: {assemblyName} v{assemblyVersion}");

                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} : {assemblyName} v{assemblyVersion}] Added immune type {type.Name}";
                TimeStopDependency.LogToImmuneFile(logEntry);
            }
        }

        /// <summary>
        /// Removes class type from TimeStopImmuneTypes
        /// </summary>
        public static void RemoveImmuneType(this RainWorldGame rwg, Type type)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            string assemblyName = callingAssembly?.GetName().Name ?? "Unknown";
            string assemblyVersion = callingAssembly?.GetName().Version?.ToString() ?? "Unknown";

            if (TimeStopDependency.TimeStopImmuneTypes.Contains(type))
            {
                TimeStopDependency.TimeStopImmuneTypes.Remove(type);
                TimeStopDependency.Logger.LogMessage($"Removed immune type: {type.Name} " +
                  $"| Caller: {callingAssembly?.FullName} " +
                  $"| Mod: {assemblyName} v{assemblyVersion}");

                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} : {assemblyName} v{assemblyVersion}] Removed immune type {type.Name}";
                TimeStopDependency.LogToImmuneFile(logEntry);
            }
        }
    }
}
