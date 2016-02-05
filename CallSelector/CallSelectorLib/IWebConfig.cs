using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CallSelectorLib
{
    public interface IWebConfig
    {
        DBType DbType();
        string DBConnectionString();

        Dictionary<string, IWebFileConfig> SenderConfigDictionary();
    }
}
