using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeStopDependency
{
    public partial class TimeStopDependency
    {
        private void PhysicalObject_Update(On.PhysicalObject.orig_Update orig, PhysicalObject self, bool eu)
        {
            orig(self, eu);
            bool objectTimeStopImmunity = false;
            if (self.grabbedBy.Count > 0)
            {
                for (int i = 0; i < self.grabbedBy.Count; i++)
                {

                    var grabber = self.grabbedBy[i].grabber;
                    if (grabber != null)
                    {
                        if (grabber.GetCustomData().isImmuneToTimeStop)
                        {
                            objectTimeStopImmunity = true;
                            break;
                        }
                    }
                }
            }
            self.GetCustomData().isImmuneToTimeStop = (self.GetCustomData().isImmuneToTimeStop || objectTimeStopImmunity);
        }
    }
}
