using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Website_Selenium_Tests_1_
{
	class WebsiteSeleniumTest
	{
		static ChromeDriver driver;
		private static WebDriverWait wait;

		static void Main()
		{
			SearchTest("don mills", 2);
			SearchTest("sheppard and yonge", 2);
			SearchTest("q.09l", 2);
			SearchTest("york mills station",2);
			SearchTest("mac and cheese festive", 2);
			//SearchTest("s", 2);
			SearchTest("140 church", 2);
			SearchTest("titanic", 2);
			SearchTest("manpuku", 2);
			SearchTest("130 king", 2);
			SearchTest("toronto weather", 2);
			SearchTest("art gallery in toronto", 2);
			SearchTest("construction yonge", 2);
			SearchTest("ramen", 2);
		}

		/// <summary>
		/// This function searches something in the search bar using query, and then selects the result based on the index. If the index invalid, it is defaulted to 1
		/// </summary>
		/// <param name="query"></param>
		/// <param name="index"></param>
		private static void SearchTest(string query, int index)
		{
			Console.WriteLine();
			Console.BackgroundColor = ConsoleColor.White;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.WriteLine("The search query was: " + query);
			Console.ResetColor();

			driver = new ChromeDriver("C:\\Users\\User\\Desktop\\SeleniumTests\\chromedriver_win32\\");
			wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(10000));
			var stopwatch = new Stopwatch();

			stopwatch.Start();
			//going to the website
			driver.Navigate().GoToUrl("http://mirasan.ca");
			stopwatch.Stop();

			//returns how long it took for the page to load
			Console.WriteLine("The webpage took " + stopwatch.ElapsedMilliseconds + " miliseconds to load");

			stopwatch.Start();

			driver.FindElement(By.Id("LayoutHeaderSearchBox")).SendKeys(query);
			driver.FindElement(By.Id("LayoutHeaderSearchBox")).SendKeys(Keys.Enter);

			var timeout = new Stopwatch();
			timeout.Start();

			// waits until the results come up
			wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("aniSpriteLoadingBarSmall")));
			wait.Until<bool>(x =>
			{
				var element = x.FindElement(By.Id("LayoutHeaderSearchStatus"));
				return element.Text != "Loading" && element.Text != "Showing recently-accessed devices";
			});
			
			//returns the time it took for the search
			timeout.Stop();
			Console.WriteLine("The search took: " + timeout.ElapsedMilliseconds);

			//if there are no results
			if (driver.FindElement(By.Id("LayoutHeaderSearchStatus")).Text == "No results")
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("There were no results found");
				Console.ResetColor();

				driver.Quit();
				return;
			}
			
			//checking for out of bounds indexes
			if (driver.FindElements(By.XPath("//*[@id='LayoutHeaderSearchResults']/div")).Count == 1)
			{
				index = 1;
			}
			else if (driver.FindElements(By.XPath("//*[@id='LayoutHeaderSearchResults']/div")).Count < index)
			{
				index = driver.FindElements(By.XPath("//*[@id='LayoutHeaderSearchResults']/div")).Count;
			}

			//Clicking on a search result
			wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='LayoutHeaderSearchResults']/div[" + index + "]")));
			driver.FindElement(By.XPath("//*[@id='LayoutHeaderSearchResults']/div["+ index +"]")).Click();
			
			// Waiting for the the popup to come up
			wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[5]/div[2]/div[2]/table/tbody")));
			for (int j = 1; j <= driver.FindElements(By.XPath("/html/body/div[5]/div[2]/div[2]/table/tbody")).Count(); j++)
			{
				wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[5]/div[2]/div[2]/table/tbody/tr["+j+"]")));	
			}

			//tests the popup
			ProcessPopup();

			stopwatch.Stop();
			Console.WriteLine("This test took: " + stopwatch.ElapsedMilliseconds);

			driver.Quit();
		}

		/// <summary>
		/// This function processes the pop up after a search
		/// </summary>
		private static void ProcessPopup()
		{
			//the element type
			string type;

			// checks to see if there is a "More Information" button
			if (driver.FindElements(By.LinkText("More Information")).Count > 0)
			{
				Console.WriteLine("More information was found");
				driver.FindElement(By.LinkText("More Information")).SendKeys(Keys.Control + Keys.Return);

				var tabs = new List<String>(driver.WindowHandles);
				driver.SwitchTo().Window(tabs[1]);
				driver.SwitchTo().Window(driver.WindowHandles.Last());

				wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("infoPageTitleText")));

				//figures out what kind of information we are dealling with and acts accordingly
				ActOnElement(out type);
				Console.WriteLine("Element accessed was a " + type + " element");

				driver.Close();
				driver.SwitchTo().Window(driver.WindowHandles.First());
				driver.SwitchTo().Window(tabs[0]);
			}

			/*
			//checks to see if there is a website link
			for (int i = 1; i <= driver.FindElements(By.XPath("/html/body/div[5]/div[2]/div[2]/table/tbody/tr")).Count; i++)
			{
				if (driver.FindElement(By.XPath("/html/body/div[5]/div[2]/div[2]/table/tbody/tr[" + i + "]/th")).Text == "Website")
				{
					driver.FindElement(By.XPath("/html/body/div[5]/div[2]/div[2]/table/tbody/tr["+ i +"]/td/a")).Click();
					return;
				}
			}
			 */
		}

		/// <summary>
		/// This function helps ou the ActOnElement function. The hasIssue parameter is set to whether or not the element has issues
		/// </summary>
		/// <param name="hasIssue"></param>
		private static void ActOnElementHelper(out bool hasIssue)
		{
			string text;
			driver.FindElementByLinkText("Comments").Click();
			wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='CommentsPanel']/div")));
			text = driver.FindElement(By.XPath("//*[@id='CommentsPanel']/div")).Text;
			Console.WriteLine("The comments associated with the element are: " + text);

			//clicking on the issues tab with the rest of them
			driver.FindElement(By.CssSelector("#TabContainer > ul > li:nth-child(2) > a")).Click();
			wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#AssociatedIssueListTablePager > div.text")));
			text = driver.FindElement(By.CssSelector("#AssociatedIssueListTablePager > div.text")).Text;
			Console.WriteLine("The issues associated with the element are: " + text);

			if (text != "No results")
			{
				/*
				//doing an issue search
				driver.FindElement(By.Id("AssociatedIssueListSearchBox")).Click();
				driver.FindElement(By.Id("AssociatedIssueListSearchBox")).SendKeys("in");
				driver.FindElement(By.Id("AssociatedIssueListSearchBox")).SendKeys(Keys.Enter);
				*/

				//selecting a result
				Console.WriteLine("There are issues associated with this element");

				wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#AssociatedIssueListTable > tbody > tr:nth-child(1)")));
				driver.FindElement(By.CssSelector("#AssociatedIssueListTable > tbody > tr:nth-child(1)")).Click();

				//Waiting for the issues page to load 
				wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Response Events")));
				hasIssue = true;
				return;
			}

			wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='AssociatedIssueListTablePager']/div[1]")));

			driver.FindElementByLinkText("Info").Click();
			hasIssue = false;
		}

		/// <summary>
		/// This function distinguishes between different kinds of elements (Vehicle, Camera, etc)
		/// and then tests their associated webpage accordingly
		/// </summary>
		/// <param name="type"></param>
		private static void ActOnElement(out string type)
		{
			bool hasIssue = false;
			var element = driver.FindElement(By.XPath("//*[@id='LayoutBody']/div[1]/a[3]"));
			string text;
			switch (element.Text)
			{
				case "POI Details":
					{
						Console.WriteLine("This is a POI element");
						type = "POI";
						ActOnElementHelper(out hasIssue);
						if (hasIssue)
						{
							goto case "Issue Details";
						}
						break;
					}
				case "Signal Details":
					{
						Console.WriteLine("This is a Signal element");
						type = "Signal";
						ActOnElementHelper(out hasIssue);
						if (hasIssue)
						{
							goto case "Issue Details";
						}
						break;
					}
				case "Weather Station Details":
					{
						Console.WriteLine("This is a Weather Station element");
						type = "Weather Station";
						ActOnElementHelper(out hasIssue);
						if (hasIssue)
						{
							goto case "Issue Details";
						}
						//selects the website link if there is one
						wait.Until(ExpectedConditions.ElementIsVisible(By.Id("InfoPanel")));
						driver.FindElement(By.XPath("//*[@id='InfoPanel']/div[2]/div[1]/table/tbody/tr[3]/td[2]/a")).Click();

						break;
					}
				case "Camera Details":
					{
						Console.WriteLine("This is a Camera element.");

						driver.FindElementByLinkText("Images").Click();
						//Play video
						driver.FindElement(By.XPath("//*[@id='ImageViewerContainer']/div/span")).Click();
						Task.Delay(new TimeSpan(0,0,50));
						
						//Adjust video speed
						driver.FindElement(By.XPath("//*[@id='ImageViewerContainer']/div/div[3]/div[1]")).Click();

						if (driver.FindElement(By.XPath("//*[@id='ImageViewerContainer']/div/div[3]/div[1]/strong")).Text == "8000")
						{
							Console.BackgroundColor = ConsoleColor.Cyan;
							Console.WriteLine("The image speed button of the camera element is working");
						}
						else
						{
							Console.BackgroundColor = ConsoleColor.Red;
							Console.WriteLine("The image speed button of the camera element is not working");
						}
						Console.ResetColor();
				
						//Change length
						driver.FindElement(By.XPath("//*[@id='ImageViewerContainer']/div/div[3]/div[2]/strong")).Click();
						if (driver.FindElement(By.XPath("//*[@id='ImageViewerContainer']/div/div[3]/div[2]/strong")).Text == "2 Days")
						{
							Console.BackgroundColor = ConsoleColor.Cyan;
							Console.WriteLine("The image Length button of the camera element is working");
						} else
						{
							Console.BackgroundColor = ConsoleColor.Red;
							Console.WriteLine("The image speed button of the camera element is not working");
						}
						Console.ResetColor();
						driver.FindElement(By.XPath("//*[@id='ImageViewerContainer']/div/span")).Click();

						

						driver.FindElementByLinkText("Comm").Click();
						wait.Until(ExpectedConditions.ElementIsVisible(By.Id("CommPanel")));
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



						//clicking on the issues tab with the rest of them
						driver.FindElement(By.XPath("//*[@id='TabContainer']/ul/li[3]/a")).Click();
						wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='AssociatedIssueListTablePager']/div[1]")));
						text = driver.FindElement(By.XPath("//*[@id='AssociatedIssueListTablePager']/div[1]")).Text;
						Console.WriteLine("The issues associated with the camera are: " + text);
						
						
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

						driver.FindElementByLinkText("Comments").Click();
						wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='CommentsPanel']/div")));
						text = driver.FindElement(By.XPath("//*[@id='CommentsPanel']/div")).Text;
						Console.WriteLine("The comments associated with the element are: " + text);

						driver.FindElement(By.LinkText("Linked Issues")).Click();
						wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='AssociatedIssueListTablePager']/div[1]")));
						text = driver.FindElement(By.XPath("//*[@id='AssociatedIssueListTablePager']/div[1]")).Text;
						Console.WriteLine("The linked issues associated with the element are: " + text);

						driver.FindElementByLinkText("Response Events").Click();
						wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ResponseEventsPanel")));
						text = driver.FindElement(By.Id("ResponseEventsListTablePager")).Text;
						Console.WriteLine("The response events associated with the element are: " + text);


						//checking to see if there is a cameras tab
						for (int i = 1; i <= driver.FindElements(By.XPath("//*[@id='TabContainer']/ul/li")).Count; i++)
						{
							if (driver.FindElement(By.XPath("//*[@id='TabContainer']/ul/li[" + i + "]/a")).Text == "Cameras")
							{
								//clicking on camera tab
								driver.FindElement(By.XPath("//*[@id='TabContainer']/ul/li[" + i + "]/a")).Click();
								wait.Until(ExpectedConditions.ElementIsVisible(By.Id("CamerasListTable")));

								//clicking on a camera
								wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='CamerasListTable']/tbody/tr[1]")));
								driver.FindElement(By.XPath("//*[@id='CamerasListTable']/tbody/tr[1]/td[4]/a/img")).Click();
								
								Console.WriteLine("There are cameras associated with this element");

								//waiting for the individual page to load
								wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Camera Details")));
								goto case "Camera Details";
							}
						}
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
 *		Info, Edit History, Response Events, Cameras, Linked Issues, Comments

 */

