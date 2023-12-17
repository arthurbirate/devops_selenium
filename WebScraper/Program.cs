using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Generic;
using OpenQA.Selenium.DevTools.V117.DOM;
using OpenQA.Selenium.Support.UI;
using System.Collections.Specialized;
using System.Reflection;
using OpenQA.Selenium.DevTools;
using System;
using System.Text;
using OpenQA.Selenium.DevTools.V117.Network;

namespace scraping_selenium
{
    class Program
    {
        private static IWebDriver driver;

        static void Main(string[] args)
        {
            // List to store dictionaries representing scraped data
            List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();

            // StringBuilder to construct CSV content
            var csvContent = new System.Text.StringBuilder();

            Console.WriteLine("\t\t**** Web Scraping with Selenium *****");
            Console.WriteLine();
            Console.WriteLine("\t\tYou enter a website, for example, *google.com*");
            Console.WriteLine();
            Console.Write("\t\tEnter a Website: ");
            var website = Console.ReadLine();
            Console.WriteLine();
            Console.Write("\t\tEnter (Xpath,cssSelector,ID,Name,ClassName) to access the search input: ");
            var findElement = Console.ReadLine();
            Console.WriteLine();
            Console.Write("\t\tEnter your search Key: ");
            var searchKey = Console.ReadLine();
            Console.WriteLine();
            Console.Write("\t\tEnter CSV file name: ");
            var fileName = Console.ReadLine();

            // Construct the full URL
            var https = "https://www.";
            var url = string.Format("{0}{1}", https, website);

            // Initialize the ChromeDriver
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl($"{url}");

            if (website == "youtube.com")
            {
                try
                {
                    // Sleep to wait for the page to load
                    System.Threading.Thread.Sleep(3000);

                    // Find and click the accept all cookies button
                    IWebElement acceptAllButton = driver.FindElement(By.XPath("//*[@id=\"content\"]/div[2]/div[6]/div[1]/ytd-button-renderer[2]/yt-button-shape/button"));
                    acceptAllButton.Click();

                    // Sleep to wait for the page to adjust after accepting cookies
                    System.Threading.Thread.Sleep(2000);

                    // Find the search input element and perform a search
                    var getElement = driver.FindElement(By.Name($"{findElement}"));
                    getElement.SendKeys($"{searchKey}");
                    getElement.Submit();

                    // Sleep to wait for search results to load
                    System.Threading.Thread.Sleep(2000);

                    // Find and click the video button in the search results
                    var videoButton = driver.FindElement(By.XPath("//*[@id=\"chips\"]/yt-chip-cloud-chip-renderer[3]"));
                    videoButton.Click();

                    // Sleep to wait for video results to load
                    System.Threading.Thread.Sleep(3000);

                    // Find all video elements
                    var collections = FindElements(By.XPath("//*[@id=\"dismissible\"]/div"));

                    Console.WriteLine();

                    foreach (var collection in collections.Take(5))
                    {
                        // Find elements within each video element
                        var videoTitles = collection.FindElements(By.CssSelector("h3.title-and-badge.style-scope.ytd-video-renderer"));
                        var uploaderNames = collection.FindElements(By.XPath(".//div[@id='channel-info']//a[@class='yt-simple-endpoint style-scope yt-formatted-string']"));
                        var views = collection.FindElements(By.CssSelector("span.inline-metadata-item.ytd-video-meta-block:first-of-type"));
                        var videoLinks = collection.FindElements(By.CssSelector("a.yt-simple-endpoint.style-scope.ytd-video-renderer"));
                        Console.WriteLine();
                        Console.WriteLine();

                        int minCount = Math.Min(videoTitles.Count, Math.Min(uploaderNames.Count, Math.Min(views.Count, videoLinks.Count)));

                        // Iterate over the minimum count of elements
                        for (int i = 0; i < minCount; i++)
                        {
                            // Extract information from each element
                            var videoTitle = videoTitles[i].Text;
                            var uploaderName = uploaderNames[i].Text;
                            var viewsTitle = views[i].Text;
                            var link = videoLinks[i].GetAttribute("href");

                            // Display in the console
                            Console.WriteLine($"Video Title: {videoTitle}");
                            Console.WriteLine($"Uploader: {uploaderName}");
                            Console.WriteLine($"Views: {viewsTitle}");
                            Console.WriteLine($"Link: {link}");

                            // Create a dictionary entry for the scraped data
                            Dictionary<string, string> entry = new Dictionary<string, string>
                            {
                                { "VideoTitle", videoTitle },
                                { "Uploader", uploaderName },
                                { "Views", viewsTitle },
                                { "link", link }
                            };

                            // Add the entry to the list
                            data.Add(entry);

                            // Append to the CSV content
                            csvContent.AppendLine($"\"{videoTitle}\",\"{viewsTitle}\",\"{uploaderName}\",\"{link}\"");
                        }
                    }
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("Accept All button not found.");
                }
            }
            else if (website == "ictjob.be")
            {
                // Sleep to wait for the page to load
                System.Threading.Thread.Sleep(2000);

                // Find the search input element and perform a search
                var element = driver.FindElement(By.Name($"{findElement}"));
                element.SendKeys($"{searchKey}");
                element.Submit();

                // Sleep to wait for search results to load
                System.Threading.Thread.Sleep(25000);

                // Find all job elements
                var getJobs = FindElements(By.CssSelector("span.job-info"));

                // Iterate over the minimum count of elements or 5, whichever is smaller
                foreach (var job in getJobs.Take(5))
                {
                    // Find elements within each job element
                    var jobTitles = job.FindElements(By.CssSelector("h2.job-title"));
                    var jobCompanies = job.FindElements(By.CssSelector("span.job-company"));
                    var locations = job.FindElements(By.CssSelector("span.job-location"));
                    var keywords = job.FindElements(By.CssSelector("span.job-keywords"));
                    var jobLinks = job.FindElements(By.CssSelector("a.job-title.search-item-link"));

                    Console.WriteLine();
                    Console.WriteLine();

                    int minCount = Math.Min(jobTitles.Count, Math.Min(jobCompanies.Count, Math.Min(locations.Count, Math.Min(keywords.Count, jobLinks.Count))));

                    // Iterate over the minimum count of elements
                    for (int i = 0; i < minCount; i++)
                    {
                        // Extract information from each element
                        var jobTitle = jobTitles[i].Text;
                        var jobCompany = jobCompanies[i].Text;
                        var keyword = keywords[i].Text;
                        var location = locations[i].Text;
                        var link = jobLinks[i].GetAttribute("href");

                        // Display in the console
                        Console.WriteLine($"Job Title: {jobTitle}");
                        Console.WriteLine($"Company Name: {jobCompany}");
                        Console.WriteLine($"Location: {location}");
                        Console.WriteLine($"Keywords: {keyword}");
                        Console.WriteLine($"Link: {link}");

                        // Create a dictionary entry for the scraped data
                        Dictionary<string, string> entry = new Dictionary<string, string>
                        {
                           
                            { "Job Title", jobTitle },
                            { "Company Name", jobCompany },
                            { "Location", location },
                            { "Keyword", keyword },
                            { "Link", link }
                        };

                        // Add the entry to the list
                        data.Add(entry);

                        // Append to the CSV content
                        csvContent.AppendLine($"\"{jobTitle}\",\"{jobCompany}\",\"{location}\",\"{keyword}\",\"{link}\"");
                    }
                }
            }
            else
            {
                // Sleep to wait for the page to load
                System.Threading.Thread.Sleep(2000);

                var acceptButton = driver.FindElement(By.XPath("//*[@id=\"onetrust-accept-btn-handler\"]"));
                acceptButton.Click();
                System.Threading.Thread.Sleep(3000);
                var button = driver.FindElement(By.XPath("//*[@id=\"app-root\"]/div/div[6]/header/div[2]/div[2]/nav/ul/li[2]/a"));
                button.Click();

                
                // Find all albums list of  album elements
                var getAlbums = FindElements(By.CssSelector("div.fragment-list"));

                // Sleep to wait for elements to load
                System.Threading.Thread.Sleep(3000);

                // Iterate over album list album elements and print the text
                foreach (var Album in getAlbums.Take(5))
                {
                    var artists = Album.FindElements(By.CssSelector("li"));
                    var albums = Album.FindElements(By.CssSelector("em"));


                   int minCount = Math.Min(artists.Count,albums.Count);


                    for (int i = 0; i < minCount; i++)
                    {
                        // Extract information from each element
                        var artist = artists[i].Text;
                        var album = albums[i].Text;
                       

                        // Display in the console
                        Console.WriteLine($"Artist: {artist}");
                        Console.WriteLine($"Album: {album}");
                    

                        // Create a dictionary entry for the scraped data
                        Dictionary<string, string> entry = new Dictionary<string, string>
                        {

                            { "Artist", artist },
                            { "Album", album },
                          
                        };

                        // Add the entry to the list
                        data.Add(entry);

                        // Append to the CSV content
                        csvContent.AppendLine($"\"{artist}\",\"{album}\"");
                    }

                }
            }

            // Serialize the data to JSON
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

            // Save JSON data to file
            SaveToFile(jsonData, $"C:\\Users\\ACER\\Desktop\\{fileName}.json");

            // Save to a CSV file
            SaveToCsv(csvContent.ToString(), $"C:\\Users\\ACER\\Desktop\\{fileName}.csv");
        }

        static void SaveToCsv(string csvContent, string filePath)
        {
            // Write CSV content to the specified file path
            File.WriteAllText(filePath, csvContent);
            Console.WriteLine($"Data saved to {filePath}");
        }

        static void SaveToFile(string data, string filePath)
        {
            // Write data to the specified file path
            File.WriteAllText(filePath, data);
            Console.WriteLine($"Data saved to {filePath}");
        }

        static IReadOnlyCollection<IWebElement> FindElements(By by)
        {
            // Custom method to continuously attempt to find elements until at least one is found
            while (true)
            {
                var elements = driver.FindElements(by);

                if (elements.Count > 0)
                    return elements;

                // Sleep to avoid continuous polling
                Thread.Sleep(5);
            }
        }
    }
}
