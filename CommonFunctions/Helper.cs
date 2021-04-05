using AutomationFramework.Driver;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using System.Collections.ObjectModel;

namespace AutomationFramework.Utilities
{
    public class Helper
    {
        #region web

        /// <summary>
        /// Used to get GetbyObject with appropriate locator
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static By GetBy(string locator)
        {
            string[] parts = locator.Split(new char[] {'>'},2);
            By by = null;
            switch (parts[0].Trim())
            {

                case "Xpath":
                    by = By.XPath(parts[1]);
                    break;

                case "ClassName":
                    by = By.ClassName(parts[1]);
                    break;

                case "CssSelector":
                    by = By.CssSelector(parts[1]);
                    break;

                case "Id":
                    by = By.Id(parts[1]);
                    break;

                case "LinkText":
                    by = By.LinkText(parts[1]);
                    break;

                case "Name":
                    by = By.Name(parts[1]);
                    break;

                case "PartialLinkText":
                    by = By.PartialLinkText(parts[1]);
                    break;

                case "TagName":
                    by = By.TagName(parts[1]);
                    break;              

            }

            return by;
        }


        /// <summary>
        /// Used to Get Web Element of type IWebElement
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static IWebElement GetWebElement(string locator)
        {
            Waits.WaitForWebElementExists(locator);

            return WebFunctions.webSession.FindElement(Helper.GetBy(locator));
        }

        /// <summary>
        ///  Used to Get Web Elements of type IWebElement
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static ReadOnlyCollection<IWebElement> GetWebElements(string locator)
        {
            Waits.WaitForWebElementExists(locator);

            return WebFunctions.webSession.FindElements(Helper.GetBy(locator));
        }

       

        /// <summary>
        /// Switch to Web windows based on title of window 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="windowName"></param>
        /// <returns></returns>
        public static bool SwitchToWebPage(string windowName)
        {
            bool isPresent = false;
            var titles = WebFunctions.webSession.WindowHandles;
            foreach (var item in titles)
            {
                WebFunctions.webSession.SwitchTo().Window(item);
                if (WebFunctions.webSession.Title.Equals(windowName))
                {
                    isPresent = true; break;
                }
            }

            return isPresent;
        }


        #endregion

        #region Windows

        /// <summary>
        /// Used to get Windows element of type WindowsElement
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="isAccessibiltyId"></param>
        /// <returns></returns>
        public static WindowsElement GetWinElement(string locator, bool isAccessibiltyId = false)
        {
            if (isAccessibiltyId)
                return WindowsFunctions.winSession.FindElementByAccessibilityId(locator);         

            else    
                return WindowsFunctions.winSession.FindElement(Helper.GetBy(locator));
            

        }

        public static ReadOnlyCollection<WindowsElement> GetWinElements(string locator, bool isAccessibiltyId = false)
        {
            if (isAccessibiltyId)
                return WindowsFunctions.winSession.FindElementsByAccessibilityId(locator);

            else
                return WindowsFunctions.winSession.FindElements(Helper.GetBy(locator));

        }

        #endregion 

    }
}
