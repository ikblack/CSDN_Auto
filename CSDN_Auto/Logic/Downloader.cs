using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSDN_Auto
{
    public class Downloader : HelperBase
    {
        private DateTime timeStartup;
        private SeDo se = null;
        private string url;
        private string id;
        private Action<DownloadInfo> callback;

        public Downloader()
        {
            timeStartup = DateTime.Now;
        }

        public void Init(string url, Action<DownloadInfo> callback)
        {
            this.url = url;
            this.id = UrlToId(url);
            this.callback = callback;
        }

        public bool IsOutDate()
        {
            return (DateTime.Now - timeStartup).TotalSeconds > 180;
        }

        public string GetHtml(string URL)
        {
            WebRequest wrt;
            wrt = WebRequest.Create(URL);
            wrt.Credentials = CredentialCache.DefaultCredentials;
            WebResponse wrp;
            wrp = wrt.GetResponse();
            string reader = new StreamReader(wrp.GetResponseStream(), Encoding.GetEncoding("gb2312")).ReadToEnd();
            try
            {
                wrt.GetResponse().Close();
            }
            catch (WebException ex)
            {
                throw ex;
            }
            return reader;
        }

        private bool Validate()
        {
            if (!url.StartsWith("https://download.csdn.net/download/")
                && !url.StartsWith("http://download.csdn.net/download/"))
            {
                Fail("错误的下载地址，请核对！");
                return false;
            }

            try
            {
                var html = GetHtml(url);
                if (html.Contains("<div class=\"error_text\">404</div>"))
                {
                    Fail("404 Not Found!  o(╥﹏╥)o");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("404"))
                    Fail("404 Not Found!  o(╥﹏╥)o");
                else
                    Fail(ex.Message);
                return false;
            }
        }



        static Dictionary<string, Downloader> m_dict = new Dictionary<string, Downloader>();

        private static Downloader GetDownloader(string id)
        {
            if (!m_dict.ContainsKey(id))
                m_dict.Add(id, new Downloader());
            return m_dict[id];
        }

        private static void Release(string id)
        {
            SeDo.Release(id);
            if (!m_dict.ContainsKey(id))
                return;
            m_dict.Remove(id);
        }

        private static bool ExistDownloader(string id)
        {
            return m_dict.ContainsKey(id);
        }

        private static string UrlToId(string url)
        {
            string id = url.TrimEnd().GetAfterLast("/");
            if (id.Contains("?"))
                id = id.GetBefore("?");
            return id;
        }

        private bool IsDownloaded()
        {
            var fileName = GetDownloadName(id);
            var zipPath = GetZipPath(id);
            if (File.Exists(zipPath) && !string.IsNullOrEmpty(fileName))
            {
                Success(fileName, id);
                return true;
            }
            return false;
        }

        private bool UseSeDo()
        {
            try
            {
                se = SeDo.Get(id);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("任务队列已满")) Fail(e.Message);
                else Fail("打开Selenium时出错~\n" + e.Message);
                return false;
            }
            return true;
        }

        private void Download()
        {
            if (!Validate())
            {
                Release(id);
                return;
            }
            if (IsDownloaded())
            {
                Release(id);
                return;
            }
            if (!UseSeDo())
            {
                Release(id);
                return;
            }

            // INFO
            Info("开始下载，请稍等...");


            Login.AutoLogin(se, (isLogin) =>
            {
                if (!isLogin)
                {
                    Fail("登录账号失败了  o(╥﹏╥)o");
                    Release(id);
                    return;
                }

                TaskDo.Execute(() =>
                {
                    try
                    {
                        //load
                        se.GoToUrl(url);
                        ALog("load -> " + url);
                        Sleep(3);
                        //cost
                        var downloadCost = -1;
                        var s_cost = se.FindElement(By.XPath("//div[@class='dl_download_box dl_download_l']/label/em")).Text;
                        int.TryParse(s_cost, out downloadCost);
                        Sleep(1);
                        ALog("cost " + downloadCost);
                        if (downloadCost < 0)
                        {
                            Fail("获取下载消耗C币失败  o(╥﹏╥)o");
                            return;
                        }

                        //check cost
                        var vipBtn = se.FindClass("direct_download vip_download vip_down");
                        if (vipBtn != null)
                        {
                            ALog("click download");
                            vipBtn.Click();
                            Sleep(3);
                        }
                        else
                        {
                            ALog("click download");
                            se.FindClass("direct_download").Click();
                            Sleep(3);
                        }

                        //confirm download
                        // < !--3.当前用户已登录，非VIP用户，需积分且积分足够： 2002-- > 
                        if (CheckClickPopWindow(se, "noVipEnoughP", "确认下载")) ;
                        // <!--已经下载过该资源，不再扣除积分的弹框-->
                        else if (CheckClickPopWindow(se, "download", "下载")) ;
                        // <!--2.当前用户已登录，VIP用户，积分不论：  2005-->
                        else if (CheckClickPopWindow(se, "vipIgnoreP", "VIP下载")) ;

                        WaitForDownload(se, id);

                        string dName = GetDownloadName(id);

                        var path = Form1.Instance.downloadDir + "/" + id + "/" + dName;
                        var desPath = GetZipPath(id);
                        if (Ziper.ZipFile(path, desPath))
                        {
                            ALog("压缩完成！");
                        }
                        else
                        {
                            Fail("压缩文件时候出错了 o(╥﹏╥)o" + id);
                        }

                        if (dName != null)
                            Success(GetDownloadName(id), id);
                        else
                            Fail("我下载到哪里去了？找不到了... " + id);
                    }
                    catch (Exception e)
                    {
                        Fail("发生什么了？" + "\n" + e.Message);
                    }
                    finally
                    {
                        Release(id);
                    }
                });
            });
        }

        public static void AutoDownload(string url, Action<DownloadInfo> callback)
        {
            var id = UrlToId(url);
            if (ExistDownloader(id))
            {
                if (GetDownloader(id).IsOutDate())
                {
                    Release(id);
                    AutoDownload(url, callback);
                }
                else
                {
                    var _down = new Downloader();
                    _down.Init(url, callback);
                    _down.Fail("别着急，资源正在下载中...");
                }
                return;
            }

            var down = GetDownloader(id);
            down.Init(url, callback);
            down.Download();
        }

        private string GetZipPath(string id)
        {
            return Form1.Instance.downloadDir + "/" + id + ".zip";
        }

        private string GetDownloadName(string id)
        {
            DirectoryInfo di = new DirectoryInfo(Form1.Instance.downloadDir + "/" + id);
            if (di.Exists && di.GetFiles().Length > 0)
                return di.GetFiles()[0].Name;
            return null;
        }

        private void WaitForDownload(SeDo se, string id)
        {
            int count = 0;
            long lastSize = 0;

            while (true)
            {
                DirectoryInfo di = new DirectoryInfo(Form1.Instance.downloadDir + "/" + id);
                var files = di.GetFiles();
                if (files.Length == 0)
                    count++;
                else if (GetDownloadName(id).EndsWith(".crdownload"))
                    count++;
                else if (new FileInfo(files[0].FullName).Length == lastSize)
                    count += 30;
                else
                    count = 0;
                if (count > 100) return;
                lastSize = new FileInfo(files[0].FullName).Length;
                Console.WriteLine("download " + count + "   " + lastSize);
                Sleep(1);
            }
        }

        private bool CheckClickPopWindow(SeDo se, string id, string btnText)
        {
            var wind = se.FindId(id);
            if (wind != null && wind.GetAttribute("style").Contains("display: block;"))
            {
                var btn = wind.FindElement(By.XPath("//*[@id='" + id + "']//a[text()='" + btnText + "']"));
                btn.Click();
                ALog("Click -> " + id + "   " + btnText);
                Sleep(3);
                return true;
            }
            return false;
        }

        private string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            byte[] byteArray = dataEncode.GetBytes(paramData); //转化
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
            webReq.Method = "POST";
            webReq.ContentType = "application/json; charset=UTF-8";

            webReq.ContentLength = byteArray.Length;
            Stream newStream = webReq.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);//写入参数
            newStream.Close();
            HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
            ret = sr.ReadToEnd();
            sr.Close();
            response.Close();
            newStream.Close();
            return ret;
        }

        public class DownloadInfo
        {
            public bool isSuccess;
            public string fileName;
            public string url;
            public string errMsg;
        }

        private void Fail(string msg)
        {
            DownloadInfo info = new DownloadInfo();
            info.isSuccess = false;
            info.errMsg = msg;
            if (callback != null)
                callback(info);
            else
                callback(info);
        }

        private void Info(string msg)
        {
            DownloadInfo info = new DownloadInfo();
            info.isSuccess = false;
            info.errMsg = msg;
            if (callback != null)
                callback(info);
            else
                callback(info);
        }

        private void Success(string fileName, string id)
        {
            DownloadInfo info = new DownloadInfo();
            info.isSuccess = true;
            info.fileName = fileName;
            info.url = Form1.Instance.serverAddr + "/" + id + ".zip";
            if (callback != null)
                callback(info);
            else
                callback(info);
        }

    }
}
