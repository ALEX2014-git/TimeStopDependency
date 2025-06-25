using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using TimeStopDependency;

namespace TimeStopDependency
{
    public partial class TimeStopDependency
    {
        private void Room_Update(MonoMod.Cil.ILContext il)
        {
            var c = new ILCursor(il);
            var d = new ILCursor(il);
            var e = new ILCursor(il);
            var f = new ILCursor(il);
            var g = new ILCursor(il);
            var h = new ILCursor(il);

            c.GotoNext(
            MoveType.After,
            x => x.MatchLdarg(0),
            x => x.MatchLdfld(typeof(Room).GetField("game")),
            x => x.MatchLdfld(typeof(RainWorldGame).GetField("pauseUpdate"))
            );
            c.MoveAfterLabels();
            c.Emit(OpCodes.Ldarg_0);
            bool CheckForTimeStopInRoomUpdateForBakgroundNoice(bool pauseUpdate, Room this_arg)
            {
                if (pauseUpdate || this_arg.game.GetCustomData().isTimeStopActive) return true;
                return false;
            }
            c.EmitDelegate<Func<bool, Room, bool>>(CheckForTimeStopInRoomUpdateForBakgroundNoice);

            d.GotoNext(
            MoveType.After,
            x => x.MatchStfld(typeof(RoomSettings.RoomEffect).GetField("amount")),
            x => x.MatchLdarg(0),
            x => x.MatchLdfld(typeof(Room).GetField("game")),
            x => x.MatchLdfld(typeof(RainWorldGame).GetField("pauseUpdate"))
            );
            d.MoveAfterLabels();
            d.Emit(OpCodes.Ldarg_0);
            bool CheckForTimeStopInRoomUpdateForCellingDrips(bool pauseUpdate, Room this_arg)
            {
                if (pauseUpdate || this_arg.game.GetCustomData().isTimeStopActive) return true;
                return false;
            }
            d.EmitDelegate<Func<bool, Room, bool>>(CheckForTimeStopInRoomUpdateForCellingDrips);

            e.GotoNext(
            MoveType.After,
            x => x.MatchLdfld(typeof(Room).GetField("shortcutsBlinking")),
            x => x.MatchBrfalse(out _),
            x => x.MatchLdarg(0),
            x => x.MatchLdfld(typeof(Room).GetField("game")),
            x => x.MatchLdfld(typeof(RainWorldGame).GetField("pauseUpdate"))
            );
            e.MoveAfterLabels();
            e.Emit(OpCodes.Ldarg_0);
            bool CheckForTimeStopInRoomUpdateForShortcutsBlinking(bool pauseUpdate, Room this_arg)
            {
                if (pauseUpdate || this_arg.game.GetCustomData().isTimeStopActive) return true;
                return false;
            }
            e.EmitDelegate<Func<bool, Room, bool>>(CheckForTimeStopInRoomUpdateForShortcutsBlinking);

            f.GotoNext(
            MoveType.After,
            x => x.MatchCallvirt(typeof(DebugPerTileVisualizer).GetMethod("Update", new Type[] { typeof(Room) })),
            x => x.MatchLdarg(0),
            x => x.MatchLdfld(typeof(Room).GetField("game")),
            x => x.MatchLdfld(typeof(RainWorldGame).GetField("pauseUpdate"))
            );
            f.MoveAfterLabels();
            f.Emit(OpCodes.Ldarg_0);
            bool CheckForTimeStopInRoomUpdateForUpdatingCreaturesInRoom(bool pauseUpdate, Room this_arg)
            {
                if (pauseUpdate || this_arg.game.GetCustomData().isTimeStopActive) return true;
                return false;
            }
            f.EmitDelegate<Func<bool, Room, bool>>(CheckForTimeStopInRoomUpdateForUpdatingCreaturesInRoom);

            g.GotoNext(
            MoveType.After,
            x => x.MatchLdloc(10),
            x => x.MatchLdarg(0),
            x => x.MatchLdfld(typeof(Room).GetField("game")),
            x => x.MatchLdfld(typeof(RainWorldGame).GetField("evenUpdate")),
            x => x.MatchCallvirt(typeof(UpdatableAndDeletable).GetMethod("Update", new Type[] { typeof(bool) }))
            );
            var endCode = e.DefineLabel();
            g.MarkLabel(endCode);


            h.GotoNext(
            MoveType.After,
            x => x.MatchLdarg(0),
            x => x.MatchLdloc(10),
            x => x.MatchCall(typeof(Room).GetMethod("ShouldBeDeferred", new Type[] { typeof(UpdatableAndDeletable) })),
            x => x.MatchStloc(11)
            );
            h.MoveAfterLabels();
            h.Emit(OpCodes.Ldloc, 10);
            h.Emit(OpCodes.Ldarg_0);
            bool CheckForTimeStopInRoomUpdateForUpdatingUAD(UpdatableAndDeletable varUAD, Room this_arg)
            {
                if (!this_arg.game.GetCustomData().isTimeStopActive ||
                    varUAD.GetCustomData().isImmuneToTimeStop ||
                    varUAD is IAmImmuneToTimeStop ||
                    varUAD.IsThisTypeImmune()
                    ) return true;
                return false;
            }
            h.EmitDelegate<Func<UpdatableAndDeletable, Room, bool>>(CheckForTimeStopInRoomUpdateForUpdatingUAD);
            h.Emit(OpCodes.Brfalse, endCode);

            Logger.LogInfo("Finisning Room.Update IL modification.");
        }
    }
}
