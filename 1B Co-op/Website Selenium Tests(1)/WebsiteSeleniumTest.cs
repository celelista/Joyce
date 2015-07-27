using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests
{
	class WebsiteSeleniumTest
	{
		static ChromeDriver driver;
		static WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(9000));

		static void Main()
		{
			Test1();
		}

		private static void Test1()
        {
			driver = new ChromeDriver("C:\\Users\\User\\Desktop\\SeleniumTests\\chromedriver_win32\\");
			var stopwatch = new Stopwatch();
			
			stopwatch.Start();
			//going to the website
			driver.Navigate().GoToUrl("http://mirasan.ca");
			stopwatch.Stop();
			//returns how long it took for the page to load
			Console.WriteLine("The webpage took " + stopwatch.ElapsedMilliseconds + " miliseconds to load");
			
			stopwatch.Start();

			driver.FindElement(By.Id("LayoutHeaderSearchBox")).SendKeys("don mills");
			driver.FindElement(By.Id("LayoutHeaderSearchBox")).SendKeys(Keys.Enter);

			var timeout = new Stopwatch();
			timeout.Start();

			// waits until the results come up
			wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("aniSpriteLoadingBarSmall")));
			wait.Until<bool>(x =>
			{
				var element = x.FindElement(By.Id("LayoutHeaderSearchStatus"));
				return element.Text != "Loading" && element.Text != "Showing recently-accessed devices"
					&& element.Text != "No results";
			});

			//returns the time it took for the search
			timeout.Stop();
			Console.WriteLine("The search took: " + timeout.ElapsedMilliseconds);

			// click on dropdown menu and wait for a popup
			driver.FindElement(By.XPath("//*[@id='LayoutHeaderSearchResults']/div[2]")).Click();
			wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[5]/div[2]/a")));
			
			// checks to see if there is a "More Information" button
			if (driver.FindElements(By.PartialLinkText("More Information")).Count > 0)
			{
				driver.FindElement(By.PartialLinkText("More Information")).Click();
				wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("infoPageTitleText")));

				string type;
				//figures out what kind of information we are dealling with and acts accordingly
				ActOnElement(out type);
			}

			else
			{
				//zoom in on map	
			}
			stopwatch.Stop();
			Console.WriteLine("This test took: " + stopwatch.ElapsedMilliseconds);

			driver.Quit();
        }

		//This helper function figures out the kind of element we are dealing with then runs test accordingly
		private static void ActOnElement(out string type)
		{
			var element = driver.FindElement(By.XPath("//*[@id='LayoutBody']/div[1]/a[3]"));
			string text;
			switch (element.Text)
			{
				case "POI Details":
				case "Signal Details":
				case "Vehicle Details":
				{
					Console.WriteLine("This is either a POI, Signal or Vehicle element.");
					
					driver.FindElementByLinkText("Comments").Click();
					wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='CommentsPanel']/div")));
					text = driver.FindElement(By.XPath("//*[@id='CommentsPanel']/div")).Text;
					Console.WriteLine("The comments associated with the element are: " + text);
					

					driver.FindElementByLinkText("Issues").Click();
					wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='AssociatedIssueListTablePager']/div[1]")));
					text = driver.FindElement(By.XPath("//*[@id='AssociatedIssueListTablePager']/div[1]")).Text;
					Console.WriteLine("The issues associated with the element are: " + text);
					

					//doing an issue search
					if (text != "No results")
					{
						driver.FindElement(By.Id("AssociatedIssueListSearchBox")).Click();
						driver.FindElement(By.Id("AssociatedIssueListSearchBox")).SendKeys("random");
						driver.FindElement(By.XPath("//*[@id='IssuesPanel']/div[1]/div/span")).Click();
					}

					wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='AssociatedIssueListTablePager']/div[1]")));

					driver.FindElementByLinkText("Info").Click();

					type = "POI,Signal,Vehicle";

					break;
				}
				case "Camera Details":
				{
					Console.WriteLine("This is a Camera element.");
					
					driver.FindElementByLinkText("Images").Click();
					driver.FindElement(By.XPath("//*[@id='ImageViewerContainer']/div/span")).Click();
					Task.Delay(new TimeSpan(0, 0, 50));
					driver.FindElement(By.XPath("//*[@id='ImageViewerContainer']/div/span")).Click();

					
					driver.FindElementByLinkText("Comm").Click();
					wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='CommentsPanel']/div")));
					text = driver.FindElementById("CommPanel").Text;
					Console.WriteLine("The communications associated with the camera are: " + text);


					driver.FindElementByLinkText("Recorded Videos").Click();
					wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='RecordedVideosListTablePager']/div[1]")));
					text = driver.FindElement(By.XPath("//*[@id='RecordedVideosListTablePager']/div[1]")).Text;
					Console.WriteLine("The recorded videos associated with the camera are: " + text);
					
					driver.FindElementByLinkText("Comments").Click();
					wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='CommentsPanel']/div")));
					text = driver.FindElement(By.XPath("//*[@id='CommentsPanel']/div")).Text;
					Console.WriteLine("The comments associated with the camera are: " + text);

					driver.FindElementByLinkText("Issues").Click();
					wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='AssociatedIssueListTablePager']/div[1]")));
					text = driver.FindElement(By.XPath("//*[@id='AssociatedIssueListTablePager']/div[1]")).Text;
					Console.WriteLine("The issues associated with the camera are: " + text);

					//doing an issue search
					if (text != "No results")
					{
						driver.FindElement(By.Id("AssociatedIssueListSearchBox")).Click();
						driver.FindElement(By.Id("AssociatedIssueListSearchBox")).SendKeys("random");
						driver.FindElement(By.XPath("//*[@id='IssuesPanel']/div[1]/div/span")).Click();
					}

					wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='AssociatedIssueListTablePager']/div[1]")));

					driver.FindElementByLinkText("Info").Click();
					
					type = "Camera";
					
					break;
				}
				case "Issue Details":
				{
					Console.WriteLine("This is an Issue element");
					type = "Issue";
					break;
				}
				default:
				{
					Console.WriteLine("The kind of element was not found");
					type = "Unknown";
					break;
				}
			}
		}
    }
}

/*Notes on website implementation:
 * (After the more information button)
 * 
 * buses: POI Details
 *		Info, Issues, Comments
 *signals: Signal Details
 *		Info, Issues, Comments
 * vehicles: Vehicle Details
 *		Info, Issues, Comments
 *		
 * cameras: Camera Details
 *		Info, Comm, Issues, Images, Recorded Videos, Comments
 * issues: Issue Details
 *		Info, Response Events, Cameras, Linked Issues, Comments

 */
