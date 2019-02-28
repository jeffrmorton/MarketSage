using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
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
    /// <summary>
    /// TestStrategy implementation of AbstractStrategy object<
    /// /summary>
    public class TestStrategy : AbstractStrategy
    {
        private string _strategy;
        private ArrayList _indicators;
        private ArrayList _exitIndicators;
        private int _maxIndicators = 20;
        private PluginServices _pluginService;
        private bool Disposition = true;

        /// <summary>
        /// TestStrategy constructor
        /// </summary>
        /// <param name="strategy">string</param>
        public TestStrategy(ref PluginServices pluginService, string strategy)
        {
            _pluginService = pluginService;
            _strategy = strategy;
            Disposition = true;
            Resolve();
            Initialize();
        }

        public TestStrategy(ref PluginServices pluginService, string strategy, bool disposition)
        {
            _pluginService = pluginService;
            _strategy = strategy;
            Disposition = disposition;
            Resolve();
            Initialize();
        }

        private void Resolve()
        {
            _indicators = new ArrayList();
            _exitIndicators = new ArrayList();
            EntryIndicatorName = "";
            ExitIndicatorName = "";
            string[] dataArray1 = new string[_maxIndicators];
            Regex rex1 = new Regex(":(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            dataArray1 = rex1.Split(_strategy);
            for (int i = 0; i < dataArray1.Length; i++)
            {
                string[] dataArray2 = new string[2];
                Regex rex2 = new Regex("\\.(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                dataArray2 = rex2.Split(dataArray1[i]);
                string[] dataArray3 = new string[3];
                Regex rex3 = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                if (dataArray2[1] != null)
                {
                    dataArray3 = rex3.Split(dataArray2[1]);
                }
                foreach (AvailablePlugin<IIndicator> plug in _pluginService.AvailablePlugins)
                {
                    if (plug.Name == dataArray2[0])
                    {
                        Assembly pluginAssembly = Assembly.LoadFrom(plug.AssemblyPath);
                        if (dataArray3.Length == 3)
                            plug.Instance = (IIndicator)Activator.CreateInstance(pluginAssembly.GetType(plug.PlugType.ToString()), int.Parse(dataArray3[0]), int.Parse(dataArray3[1]), int.Parse(dataArray3[2]));
                        else if (dataArray3.Length == 2)
                            plug.Instance = (IIndicator)Activator.CreateInstance(pluginAssembly.GetType(plug.PlugType.ToString()), int.Parse(dataArray3[0]), int.Parse(dataArray3[1]));
                        else if (dataArray3.Length == 1)
                            plug.Instance = (IIndicator)Activator.CreateInstance(pluginAssembly.GetType(plug.PlugType.ToString()), int.Parse(dataArray3[0]));
                        else
                            plug.Instance = (IIndicator)Activator.CreateInstance(pluginAssembly.GetType(plug.PlugType.ToString()));
                        IIndicator rne = plug.Instance;
                        _indicators.Add(rne);
                    }
                }
            }
            if (Disposition == true)
            {
                _entryName = GetSortedName(_indicators);
            }
            else
            {
                _exitName = GetSortedName(_indicators);
            }
            /*

                        for (int i = 0; i < _indicators.Count; i++)
                        {
                            EntryIndicatorName += ((IIndicator)_indicators[i]).GetName();
                            if (i != _indicators.Count - 1)
                                EntryIndicatorName += ":";
                        }
                        if (_exitFlag == true)
                        {
                            _exitIndicators = new ArrayList();
                            _exitIndicators.Add(new StandardExit(_maximumHoldingPeriodBars, _profitTargetVolatilityUnits, _moneyManagementStopVolatilityUnits));
                            for (int i = 0; i < _exitIndicators.Count; i++)
                            {
                                ExitIndicatorName += ((IIndicator)_exitIndicators[i]).GetName();
                                if (i != _exitIndicators.Count - 1)
                                    ExitIndicatorName += ":";
                            }
                        }
                         */
        }

        /// <summary>
        /// Initialize strategy
        /// </summary>
        public override void Initialize()
        {
            if (Disposition == true)
            {
                CompoundIndicator entryIndicator = new CompoundIndicator(_indicators, EntryIndicatorName);
                EntryIndicator = entryIndicator;
                CompoundIndicator exitIndicator = new CompoundIndicator(new StandardIndicator());
                ExitIndicator = exitIndicator;
            }
            else
            {
                CompoundIndicator entryIndicator = new CompoundIndicator(new StandardIndicator());
                EntryIndicator = entryIndicator;
                CompoundIndicator exitIndicator = new CompoundIndicator(_indicators, ExitIndicatorName);
                ExitIndicator = exitIndicator;
            }
            /*
            CompoundIndicator indicator = new CompoundIndicator(_indicators, EntryIndicatorName);
            EntryIndicator = indicator;
            if (_exitFlag == true)
            {
                StandardExit exitIndicator = new StandardExit(_maximumHoldingPeriodBars, _profitTargetVolatilityUnits, _moneyManagementStopVolatilityUnits);
                StandardExitIndicator = exitIndicator;
            }
             */
            _pendingOrders = new ArrayList();
            _tickEvents = new ArrayList();
        }

        string GetSortedName(ArrayList _indicators)
        {
            string name = "";
            ArrayList array = new ArrayList();
            for (int i = 0; i < _indicators.Count; i++)
            {
                array.Add(((IIndicator)_indicators[i]).GetName());
            }
            array.Sort();
            for (int i = 0; i < array.Count; i++)
            {
                name += ((string)array[i]);
                if (i != array.Count - 1)
                    name += ":";
            }
            return name;
        }
    }
}
