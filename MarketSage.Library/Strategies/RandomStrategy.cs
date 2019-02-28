using System;
using System.Collections;
using System.Text;
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
    public class RandomStrategy : AbstractStrategy
    {
        private ArrayList _indicators;
        private Random _randomClass;
        private int _minIndicators = 1;
        private int _maxIndicators = 7;
        private int _minPeriod = 1;
        private int _maxPeriod = 200;
        private PluginServices _pluginService;
        private bool Disposition = true;

        public RandomStrategy(ref PluginServices pluginService, int maxind, int maxper)
        {
            _randomClass = new Random();
            _indicators = new ArrayList();
            _entryName = "";
            _pluginService = pluginService;
            _maxIndicators = maxind;
            _maxPeriod = maxper;
            int indicators = _randomClass.Next(1, _maxIndicators + 1);
            for (int i = 0; i < indicators; i++)
            {
                int indicator = _randomClass.Next(1, _pluginService.AvailablePlugins.Count + 1) - 1;
                int period = _randomClass.Next(1, _maxPeriod);
                AvailablePlugin<IIndicator> plug = _pluginService.AvailablePlugins[indicator];
                Assembly pluginAssembly = Assembly.LoadFrom(plug.AssemblyPath);
                plug.Instance = (IIndicator)Activator.CreateInstance(pluginAssembly.GetType(plug.PlugType.ToString()), period);
                IIndicator rne = plug.Instance;
                _indicators.Add(rne);
            }
            if (Disposition == true)
            {
                for (int i = 0; i < _indicators.Count; i++)
                {
                    _entryName += ((IIndicator)_indicators[i]).GetName();
                    if (i != _indicators.Count - 1)
                        _entryName += ":";
                }
            }
            else
            {
                for (int i = 0; i < _indicators.Count; i++)
                {
                    _exitName += ((IIndicator)_indicators[i]).GetName();
                    if (i != _indicators.Count - 1)
                        _exitName += ":";
                }
            }
            /*
                        if (_exitFlag == true)
                        {
                            _exitIndicators.Add(new StandardExit(_maximumHoldingPeriodBars, _profitTragetVolatilityUnits, _moneyManagementStopVolatilityUnits));

                            for (int i = 0; i < _exitIndicators.Count; i++)
                            {
                                ExitIndicatorName += ((IIndicator)_exitIndicators[i]).GetName();
                                if (i != _exitIndicators.Count - 1)
                                    ExitIndicatorName += ":";
                            }
                        }
                          */
        }
        public RandomStrategy(ref PluginServices pluginService, int minind, int maxind, int minper, int maxper)
        {
            _randomClass = new Random();
            _indicators = new ArrayList();
            _entryName = "";
            _pluginService = pluginService;
            _minIndicators = minind;
            _maxIndicators = maxind;
            _minPeriod = minper;
            _maxPeriod = maxper;
            int indicators = _randomClass.Next(_minIndicators, _maxIndicators + 1);
            for (int i = 0; i < indicators; i++)
            {
                int indicator = _randomClass.Next(1, _pluginService.AvailablePlugins.Count + 1) - 1;
                int period = _randomClass.Next(_minPeriod, _maxPeriod);
                AvailablePlugin<IIndicator> plug = _pluginService.AvailablePlugins[indicator];
                Assembly pluginAssembly = Assembly.LoadFrom(plug.AssemblyPath);
                plug.Instance = (IIndicator)Activator.CreateInstance(pluginAssembly.GetType(plug.PlugType.ToString()), period);
                IIndicator rne = plug.Instance;
                _indicators.Add(rne);
            }
            if (Disposition == true)
            {
                for (int i = 0; i < _indicators.Count; i++)
                {
                    _entryName += ((IIndicator)_indicators[i]).GetName();
                    if (i != _indicators.Count - 1)
                        _entryName += ":";
                }
            }
            else
            {
                for (int i = 0; i < _indicators.Count; i++)
                {
                    _exitName += ((IIndicator)_indicators[i]).GetName();
                    if (i != _indicators.Count - 1)
                        _exitName += ":";
                }
            }
        }

        public RandomStrategy(ref PluginServices pluginService, int minind, int maxind, int minper, int maxper, bool disposition)
        {
            _randomClass = new Random();
            _indicators = new ArrayList();
            _entryName = "";
            _pluginService = pluginService;
            _minIndicators = minind;
            _maxIndicators = maxind;
            _minPeriod = minper;
            _maxPeriod = maxper;
            Disposition = disposition;
            int indicators = _randomClass.Next(_minIndicators, _maxIndicators + 1);
            for (int i = 0; i < indicators; i++)
            {
                int indicator = _randomClass.Next(1, _pluginService.AvailablePlugins.Count + 1) - 1;
                int period = _randomClass.Next(_minPeriod, _maxPeriod);
                AvailablePlugin<IIndicator> plug = _pluginService.AvailablePlugins[indicator];
                Assembly pluginAssembly = Assembly.LoadFrom(plug.AssemblyPath);
                plug.Instance = (IIndicator)Activator.CreateInstance(pluginAssembly.GetType(plug.PlugType.ToString()), period);
                IIndicator rne = plug.Instance;
                _indicators.Add(rne);
            }
            if (Disposition == true)
            {
                _entryName = GetSortedName(_indicators);
            }
            else
            {
                _exitName = GetSortedName(_indicators);
            }
        }

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
