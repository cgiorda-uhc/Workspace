using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace WebParser
{
    public static class WebParser
    {
        static IWebDriver driver;



        [Test]
        public static async void cssDemo()
        {
            driver = new ChromeDriver(@"C:\Users\cgiorda\Documents\chrome");
            driver.Url = "https://dbvrp78212.uhc.com:5601/login?next=%2F#?_g=()";
            driver.Url = "https://dbvrp78212.uhc.com:5601/app/kibana#/dashboard/5d0b16b0-b73c-11ea-a012-dd49272c1481?_g=(refreshInterval:(pause:!t,value:0),time:(from:'2022-01-01T06:00:00.000Z',to:'2022-01-31T06:00:00.000Z'))&_a=(description:'',filters:!(),fullScreenMode:!f,options:(hidePanelTitles:!f,useMargins:!t),panels:!((embeddableConfig:(),gridData:(h:15,i:'49608a48-783b-4e05-851d-e38fc424b078',w:24,x:0,y:0),id:ed94f150-b594-11ea-a012-dd49272c1481,panelIndex:'49608a48-783b-4e05-851d-e38fc424b078',type:visualization,version:'7.4.2'),(embeddableConfig:(),gridData:(h:15,i:'021ee3f3-f783-4242-8d5f-cdd7f5b1997d',w:24,x:24,y:0),id:e5d9da20-5689-11eb-a676-17f4e0b1da9d,panelIndex:'021ee3f3-f783-4242-8d5f-cdd7f5b1997d',type:visualization,version:'7.4.2'),(embeddableConfig:(),gridData:(h:13,i:e36ab862-c788-4657-b893-df7a452625a6,w:48,x:0,y:30),id:'7ac52ca0-b58d-11ea-a012-dd49272c1481',panelIndex:e36ab862-c788-4657-b893-df7a452625a6,type:search,version:'7.4.2'),(embeddableConfig:(mapCenter:!n,mapZoom:!n),gridData:(h:15,i:'88c195b4-d92e-4724-b3a1-b8188cf87553',w:24,x:0,y:15),id:b95559f0-5690-11eb-a676-17f4e0b1da9d,panelIndex:'88c195b4-d92e-4724-b3a1-b8188cf87553',type:visualization,version:'7.4.2')),query:(language:kuery,query:''),timeRestore:!f,title:'EDC%20Analytics%20Dashboard',viewMode:view)";

            driver.Manage().Window.Maximize();


            //CHRIS ADDED TIMEOUT EXTENSION METHOD
            //IWebElement passwordTextBox = driver.FindElement(By.XPath("//*[@data-test-subj='loginPassword']"), 30);
            //VS:
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);


            //LOGIN
            IWebElement usernameTextBox = driver.FindElement(By.XPath("//*[@data-test-subj='loginUsername']"));
            IWebElement passwordTextBox = driver.FindElement(By.XPath("//*[@data-test-subj='loginPassword']"));
            IWebElement loginButton = driver.FindElement(By.XPath("//*[@data-test-subj='loginSubmit']"));
            usernameTextBox.SendKeys("cgiorda");
            passwordTextBox.SendKeys("cgggmapuser6!");
            loginButton.Click();

            //LINK POPUP
            IWebElement ellipseButton = driver.FindElement(By.XPath("/html/body/div[1]/div/div[3]/div/div[2]/dashboard-app/div[2]/div/div/div[4]/div/div/div[1]/div[2]/div/button"));
            ellipseButton.Click();


            //CLICK DOWNLOAD CSV LINK
            IWebElement clickDownloadSpan = driver.FindElement(By.XPath("//*[@data-test-subj='embeddablePanelAction-downloadCsvReport']"));
            clickDownloadSpan.Click();

            //WAIT UNTIL FILE EXISTS
            string strFile = GetDownloadsPath() + "\\" + "EDC Claims wProcName.csv";
            SpinWait.SpinUntil(() => File.Exists(strFile) == true, 400000);

            //PROCESS FILE??????????
            //PROCESS FILE??????????
            //PROCESS FILE??????????
            File.Delete(strFile);


            //https://www.guru99.com/execute-javascript-selenium-webdriver.html
            //https://www.lambdatest.com/blog/scraping-dynamic-web-pages/?utm_source=twitter&utm_medium=blog&utm_campaign=OrganicPosting&utm_term=cpage300321
            //var timeout = 10000; /* Maximum wait time of 10 seconds */
            //var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
            //wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            //Thread.Sleep(5000);
            //IWebElement clickDownloadSpan = wait.Until(ExpectedConditions.ElementExists(By.XPath("//html/body/div[5]/div/div[3]/div/div[2]/div/div/div[2]/div/button[3]/span/span")));
            //driver.SwitchTo().Frame(0);
            //driver.SwitchTo().Window(driver.WindowHandles[1]);
            //List<IWebElement> buttons = new List<IWebElement>(driver.FindElements(By.TagName("button")));


        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetKnownFolderPath(ref Guid id, int flags, IntPtr token, out IntPtr path);
        private static Guid FolderDownloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");
        public static string GetDownloadsPath()
        {
            if (Environment.OSVersion.Version.Major < 6) throw new NotSupportedException();

            IntPtr pathPtr = IntPtr.Zero;

            try
            {
                SHGetKnownFolderPath(ref FolderDownloads, 0, IntPtr.Zero, out pathPtr);
                return Marshal.PtrToStringUni(pathPtr);
            }
            finally
            {
                Marshal.FreeCoTaskMem(pathPtr);
            }
        }




        [SetUp]
        public static void startBrowser()
        {
            driver = new ChromeDriver(@"C:\Users\cgiorda\chromedriver_win32\chrome");
        }

        [Test]
        public static void test()
        {
            driver.Url = "http://www.google.co.in";
        }

        [TearDown]
        public static void closeBrowser()
        {
            driver.Close();
        }

    }

}
