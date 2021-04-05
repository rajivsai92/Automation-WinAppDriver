using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using System;
using System.Diagnostics;
using System.Threading;


namespace AutomationFramework.Driver
{
    public class WindowsFunctions
    {
        // All window functions, switching


        private const string windowsApplicationDriverUrl = "http://127.0.0.1:4723/";
        public  static WindowsDriver<WindowsElement> winSession;


        /// <summary>
        /// Used to Activate or switch to new window
        /// </summary>
        /// <param name="WindowName"></param>
        /// <param name="counter"></param>
        public void ActivateWindowSession(string WindowName, int commandTimeOut=180,int implictWait=180, int counter = 20)
        {   

            try
            {
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", "Root");
                appCapabilities.SetCapability("deviceName", "WindowsPC");
                ///appCapabilities.AddAdditionalCapability("ms:waitForAppLaunch", "50");
                winSession = new WindowsDriver<WindowsElement>(new Uri(windowsApplicationDriverUrl), appCapabilities);

                var processlist = winSession.WindowHandles;
                //winSession.SwitchTo().Window(winSession.WindowHandles[0]);

                while (winSession.FindElementByName(WindowName) == null)
                { Thread.Sleep(1000); }
                var y = winSession.FindElementByName(WindowName);
                var yy = y.GetAttribute("NativeWindowHandle");
                appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("appTopLevelWindow", int.Parse(yy).ToString("x"));
                winSession = new WindowsDriver<WindowsElement>(new Uri(windowsApplicationDriverUrl), appCapabilities,TimeSpan.FromSeconds(commandTimeOut));
                winSession.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(implictWait);

            }

            catch
            {

                if (counter != 0)
                {
                    int i = counter - 1;
                    ActivateWindowSession(WindowName, i);
                }

            }

        }



        /// <summary>
        /// Used to click on allow pop up
        /// </summary>
        /// <param name="pageTitle"></param>
        public void ClickOnAllowButton(string pageTitle)
        {
            try
            {
                StartWinAppDriver();
                ActivateWindowSession(pageTitle,5,5);
                var WindowPopup = winSession.FindElementByClassName("IEFrame");
                WindowPopup.FindElementByName("Allow").Click();
            }
            catch (Exception e)
            {

            }

           winSession.Quit();
           CloseApplication("WinAppDriver");

        }


        /// <summary>
        /// Used to launch WinAppDriver.exe
        /// </summary>

        public void StartWinAppDriver()
        {
            Process.Start(@"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe");
            Thread.Sleep(5000);
        }


        public void CloseApplication(string window)
        {

            Process[] p1 = Process.GetProcesses();
            foreach (Process p in p1)
            {
                if (p.ProcessName.Contains(window))
                    p.Kill();
            }
        }



    }
}
