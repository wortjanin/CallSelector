using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CallSelectorLib
{
    /// <summary>
    /// Must be thread-safe
    /// </summary>
    public interface ISelector : IDisposable
    {
        void setIErrorHandler(IErrorHandler errorHandler);

        /// <summary>
        /// Runs until mailbox has messages
        /// If finished by calling Stop() method, returns false otherwise true
        /// </summary>
        bool Run();

        void Stop();
    }
}
