using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BepInEx;

namespace TimeStopDependency
{
    public partial class TimeStopDependency
    {
    }

    internal static class UpdatableAndDeletableCWT
    {
        static ConditionalWeakTable<UpdatableAndDeletable, Data> table = new ConditionalWeakTable<UpdatableAndDeletable, Data>();

        public static Data GetCustomData(this UpdatableAndDeletable self)
        {
            // Пытаемся получить существующие данные
            if (table.TryGetValue(self, out Data data))
                return data;

            // Создаем новые данные с передачей self в конструктор
            data = new Data(self);
            table.Add(self, data);
            return data;
        }

        public class Data
        {
            public UpdatableAndDeletable Target { get; }

            internal bool isImmuneToTimeStop;
            internal bool checkedTimeStopImmunity = false;
            public Data(UpdatableAndDeletable target)
            {
                Target = target;
            }
        }
    }

    public static class UpdatableAndDeletableExtensions
    {
        /// <summary>
        /// Switches Time Stop Immunity State for selected UpdatableAndDeletable object.
        /// Grants immunity if object doesn't have it, and revokes immunity if object already has it.
        /// </summary>
        public static void SwitchTimeStopImmunityState(this UpdatableAndDeletable uAD)
        {
            uAD.GetCustomData().isImmuneToTimeStop = !uAD.GetCustomData().isImmuneToTimeStop;
        }
        /// <summary>
        /// Makes selected UpdatableAndDeletable object immune to Time Stop
        /// </summary>
        public static void GrantTimeStopImmunity(this UpdatableAndDeletable uAD)
        {
            uAD.GetCustomData().isImmuneToTimeStop = true;
        }
        /// <summary>
        /// Makes selected UpdatableAndDeletable object vulnerable to Time Stop
        /// </summary>
        public static void RevokeTimeStopImmunity(this UpdatableAndDeletable uAD)
        {
            uAD.GetCustomData().isImmuneToTimeStop = false;
        }

        /// <summary>
        /// Returns whenever or not selected UpdatableAndDeletable object has immunity to Time Stop
        /// </summary>
        public static bool IsTimeStopImmune(this UpdatableAndDeletable uAD)
        {
            return uAD.GetCustomData().isImmuneToTimeStop;
        }

        /// <summary>
        /// Returns whenever or not type of selected UpdatableAndDeletable object contains in TimeStopImmuneTypes list
        /// </summary>
        public static bool IsThisTypeImmune(this UpdatableAndDeletable uAD)
        {
            Type type = uAD.GetType();
            if (type == null) return false;
            return TimeStopDependency.TimeStopImmuneTypes.Contains(type);
        }
        
    }
}
