using System;
using System.Collections.Generic;
using System.Text;

namespace MarketSage.Library
{
    /// <summary>
    /// Collection for AvailablePlugin Type
    /// 
    /// Taken directly from Redth's CodeProject article:
    /// http://www.codeproject.com/csharp/PluginsInCSharp.asp
    /// 
    /// Note: Modified to use .NET 2.0 generics
    /// </summary>
    public class AvailablePlugins<PluginType> : List<AvailablePlugin<PluginType>>
    {
        
    }
}
