using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Configuration;
using OpenQA.Selenium;
using System.Diagnostics;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading;

namespace AutomationFramework.Utilities
{
    public static class CommonFunctions
    {
        
        /// <summary>
        /// Returns the fully qualified path of a folder inside the project
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static string GetProjectPathOfFolder(string folderName)
        {
            string path = string.Empty;
            try
            {

                string pro = (new FileInfo(AppDomain.CurrentDomain.BaseDirectory)).Directory.Parent.Parent.Parent.FullName;

                string projectDir = (new FileInfo(AppDomain.CurrentDomain.BaseDirectory)).Directory.Parent.Parent.FullName;
                //string projectDir = ConfigurationManager.AppSettings["ProjectPath"];

                path = projectDir + @"\" + folderName + @"\";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return path;
        }


        public static String GetProjectPackageFolderPath()
        {

            string path = null;
            string projectDir = (new FileInfo(AppDomain.CurrentDomain.BaseDirectory)).Directory.Parent.Parent.Parent.FullName;            
           return path = projectDir + @"\packages\";
        }


        public static String GetCurrentProjectDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory.ToString().Split('\\')[0];
        }



        public static String CalculateBusinessDays(int NoOfDays)
        {
            String FinalDate = string.Empty;
            List<DateTime> HolidaysList = new List<DateTime>();
            HolidaysList.Add(DateTime.Parse("01/01/2020"));
            HolidaysList.Add(DateTime.Parse("02/17/2020"));
            HolidaysList.Add(DateTime.Parse("05/25/2020"));
            HolidaysList.Add(DateTime.Parse("07/04/2020"));
            HolidaysList.Add(DateTime.Parse("09/07/2020"));
            HolidaysList.Add(DateTime.Parse("11/11/2020"));
            HolidaysList.Add(DateTime.Parse("11/26/2020"));
            HolidaysList.Add(DateTime.Parse("12/25/2020"));
            HolidaysList.Add(DateTime.Parse("12/24/2020"));
            string Day = DateTime.Today.DayOfWeek.ToString();
            int i = 0;
            while (NoOfDays > 0)
            {
                i++;
                Day = DateTime.Today.AddDays(i).DayOfWeek.ToString();
                if (Day != "Sunday" && Day != "Saturday" && !HolidaysList.Contains(DateTime.Today.AddDays(i)))
                {
                    NoOfDays--;
                }
            }
            return DateTime.Today.AddDays(i).ToString("MM/dd/yyyy");

        }


        public static string GetCurrentDate(string Zone = "India Standard Time", string format = "MM/dd/yyyy")
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(Zone)).ToString(format);
        }





        /// <summary>
        /// Used to Generate Task action comments 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="taskAction"></param>
        /// <param name="policyNumber"></param>
        /// <returns></returns>
        public static string GenerateTaskActionComments(string serviceType, string taskAction, int policyNumber = 1)
        {
            string taskActionComment = string.Format("Policy {0}-Comments for {1} performing {2} | {3}", policyNumber, serviceType, taskAction, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            return taskActionComment;
        }

        public static string AdditionalComments(string serviceType)
        {
            string taskActionComment = string.Format("Additional comments for {0} | {1}", serviceType, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            return taskActionComment;
        }



        /// <summary>
        /// Used to generate memo text
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string GenerateMemoText(string txt="Memo_")
        {
            string memoText = string.Format(txt+ DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            return memoText;
        }


        /// <summary>
        /// Used to generate random string
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string GenerateRandom(string txt = "Random")
        {
            Random rad = new Random();            
            string randomText = txt+"_"+ rad.Next(100000, 1000000000);
            return randomText;
        }

        public static string GetRandomString()
        {
            return DateTime.Now.ToString("ddHHmmssZ");
        }

        public static void KillProcess(string name)
        {
            var runningProcesses = Process.GetProcesses();
            foreach (var process in runningProcesses)
            {
                try
                {
                    if (process.ProcessName == name)
                    {
                        //Reports.Log("Killing Process : " + name);
                        process.Kill();
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public static Process ExecuteBatchFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Assert.Fail("Could not find batch file : " + filePath);
            }

            return Process.Start(filePath);
        }
        public static string GetCurrentTestName()
        {
            string TestName = "Test";
            try
            {
                TestName = TestContext.CurrentContext.Test.FullName;
                if (string.IsNullOrEmpty(TestName))
                {
                    TestName = NUnit.Framework.TestContext.CurrentContext.Test.Name;
                }
            }
            catch (Exception e)
            {
                TestName = "Test";
            }
            TestName = Regex.Replace(TestName, "[^a-zA-Z0-9%._]", string.Empty);
            return TestName;
        }
        public static string GetShortTestName(int length)
        {
            var name = GetCurrentTestName();
            name = name.Replace("/", "_");
            name = name.Replace(":", "_");
            name = name.Replace("\\", "_");
            name = name.Replace("\"", "");
            name = name.Replace(" ", "");
            if (name.Length > length)
            {
                name = name.Substring((name.Length - length), length);
            }

            return name;
        }
        public static string GetCodeDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug", "").Replace(@"\bin\Release", "");
        }
        public static void Delay(int delayMs)
        {
            if (delayMs > 0)
            {
                Thread.Sleep(delayMs);
            }
        }

        public static string Encode(string s)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
        }

        public static string Decode(string s)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(s));
        }

    }
}
