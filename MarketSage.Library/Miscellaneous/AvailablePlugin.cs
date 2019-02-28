using System;
using System.Collections.Generic;
using System.Text;

namespace MarketSage.Library
{
    /// <summary>
    /// Data Class for Available Plugin.  Holds and instance of the loaded Plugin, as well as the Plugin's Assembly Path
    /// 
    /// Taken directly from Redth's CodeProject article:
    /// http://www.codeproject.com/csharp/PluginsInCSharp.asp
    /// </summary>
    public class AvailablePlugin<PluginType>
    {
        //This is the actual AvailablePlugin object.. 
        //Holds an instance of the plugin to access
        //ALso holds assembly path... not really necessary
        private PluginType myInstance;
        private string myAssemblyPath = "";
        private Type myType;
        private string _name = "";

        public PluginType Instance
        {
            get { return myInstance; }
            set { myInstance = value; }
        }
        public string AssemblyPath
        {
            get { return myAssemblyPath; }
            set { myAssemblyPath = value; }
        }
        public Type PlugType
        {
            get { return myType; }
            set { myType = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
