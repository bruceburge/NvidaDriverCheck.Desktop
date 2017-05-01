using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NvidaDriverChecker.DesktopApp
{
    public static class WebContentHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static Tuple<bool,string> GenerateCheckResponseMessage(string previousCheckFileContents, string latest)
        {
            var msg = string.Empty;
            var raiseAlert = true;

            if (string.IsNullOrEmpty(latest))
            {
                msg = "Could not retrieve latest date!";
            }
            else if (string.IsNullOrWhiteSpace(previousCheckFileContents))
            {
                msg = string.Format("No previous date recorded,Writing {0}", latest);
            }
            else
            {
                var doDatesMatch = (previousCheckFileContents == latest);
                msg = string.Format("{0}ew Driver;Newest date:{1}", (doDatesMatch ? "No n" : "N"), latest);
                raiseAlert = !doDatesMatch;
            }
                        
            return new Tuple<bool, string>(raiseAlert, msg); ;
        }

        public static List<string> GetValuesFromContent(string webContent, string matchPattern, string stripPattern = @"\s*<.*?>\s*")
        {
            List<string> listOfMatches = new List<string>();
            logger.Trace("Start GetValuesFromContent");
            MatchCollection matches = Regex.Matches(webContent, matchPattern, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                string value = match.Groups[1].Value;
                // Remove inner tags from text.
                string cleanText = Regex.Replace(value, stripPattern, "", RegexOptions.Singleline);
                listOfMatches.Add(cleanText);
            }

            return listOfMatches;
        }

        public static string GetWebContents(string url)
        {
            try
            {
                string contents = string.Empty;
                logger.Trace("Start GetWebContents");
                using (WebClient webClient = new WebClient())
                {
                    contents = webClient.DownloadString(url);
                }
                logger.Trace(string.Format("End GetWebContents, Contents character count {0}", contents.Length));
                return contents;
            }
            catch (Exception ex)
            {
                logger.Error(ex, string.Format("Error occured while trying to get contents from {0}", url));
                throw;
            }
        }
    }
}
