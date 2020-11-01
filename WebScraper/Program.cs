using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace WebScraper
{
    /// <summary>
    /// Program class.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main method to call and process data.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string siteUrl = "https://www.314e.com/";
            MainAsync(siteUrl).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get text from provided url and process data to get top 10 frequent words.
        /// Nuget Package used to get data from URL and for its further processing is HtmlAgilityPack.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        async static Task MainAsync(string args)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(args);
            var pageContents = await response.Content.ReadAsStringAsync();
            
            //Parse page contents using htmldocument (HtmlAgility Nuget Package).
            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(pageContents);
            
            //Get Inner Nodes having div class.
            var divNodes = pageDocument.DocumentNode.SelectNodes("//div");
            StringBuilder webTextBuilder = new StringBuilder();

            foreach(var item in divNodes)
            {
                var cleanText = GetCleanText(item.InnerText);
                _ = !string.IsNullOrEmpty(cleanText) ? webTextBuilder.Append(cleanText) : null;
            }
            
            var list = RemoveExtraSpaces(webTextBuilder.ToString()).Split(" ");
            
            //Get to 10 frequently occured words.
            var topTenFrequentWords = FindWords(list);
            Console.WriteLine("Top 10 Frequent Words - ");
            Console.WriteLine(topTenFrequentWords);

            //Get top 10 frequent Pair of words
            var topTenFrequentWordPairs = FindWordPairs(list);
            Console.WriteLine("Top 10 Frequent Word Pairs - ");
            Console.WriteLine(topTenFrequentWordPairs);

            Console.ReadLine();
        }

        /// <summary>
        /// clean html text provided by htmlagility.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <returns></returns>
        static string GetCleanText(string htmlText)
        {
            //Replace new line with spaces.
            htmlText = htmlText.Replace("\n", " ");
            htmlText = htmlText.Replace("\r", string.Empty);
            //Replace tabs with spaces
            htmlText = htmlText.Replace("\t", " ");
            return htmlText;
        }

        /// <summary>
        /// remove multiple spaces.
        /// </summary>
        /// <param name="htmlText"></param>
        /// <returns></returns>
        static string RemoveExtraSpaces(string htmlText)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            htmlText = regex.Replace(htmlText, " ");
            return htmlText;
        }

        /// <summary>
        /// Find top 10 words with frequencies using HashMap dataset.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        static string FindWords(string[] arr)
        {
            // Create Dictionary to store word  
            // and it's frequency  
            Dictionary<string, int> words =
                new Dictionary<string, int>();

            // Iterate through array of words  
            for (int i = 0; i < arr.Length; i++)
            {
                // If word already exist in Dictionary  
                // then increase it's count by 1  
                if (words.ContainsKey(arr[i]))
                {
                    words[arr[i]] = words[arr[i]] + 1;
                }

                // Otherwise add word to Dictionary  
                else
                {
                    words.Add(arr[i], 1);
                }
            }

            StringBuilder key = new StringBuilder();
            
            //Get top 10 most frequent words.
            var sortedDict = (from entry in words orderby entry.Value descending select entry)
              .ToDictionary(pair => pair.Key, pair => pair.Value).Take(10).ToList();

            //Generating data for display.
            foreach (KeyValuePair<String, int> item in sortedDict)
            {
                key.Append(item.Key).Append("~").Append(item.Value).Append("  ");
            }

            // Return top 10 words having highest frequency  
            return key.ToString().Trim();
        }

        /// <summary>
        /// Get top 10 frequently occured word pairs using HashMap.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        static string FindWordPairs(string[] arr)
        {
            // Create Dictionary to store word pair 
            // and it's frequency  
            Dictionary<string, int> wordPairs =
                new Dictionary<string, int>();

            // Iterate through array of words
            for (int i = 0; i < arr.Length; i=i+2)
            {
                //get pair of words by joining
                string pairVal = (i + 1 >= arr.Length) ? arr[i] : string.Join(' ', arr[i], arr[i + 1]);

                // If word pair already exist in Dictionary  
                // then increase it's count by 1
                if (wordPairs.ContainsKey(pairVal))
                {
                    wordPairs[pairVal] = wordPairs[pairVal] + 1;
                }

                // Otherwise add word to Dictionary  
                else
                {
                    wordPairs.Add(pairVal, 1);
                }
            }

            StringBuilder key = new StringBuilder();

            //Get top 10 most frequent word pairs.
            var sortedDict = (from entry in wordPairs orderby entry.Value descending select entry)
              .ToDictionary(pair => pair.Key, pair => pair.Value).Take(10).ToList();

            //Generating data for display.
            foreach (KeyValuePair<String, int> item in sortedDict)
            {
                key.Append(item.Key).Append("~").Append(item.Value).Append("  ");
            }

            // Return top 10 word pairs having highest frequency  
            return key.ToString().Trim();
        }
    }
}
