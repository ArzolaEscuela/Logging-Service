using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;

namespace Logging_Service
{
    public partial class Service : ServiceBase
    {
        private static System.Timers.Timer _timer;
        private int _elapsedSeconds = 0;

        public Service()
        {
            InitializeComponent();
        }

        // Remnant of tests
        public void DebugStart()
        {

        }

        private void PrepareLogging()
        {
            EventLog.Log = "Application"; // Set logs to the application category
            AutoLog = false;

            ((ISupportInitialize)this.EventLog).BeginInit();
            if (!EventLog.SourceExists(this.ServiceName))
            {
                EventLog.CreateEventSource(this.ServiceName, "Application");
            }
            ((ISupportInitialize)this.EventLog).EndInit();

            EventLog.Source = this.ServiceName;
        }

        private void OnEverySecond(object source, ElapsedEventArgs e)
        {
            _elapsedSeconds++;

            EventLog.WriteEntry("Example Log");

            if (_elapsedSeconds % 5 == 0)
            {
                EventLog.WriteEntry("Example Warning", EventLogEntryType.Warning);
            }

            if (_elapsedSeconds % 30 == 0)
            {
                EventLog.WriteEntry("Example Error", EventLogEntryType.Error);
            }

            if (_elapsedSeconds > 59) { _elapsedSeconds = 0; }
        }

        protected override void OnStart(string[] args)
        {
            PrepareLogging();

            // Create a timer with a one second interval.
            _timer = new System.Timers.Timer(1000);

            // Hook up the Elapsed event for the timer.
            _timer.Elapsed += new ElapsedEventHandler(OnEverySecond);

            _timer.Interval = 1000;
            _timer.Enabled = true;
            // System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "OnStart.txt");
        }

        protected override void OnStop() { }
    }
}
