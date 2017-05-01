using System;
using System.IO;
using NLog;

namespace NvidaDriverChecker.DesktopApp
{
    public static class UserApplicationDataHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static string applicationDirectory = "";
        private static string applicationFile = "";

        public static string ValidateUserData(string ApplicationDirectoryName, string FileName)
        {
            applicationDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationDirectoryName);

            logger.Trace(string.Format("Validate User directory: {0}", applicationDirectory));
            CreateDirectory(applicationDirectory);
            applicationFile = Path.Combine(applicationDirectory, FileName);
            logger.Trace(string.Format("Validate file: {0}", applicationFile));
            WriteToFile();
            string results = ReadFile(applicationFile);
            logger.Trace(string.Format("Read file: {0} characters read from {1}", results.Length, applicationFile));
            return results;
        }

        public static string ReadFile()
        {
            return ReadFile(applicationFile);
        }

        public static string ReadFile(string filePath = "")
        {

            var results = string.Empty;

            if (!File.Exists(filePath))
            {
                var errorMessage = string.Format("File not found, While attempting a read : {0}", filePath);
                var ex = new FileNotFoundException(errorMessage);
                logger.Error(ex);
                throw ex;
            }

            try
            {
                results = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                logger.Error(ex, string.Format("Error attempting to read from {0}", filePath));
                throw;
            }

            return results;
        }
        public static void WriteToFile(string contents = "")
        {
            WriteToFile(applicationFile, contents);
        }

        public static void WriteToFile(string filePath, string contents = "")
        {

            try
            {
                bool doesFileExist = File.Exists(filePath);

                logger.Trace(string.Format("File does{0} exist : {1}", (doesFileExist ? "" : " not"), filePath));

                if (doesFileExist)
                {
                    if (!string.IsNullOrEmpty(contents))
                    {
                        logger.Trace(string.Format("Contents exist, writing {0} characters to {1}", contents.Length, filePath));
                        File.WriteAllText(filePath, contents);
                    }
                    else
                    {
                        logger.Debug("File exist, but no contents to write, Do nothing");
                    }
                }
                else
                {
                    logger.Trace(string.Format("Contents of {0} characters used to create {1}", contents.Length, filePath));
                    File.WriteAllText(filePath, contents);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, String.Format("Failed to create or write to file: {0}", filePath));
                throw;
            }
        }

        public static void CreateDirectory()
        {
            CreateDirectory(applicationDirectory);
        }

        public static void CreateDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                logger.Trace(String.Format("Directory not found, attempt to create Directory: {0}", directoryPath));
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, String.Format("Failed to create Directory: {0}", directoryPath));
                    throw;
                }

            }
            else
            {
                logger.Trace(String.Format("Directory found : {0}", directoryPath));
            }
        }


    }
}
