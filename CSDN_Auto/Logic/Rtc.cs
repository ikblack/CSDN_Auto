using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDN_Auto
{
    public class Rtc
    {
        public static event Action<PipeMsg> onMessage = null;
        private static PipeChanel.Pipechanel pipeserver;

        public static void Init()
        {
            pipeserver = new PipeChanel.Pipechanel(2, "PipeServer", "PipeClient");
            PipeChanel.Pipechanel.msgReceived += new PipeChanel.PipeMsg.PipeMsgEventHandler(Pipechanel_msgReceived);
        }

        public class PipeMsg
        {
            public int fromQQType;
            public long fromGroup;
            public long fromQQ;
            public string fromUrl;
            public bool isDownloadSuccess;
            public string downloadFileName;
            public string downloadUrl;
            public string dowloadErrorInfo;
        }

        private static void Pipechanel_msgReceived(object sender, PipeChanel.PipeMsg.PipeMsgEventArgs e)
        {
            PipeMsg msg = JsonConvert.DeserializeObject<PipeMsg>(e.receivedMsg);
            onMessage?.Invoke(msg);
        }

        public static void Send(PipeMsg msg)
        {
            string s_msg = JsonConvert.SerializeObject(msg);
            pipeserver.Send(s_msg);
        }
    }
}
