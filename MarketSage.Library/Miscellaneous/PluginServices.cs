using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

/* MarketSage
   Copyright © 2008, 2009 Jeffrey Morton
 
   This program is free software; you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation; either version 2 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.
 
   You should have received a copy of the GNU General Public License
   along with this program; if not, write to the Free Software
   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA */

namespace MarketSage.Library
{
    public class PluginServices
    {
        private static PluginServices instance = new PluginServices();
        private AvailablePlugins<IIndicator> _availablePlugins = new AvailablePlugins<IIndicator>();

        public AvailablePlugins<IIndicator> AvailablePlugins
        {
            get { return _availablePlugins; }
            set { _availablePlugins = value; }
        }

        public static PluginServices Instance
        {
            get { return instance; }
        }

        public PluginServices()
        {
        }

        public void FindPlugins()
        {
            _availablePlugins.Clear();
            AddPlugin(AppDomain.CurrentDomain.BaseDirectory + "MarketSage.Plugin.Core.dll");  // default
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "plugins"))
                FindPlugins(AppDomain.CurrentDomain.BaseDirectory + "plugins");
        }

        public void FindPlugins(string path)
        {
            //First empty the collection, we're reloading them all
            _availablePlugins.Clear();
            AddPlugin(AppDomain.CurrentDomain.BaseDirectory + "MarketSage.Plugin.Core.dll");
            if (Directory.Exists(path))
                //Go through all the files in the plugin directory
                foreach (string fileOn in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                {
                    FileInfo file = new FileInfo(fileOn);

                    //Preliminary check, must be .dll
                    if (file.Extension.Equals(".dll"))
                    {
                        //Add the 'plugin'
                        this.AddPlugin(fileOn);
                    }
                }
        }

        public void ClosePlugins()
        {
            foreach (AvailablePlugin<IIndicator> pluginOn in _availablePlugins)
            {
                //Close all plugin instances
                //We call the plugins Dispose sub first incase it has to do 
                //Its own cleanup stuff
                //pluginOn.Instance.Dispose();

                //After we give the plugin a chance to tidy up, get rid of it
                pluginOn.Instance = null;
            }
            //Finally, clear our collection of available plugins
            _availablePlugins.Clear();
        }

        private void AddPlugin(string filename)
        {
            //Create a new assembly from the plugin file we're adding..
            Assembly pluginAssembly = Assembly.LoadFrom(filename);

            //Next we'll loop through all the Types found in the assembly
            foreach (Type pluginType in pluginAssembly.GetTypes())
            {
                if (pluginType.IsPublic) //Only look at public types
                {
                    //if (!pluginType.IsAbstract)  //Only look at non-abstract types
                    //{
                    //Gets a type object of the interface we need the plugins to match
                    Type typeInterface = pluginType.GetInterface("MarketSage.Library.IIndicator", true);

                    //Make sure the interface we want to use actually exists
                    if (typeInterface != null)
                    {
                        //Create a new available plugin since the type implements the IPlugin interface
                        AvailablePlugin<IIndicator> newPlugin = new AvailablePlugin<IIndicator>();
                        //Set the filename where we found it
                        newPlugin.AssemblyPath = filename;
                        newPlugin.PlugType = pluginType;
                        newPlugin.Name = pluginType.Name;
                        //Create a new instance and store the instance in the collection for later use
                        //We could change this later on to not load an instance.. we have 2 options
                        //1- Make one instance, and use it whenever we need it.. it's always there
                        //2- Don't make an instance, and instead make an instance whenever we use it, then close it
                        //For now we'll just make an instance of all the plugins
                        //newPlugin.Instance = (IIndicator)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));
                        //MessageBox.Show(pluginType.ToString());
                        //Set the Plugin's host to this class which inherited IPluginHost
                        //NOTE: no need to d this here
                        //newPlugin.Instance.Host = this;
                        //Call the initialization sub of the plugin
                        //newPlugin.Instance.Initialize();

                        //Add the new plugin to our collection here
                        this._availablePlugins.Add(newPlugin);

                        //cleanup a bit
                        newPlugin = null;
                    }
                    typeInterface = null; //Mr. Clean			
                    //}
                }
            }
            pluginAssembly = null; //more cleanup
        }
    }
}
