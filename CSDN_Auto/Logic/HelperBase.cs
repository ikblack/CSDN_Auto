using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDN_Auto
{
    public class HelperBase
    {
        protected static void Sleep(float seconds)
        {
            System.Threading.Thread.Sleep((int)(seconds * 1000));
        }

        public static void ALog(string text)
        {
            Form1.Instance.ALog(text);
        }
    }
}
