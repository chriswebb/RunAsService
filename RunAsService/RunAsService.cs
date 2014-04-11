using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace RunAsService
{
    public partial class RunAsService : ServiceBase
    {
        private String myExecutable = null;
        private Process myProcess = null;
        public RunAsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                this.myExecutable = System.Configuration.ConfigurationSettings.AppSettings["executable"];
            }
            catch
            {
                EventLog.WriteEntry("Could not read 'executable' value from application configuration file.", EventLogEntryType.Error); 
                this.Stop();
                return;
            }

            try
            {
                this.myProcess = Process.Start(this.myExecutable);
            }
            catch (Win32Exception)
            {
                EventLog.WriteEntry("Could not find file: " + Environment.NewLine + Environment.NewLine + this.myExecutable, EventLogEntryType.Error);
                this.Stop();
            }
            catch (System.IO.FileNotFoundException)
            {
                EventLog.WriteEntry("Could not find file: " + Environment.NewLine + Environment.NewLine + this.myExecutable, EventLogEntryType.Error);
                this.Stop();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Error occured while trying to start \"" + this.myExecutable + "\": " + Environment.NewLine + Environment.NewLine + ex.ToString(), EventLogEntryType.Error);
                this.Stop();
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (!(this.myProcess == null && this.myProcess.HasExited))
                    this.myProcess.Kill();
            }
            catch
            {
            }
        }
    }
}
