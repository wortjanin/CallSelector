using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using CallSelectorLib;
using System.IO;

namespace CallSelector
{
    public partial class CallSelectorService : ServiceBase
    {
        private IErrorHandler errorHandler = null;
        private ISelectorConfig config = null;
        private volatile ISelector selector = null;

        private Object lockShutdown = new Object();
        private Object lockLog = new Object();
        private volatile bool hasShutteddown = false;

        private Thread processingThread = null;
        private ManualResetEvent threadStarted = new ManualResetEvent(false); 
        private ManualResetEvent stopThread = new ManualResetEvent(false);

        public CallSelectorService()
        {
            InitializeComponent();
            
            string serviceName = "CallSelectorSource";
            string logName = "CallSelectorLog";
            if (!System.Diagnostics.EventLog.SourceExists(serviceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    serviceName, logName);
            }
            serviceEventLog.Source = serviceName;
            serviceEventLog.Log = logName;
        }

        private void RunProcessing()
        {
            try
            {
                int delay = config.HostRequestDelayMilliseconds();
                selector = CallSelectorFactory.createISelector(config);
                selector.setIErrorHandler(errorHandler);
                threadStarted.Set();
                while (!stopThread.WaitOne(delay))
                {
                    if (false == selector.Run())
                        break;
                }
                lock (lockLog)
                {
                    serviceEventLog.WriteEntry("Processing finished.");
                }
            }
            catch (ThreadAbortException) {/*ignore*/}
        }


        private void setInstallationWorkingDirectory()
        {
            String path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            path = System.IO.Path.GetDirectoryName(path);
            Directory.SetCurrentDirectory(path);
        }


        protected override void OnStart(string[] args)
        {
           
            try
            {
                base.OnStart(args);
                int delay = 0;
                lock (lockShutdown)
                {
                    hasShutteddown = false;

                    errorHandler = new ErrorHandler(serviceEventLog, lockLog);

                    setInstallationWorkingDirectory();
                    config = CallSelectorFactory.loadISelectorConfig(new FileInfo("CallSelectorConfig.xml"));
                    delay = config.HostRequestDelayMilliseconds();
                    processingThread = new Thread(new ThreadStart(RunProcessing));
                    processingThread.Start();
                }
                lock (lockLog)
                {
                    serviceEventLog.WriteEntry("Start. delay=" + delay + "; " + (config.LogDebug() ? "debug=true" : "debug=false"));
                }
            }
            catch (ThreadAbortException) { /*ignore*/}
            catch (Exception e)
            {
                if (null != errorHandler) errorHandler.handle(e);
            }
            
        }

        protected override void OnShutdown()
        {
            ShutdownService();
            base.OnShutdown();
        }

        protected override void OnStop()
        {
            ShutdownService();
            base.OnStop();
        }

        private void ShutdownService()
        {
            try
            {
                lock (lockShutdown)
                {
                    if (!hasShutteddown)
                    {
                        hasShutteddown = true;
                        if (threadStarted.WaitOne(1500))
                        {
                            ISelector s = selector;
                            stopThread.Set();
                            s.Stop();
                            if (!processingThread.Join(15000))
                                processingThread.Abort();
                        }
                    }
                }
                lock (lockLog)
                {
                    serviceEventLog.WriteEntry("Service stopped.");
                }
            }
            catch {/* ignire */}
            finally
            {
                selector.Dispose();
                stopThread.Reset();
                threadStarted.Reset();
            }
        }

        private class ErrorHandler : IErrorHandler
        {
            private EventLog serviceEventLog;
            private Object lockLog;
            public ErrorHandler(EventLog serviceEventLog, Object lockLog)
            {
                this.serviceEventLog = serviceEventLog;
                this.lockLog = lockLog;
            }

            #region IErrorHandler Members


            public void handle(Exception e)
            {
                if (e is ThreadAbortException) throw e;
                try
                {
                    lock (lockLog)
                    {
                        serviceEventLog.WriteEntry(e.Message, EventLogEntryType.Error);
                    }
                }
                catch { }
            }

            public void handleDebugInfo(string message)
            {
                try
                {
                    lock (lockLog)
                    {
                        serviceEventLog.WriteEntry(message, EventLogEntryType.Warning);
                    }
                }
                catch { }
            }

            #endregion
        }
    }
}
