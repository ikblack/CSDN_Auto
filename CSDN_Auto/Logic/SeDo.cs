using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium;
using System.IO;
using System.Runtime.InteropServices;

namespace CSDN_Auto
{
    public class SeDo
    {
        private static Dictionary<string, SeDo> dict = new Dictionary<string, SeDo>();
        public static SeDo Get(string id)
        {
            if (!dict.ContainsKey(id))
            {
                SeDo se = new SeDo(id);
                dict.Add(id, se);
            }
            return dict[id];
        }

        public static void Release(string id)
        {
            Form1.Instance.ALog("释放: " + id);
            if (!dict.ContainsKey(id))
                return;
            dict[id].Dispose();
            dict.Remove(id);
        }

        public static void DisposeAll()
        {
            foreach (var kv in dict)
            {
                kv.Value.Dispose();
            }

            dict.Clear();
        }

        public ChromeDriver driver = null;
        string saveDir = "";
        string driverDir = "";
        string optionDir = "";
        string id = "";
        public const int MAX_OPTIONS = 10;


        private string GetOptions()
        {
            if (!Directory.Exists("tmp_options"))
                Directory.CreateDirectory("tmp_options");

            foreach (var di in Directory.GetDirectories("tmp_options/"))
            {
                var didir = "tmp_options/" + Path.GetFileName(di);
                if (dict.Values.All(a => a.optionDir != didir))
                    return didir;
            }

            if (Directory.GetDirectories("tmp_options/").Length < MAX_OPTIONS)
            {
                return "tmp_options/" + (Directory.GetDirectories("tmp_options/").Length + 1);
            }

            throw new Exception(string.Format("任务队列已满({0})，请稍等...", dict.Count));
        }

        private SeDo(string id)
        {
            this.id = id;
            this.saveDir = Form1.Instance.downloadDir + "/" + id;
            this.driverDir = "tmp_drivers/" + id;
            this.optionDir = GetOptions();
            FileHelper.CreateDir(driverDir, "WebDriver");
            FileHelper.CreateDir(saveDir);
            Form1.Instance.ALog("Use Driver : " + this.driverDir + "\nUse Option : " + this.optionDir);
            Init();
        }

        private void Init()
        {
            var options = new ChromeOptions();
            options.AddArgument("--safebrowsing-disable-download-protection");
            options.AddArgument("--safebrowsing-disable-extension-blacklist");
            options.AddArgument("--disable-infobars");
            options.AddArgument("user-data-dir=" + new FileInfo(optionDir).FullName);
            options.AddUserProfilePreference("disable-popup-blocking", false);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("download.directory_upgrade", true);
            options.AddUserProfilePreference("download.default_directory", new FileInfo(saveDir).FullName);
            options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
            options.AddUserProfilePreference("safebrowsing.enabled", true);

            // chromedriver 路径 （不要配置servicePort，自动分配就可以，跟DebuggerAddress 没有任何关系！）
            var service = ChromeDriverService.CreateDefaultService(driverDir);
            driver = new ChromeDriver(service, options);
        }

        public void GoToUrl(string url)
        {
            if (driver == null)
                return;
            driver.Navigate().GoToUrl(url);
        }

        public void Dispose()
        {
            if (driver != null)
                driver.Quit();
            System.Threading.Thread.Sleep(1000);
            FileHelper.Delete(driverDir);
        }

        public IWebElement FindText(string text)
        {
            var el = FindElement(By.XPath("//*[text()='" + text + "']"));
            return el;
        }

        public IWebElement FindId(string id)
        {
            var el = FindElement(By.Id(id));
            return el;
        }

        public IWebElement FindName(string name)
        {
            var el = FindElement(By.Name(name));
            return el;
        }

        public IWebElement FindClass(string css)
        {
            var el = FindElement(By.XPath("//*[@class='" + css + "']"));
            return el;
        }

        public IWebElement FindElement(By by, int timeoutInSeconds = -1)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv =>
                {
                    var _els = drv.FindElements(by);
                    return _els.Count > 0 ? _els[0] : null;
                });
            }
            var els = driver.FindElements(by);
            return els.Count > 0 ? els[0] : null;
        }

        public string CurUrl()
        {
            return driver.Url;
        }
    }
}
