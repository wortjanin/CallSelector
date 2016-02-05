using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CallSelectorLib.Impl
{
    public class SelectorImpl : Selector
    {
        private IErrorHandler errorHandler;
        private volatile bool run = true;

        public SelectorImpl(ISelectorConfig config)
            : base(config)
        {
        }

        public override void setIErrorHandler(IErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
        }

        public override bool Run()
        {
            try
            {
                int i = 0;
                while (run && base.callDataProvider.HasMessages())
                {
                    ICallMessage callMessage = base.callDataProvider.NextMessage();
                    base.callDataConsumer.HandleMessage(callMessage);
                    callMessage.Delete();
                    i++;
                    if (null != errorHandler && logDebug)
                        errorHandler.handleDebugInfo("" + i + " messages deleted/processed.");

                }
                if (null != errorHandler && logDebug && 0 == i)
                    errorHandler.handleDebugInfo("0 messages deleted/processed.");
            }
            catch (Exception e)
            {
                if (null != errorHandler) errorHandler.handle(e);
                else throw e;
            }

            return run;
        }

        public override void Stop()
        {
            run = false;
        }

    }
}
