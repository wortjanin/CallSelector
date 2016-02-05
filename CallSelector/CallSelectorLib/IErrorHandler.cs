using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CallSelectorLib
{
    public interface IErrorHandler
    {
        void handle(Exception e);
        void handleDebugInfo(string message);
    }
}
