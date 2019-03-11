using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDN_Auto
{
    public static class TaskDo
    {
        public static void Execute(Action callback)
        {
            var t = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                try
                {
                    callback();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }));
            t.TrySetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
        }

        public static void WaitFor(float seconds, Action callback)
        {
            var t = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                try
                {
                    System.Threading.Thread.Sleep((int)(seconds * 1000));
                    callback();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }));
            t.TrySetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
        }
    }
}
