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
    /// Genetic strategy implementation of AbstractStrategy object
    /// </summary>
    public class GeneticStrategy : AbstractStrategy
    {
        private ArrayList _entryIndicators;
        private ArrayList _exitIndicators;
        private int _maxIndicators = 7; // 21
        //private ArrayList _genes;
        //private Random _randomClass;
        private PluginServices _pluginService;
        /*
                /// <summary>
                /// Genetic strategy constructor
                /// </summary>
                public GeneticStrategy(ArrayList genes)
                {
                    _genes = genes;
                    Initialize();
                }
         */

        /// <summary>
        /// Genetic strategy constructor
        /// </summary>
        public GeneticStrategy(ref PluginServices pluginService, string entry, string exit)
        {
            _pluginService = pluginService;
            _entryIndicators = Resolve(entry);
            EntryIndicatorName = GetSortedName(_entryIndicators);
            _exitIndicators = Resolve(exit);
            ExitIndicatorName = GetSortedName(_exitIndicators);
            Initialize();
        }

        /// <summary>
        /// Genetic strategy constructor
        /// </summary>
        public GeneticStrategy(ref PluginServices pluginService, string genome)
        {
            _pluginService = pluginService;
            string[] dataArray = new string[2];
            Regex rex = new Regex("&(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            dataArray = rex.Split(genome);
            _entryIndicators = Resolve(dataArray[0]);
            EntryIndicatorName = GetSortedName(_entryIndicators);
            _exitIndicators = Resolve(dataArray[1]);
            ExitIndicatorName = GetSortedName(_exitIndicators);
            Initialize();
        }

        /// <summary>
        /// Genetic strategy constructor
        /// </summary>
        public GeneticStrategy(ref PluginServices pluginService, ArrayList entry, ArrayList exit)
        {
            _pluginService = pluginService;
            _entryIndicators = Resolve(entry);
            EntryIndicatorName = GetSortedName(_entryIndicators);
            _exitIndicators = Resolve(exit);
            ExitIndicatorName = GetSortedName(_exitIndicators);
            Initialize();
        }

        private ArrayList Resolve(ArrayList genes)
        {
            ArrayList indicators = new ArrayList();
            Random rnd = new Random();
            int chance = 7;
            int number = rnd.Next(1, _maxIndicators);

            for (int i = 0; i < number; i++)
            {
                int indicator = rnd.Next(0, genes.Count - 1);
                string[] dataArray2 = new string[2];
                Regex rex2 = new Regex("\\.(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                dataArray2 = rex2.Split(genes[indicator].ToString());
                string[] dataArray3 = new string[3];
                Regex rex3 = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                dataArray3 = rex3.Split(dataArray2[1]);
                int period = int.Parse(dataArray3[0]);
                // Mutate
                if (rnd.Next(1, 10) == chance)
                {
                    if (rnd.Next(0, 1) == 0)
                    {
                        period = period - 1;
                    }
                    else
                    {
                        period = period + 1;
                    }
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
                        indicators.Add(rne);
                    }
                }
            }
            return indicators;
        }

        /*
        /// <summary>
        /// Implement strategy
        /// </summary>
        private void Resolve()
        {
            _indicators = new ArrayList();
            _randomClass = new Random();
            EntryIndicatorName = "";

            int chance = 7;
            int indicators = _randomClass.Next(1, _maxIndicators);

            for (int i = 0; i < indicators; i++)
            {
                int indicator = _randomClass.Next(1, _genes.Count);

                string[] dataArray2 = new string[2];
                Regex rex2 = new Regex("\\.(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                dataArray2 = rex2.Split(_genes[indicator].ToString());

                string[] dataArray3 = new string[3];
                Regex rex3 = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                dataArray3 = rex3.Split(dataArray2[1]);

                int period = int.Parse(dataArray3[0]);

                // Mutate
                if (_randomClass.Next(1, 10) == chance)
                {
                    if (_randomClass.Next(0, 1) == 0)
                    {
                        period = period - 1;
                    }
                    else
                    {
                        period = period + 1;
                    }
                }

                switch (dataArray2[0])
                {
                    case "DeM":
                        _indicators.Add(new DeM(period));
                        break;
                    case "FibR":
                        _indicators.Add(new FibR(period));
                        break;
                    case "MACD":
                        _indicators.Add(new MACD());
                        break;
                    case "MFI":
                        _indicators.Add(new MFI(period));
                        break;
                    case "ROC":
                        _indicators.Add(new ROC(period));
                        break;
                    case "RSI":
                        _indicators.Add(new RSI(period));
                        break;
                    case "BB":
                        _indicators.Add(new BB(period));
                        break;
                    case "SMA":
                        _indicators.Add(new SMA(period));
                        break;
                    case "EMA":
                        _indicators.Add(new EMA(period));
                        break;
                    case "WMA":
                        _indicators.Add(new WMA(period));
                        break;
                    case "WillR":
                        _indicators.Add(new WillR(period));
                        break;
                    case "LR":
                        _indicators.Add(new LR(period));
                        break;
                    case "OBV":
                        _indicators.Add(new OBV(period));
                        break;
                    case "BOP":
                        _indicators.Add(new BOP(period));
                        break;
                    case "VWAP":
                        _indicators.Add(new VWAP(period));
                        break;
                    case "Aroon":
                        _indicators.Add(new Aroon(period));
                        break;
                    case "AroonO":
                        _indicators.Add(new AroonO(period));
                        break;
                    case "CCI":
                        _indicators.Add(new CCI(period));
                        break;
                    case "ATR":
                        _indicators.Add(new ATR(period));
                        break;
                    case "MinVol":
                        _indicators.Add(new MinVol(period));
                        break;
                    case "BuyandHold":
                        _indicators.Add(new BuyandHold());
                        break;
                    case "MaxVol":
                        _indicators.Add(new MaxVol(period));
                        break;
                    case "PrevClose":
                        _indicators.Add(new PrevClose(period));
                        break;
                    case "AO":
                        _indicators.Add(new AO(int.Parse(dataArray3[0])));
                        break;
                    case "AC":
                        _indicators.Add(new AC(int.Parse(dataArray3[0])));
                        break;
                    case "LCP":
                        _indicators.Add(new LCP());
                        break;
                    case "LCZ":
                        _indicators.Add(new LCZ());
                        break;
                    default:
                        MessageBox.Show("Got unexpected result" + "\r\n" + dataArray2[0].ToString());
                        break;
                }
            }

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
        }
         */

        private ArrayList Resolve(string genome)
        {
            ArrayList indicators = new ArrayList();
            string[] dataArray1 = new string[_maxIndicators];
            Regex rex1 = new Regex(":(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            dataArray1 = rex1.Split(genome);
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
                        indicators.Add(rne);
                    }
                }
            }
            return indicators;
        }

        /// <summary>
        /// Initialize strategy
        /// </summary>
        public override void Initialize()
        {
            CompoundIndicator entryIndicator = new CompoundIndicator(_entryIndicators, EntryIndicatorName);
            EntryIndicator = entryIndicator;
            CompoundIndicator exitIndicator = new CompoundIndicator(_exitIndicators, ExitIndicatorName);
            ExitIndicator = exitIndicator;
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
