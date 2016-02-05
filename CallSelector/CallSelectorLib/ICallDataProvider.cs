using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CallSelectorLib
{
    public interface ICallDataProvider : IDisposable 
    {
        bool HasMessages();
        ICallMessage NextMessage();
    }
}
