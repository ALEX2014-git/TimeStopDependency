using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeStopDependency
{
    public partial class TimeStopDependency
    {
        private void Weapon_Grabbed(On.Weapon.orig_Grabbed orig, Weapon self, Creature.Grasp grasp)
        {
            orig(self, grasp);
            var grabber = grasp.grabber;
            if (grabber == null) return;
            if (grabber is Player)
            {
            self.GetCustomData().isImmuneToTimeStop = true;
            }
        }
    }
}
