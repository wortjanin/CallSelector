using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CallSelectorLib.Impl;
using System.IO;

namespace CallSelectorLib
{
    public class CallSelectorFactory
    {
        public static ISelector createISelector(ISelectorConfig config)
        {
            return new SelectorImpl(config);
        }

        public static ISelectorConfig loadISelectorConfig(FileInfo fileInfo)
        {
            return new SelectorConfigImpl(fileInfo);
        }

        public static IWebConfig loadIWebConfig(FileInfo fileInfo)
        {
            return new WebConfigImpl(fileInfo);
        }
    }
}
