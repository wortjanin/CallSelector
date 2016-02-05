using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace CallSelectorLib.Impl
{
    public abstract class Selector : ISelector
    {
        protected ICallDataProvider callDataProvider;
        protected ICallDataConsumer callDataConsumer;

        protected bool logDebug = false;
        public Selector(ISelectorConfig config)
        {
            Debug.Assert(null != config);
            logDebug = config.LogDebug();
            switch (config.Protocol())
            {
                case MailProtocol.Pop3:
                    this.callDataProvider = new CallDataProviderImplPop3(config);
                    break;
                default:
                    throw new InvalidDataException("Undefined Mail protocol type");
            }

            switch (config.DbType())
            {
                case DBType.SQLServer:
                    this.callDataConsumer = new CallDataConsumerImplSqlServer(config);
                    break;
                default:
                    throw new InvalidDataException("Undefined Data base type");
            }
        }

        public abstract void setIErrorHandler(IErrorHandler errorHandler);
        public abstract bool Run();
        public abstract void Stop();

        #region IDisposable Members

        public void Dispose()
        {
           this.Stop();
           this.callDataProvider.Dispose();
           this.callDataConsumer.Dispose();
        }

        #endregion
    }
}
