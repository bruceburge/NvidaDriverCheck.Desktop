using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using System.Net;
using System.Text.RegularExpressions;

namespace NvidaDriverChecker.DesktopApp
{
    public partial class HiddenForm : Form
    {
        //TODO: move out of this scope.
        string urlToCheck;
        string patternToStripContent;
        double updateInterval;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public HiddenForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //TODO: move application settings to UI config, maybe
            try
            {
                logger.Trace("Getting Config");
                urlToCheck = Properties.Settings.Default.UrlToCheck;
                patternToStripContent = Properties.Settings.Default.PatternToStripContent;
                updateInterval = Properties.Settings.Default.UpdateIntervalMilliseconds;
                logger.Trace("End Getting Config");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error Getting Config from Properties settings in app.config");
                throw;                
            }

            notifyIcon1.Visible = true;
            tsmiLastDriverDate.Text = "Lastest Post: Unknown";
            notifyIcon1.Text = tsmiLastDriverDate.Text;
            
            var notificationTimer = new System.Timers.Timer(updateInterval);
            notificationTimer.AutoReset = true;
            notificationTimer.Elapsed += NotificationTimer_Elapsed;

            GetUpdate();

            notificationTimer.Enabled = true;
            logger.Debug(string.Format("Timer enabled with interval of {0}",updateInterval));

        }

        private void NotificationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            logger.Trace("Timer Elapsed, Get update");
            GetUpdate();
        }

        private Tuple<bool,string> GetUpdate()
        {
            var previousCheckFileContents = UserApplicationDataHelper.ValidateUserData("NvidaDriverDateChecker", "LastCheckedDate.txt");

            List<string> contentList = new List<string>();

            var webContent = WebContentHelper.GetWebContents(urlToCheck);

            contentList = WebContentHelper.GetValuesFromContent(webContent, patternToStripContent);

            var latest = contentList.FirstOrDefault();

            var messageMatchTuple = WebContentHelper.GenerateCheckResponseMessage(previousCheckFileContents, latest);
            
            if (!string.IsNullOrWhiteSpace(latest))
            {
                UserApplicationDataHelper.WriteToFile(latest);
                tsmiLastDriverDate.Text = "Lastest Post: " + latest;
                notifyIcon1.Text = tsmiLastDriverDate.Text;
            }


            logger.Debug(string.Format("Update Response, should alert user {0}, with message {1}", messageMatchTuple.Item1, messageMatchTuple.Item2));
            

            if(messageMatchTuple.Item1)
            {
                notifyIcon1.ShowBalloonTip(15000, "Nvida Driver Checker", messageMatchTuple.Item2.Replace(";", "\n").Replace(":", "\n"), ToolTipIcon.Info);
            }

            return messageMatchTuple;
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(urlToCheck);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void updateNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var response = GetUpdate();
            notifyIcon1.ShowBalloonTip(15000, "Nvida Driver Checker", response.Item2.Replace(";","\n").Replace(":","\n"), ToolTipIcon.Info);
        }

        private void HiddenForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.Visible = false;
        }
    }
}
