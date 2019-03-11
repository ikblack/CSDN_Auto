using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSDN_Auto
{
    public partial class Form1 : Form
    {
        public string serverAddr { get { return textBox2.Text; } }
        public string downloadDir { get { return textBox1.Text; } }
        public string account { get { return textBox5.Text; } }
        public string password { get { return textBox4.Text; } }

        public static Form1 Instance
        {
            get; private set;
        }

        public Form1()
        {
            Instance = this;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.ApplicationExit += Application_ApplicationExit;
            Rtc.Init();
            Rtc.onMessage += Rtc_onMessage;
        }

        private void Rtc_onMessage(Rtc.PipeMsg obj)
        {
            if (obj.fromUrl == "重启鸭子")
            {
                System.Diagnostics.Process.Start(Application.ExecutablePath);
                Application.Exit();
                return;
            }

            Downloader.AutoDownload(obj.fromUrl, (info) =>
            {
                obj.isDownloadSuccess = info.isSuccess;
                obj.downloadUrl = info.url;
                obj.downloadFileName = info.fileName;
                obj.dowloadErrorInfo = info.errMsg;
                Rtc.Send(obj);
            });
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            SeDo.DisposeAll();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Downloader.AutoDownload(textBox3.Text
            , (info) =>
            {
                if (info.isSuccess)
                    ALog(info.fileName + "\n" + info.url);
                else
                    ALog(info.errMsg);
            });
        }

        public delegate void SetControlDelegate1(string text);
        public void ALog(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new SetControlDelegate1((a) => { ALog(a); }), new object[] { text });
            }
            else
            {
                richTextBox1.AppendText(text + "\n");
                richTextBox1.ScrollToCaret();
            }
        }
    }
}
