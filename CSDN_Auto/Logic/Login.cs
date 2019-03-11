using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDN_Auto
{
    public class Login : HelperBase
    {
        public static void ForceLogin(SeDo se, Action<bool> callback)
        {
            AutoLogin(se,callback, true);
        }

        public static void AutoLogin(SeDo se, Action<bool> callback, bool forceLogin = false)
        {
            TaskDo.Execute(() =>
            {
                try
                {
                    if (forceLogin)
                    {
                        se.GoToUrl("https://passport.csdn.net/account/logout");
                        Sleep(1);
                        se.GoToUrl("https://passport.csdn.net/login");
                    }
                    else
                    {
                        se.GoToUrl("https://i.csdn.net");
                    }
                    Sleep(2);
                    if (forceLogin || !se.CurUrl().StartsWith("https://i.csdn.net"))
                    {

                        if (string.IsNullOrEmpty(Form1.Instance.account)
                            || string.IsNullOrEmpty(Form1.Instance.password))
                        {
                            throw new Exception("账号或密码为空");
                        }

                        se.FindText("帐号登录").Click();
                        Sleep(0.5f);
                        se.FindId("all").Click();
                        Sleep(0.5f);
                        se.FindId("all").SendKeys(Form1.Instance.account);
                        Sleep(0.5f);
                        se.FindClass("main-login").Click();
                        Sleep(0.5f);
                        se.FindId("password-number").Click();
                        Sleep(0.5f);
                        se.FindId("password-number").SendKeys(Form1.Instance.password);
                        Sleep(0.5f);
                        se.FindClass("main-login").Click();
                        Sleep(0.5f);
                        se.FindClass("btn btn-primary").Click();
                        Sleep(2);
                        se.GoToUrl("https://i.csdn.net");
                        Sleep(2);
                        if (se.CurUrl().StartsWith("https://i.csdn.net"))
                            callback.Invoke(true);
                        else
                            callback.Invoke(false);
                    }
                    else
                    {
                        callback.Invoke(true);
                    }
                }
                catch (Exception e)
                {
                    ALog(e.Message);
                    callback.Invoke(false);
                }
            });
        }
    }
}
