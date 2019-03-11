using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDN_Auto
{
    public class Commenter
    {/*
        private static MainForm mainForm { get { return MainForm.Instance; } }
        private static void Sleep(float seconds)
        {
            System.Threading.Thread.Sleep((int)(seconds * 1000));
        }
        private static void ALog(string str)
        {
            CtrlForm.Instance.ALog(str);
        }

        public static void Comment()
        {
            //check need comment
            bool needComment = JsDo.HasText("<div id=\"dl_lock\" class=\"dl_popup\" style=\"display: block;");
            if (needComment)
            {
                //goto comment page
                ALog("comment begin");
                JsDo.ClickClass("goComment");
                Sleep(3);

                JsDo.SetValue("star", "4");
                ALog("comment stars");
                Sleep(1);

                string comm = DHelper.GetRandomComment();
                JsDo.SetInnerText("cc_body", comm);
                ALog("comment: " + comm);
                Sleep(1);

                JsDo.ClickClass("btn btn-sm btn-red");

                ALog("submit comment");
                Sleep(2);

                mainForm.CloseActiveTab();
                Sleep(2);

                //close popup
                JsDo.ClickClass("fa fa-close pop_close", "<div id=\"dl_lock\" class=\"dl_popup\" style=\"display: block;");
                Sleep(1);

                //retry download
                JsDo.ClickClass("direct_download");
                ALog("retry download");
                Sleep(1);
            }

        }
        */
    }
}
