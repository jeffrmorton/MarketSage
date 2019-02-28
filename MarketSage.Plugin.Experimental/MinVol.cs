using System;
using System.Collections;
using MarketSage.Library;

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

namespace MarketSage.Plugin
{
    /// <summary>Minimum Volume</summary>
    public class MinVol : IIndicator
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _signal;
        private int _period;
        private double _minimum;
        string myName = "Minimum Volume";
        string myDescription = "";
        string myAuthor = "Jeffrey Morton";
        string myVersion = "1.0.0";


        /// <summary>Creates new MinVol object of IIndicator interface</summary>
        public MinVol()
        {
            _period = 20;  // default
            _minimum = 500000; // default
            Initialize();
        }

        /// <summary>Creates new MinVol object of IIndicator interface</summary>
        /// <param name="period">(From 1 to 100000) Number of period</param>
        public MinVol(int period)
        {
            _period = period;
            _minimum = 500000;
            Initialize();
        }

        /// <summary>Creates new MinVol object of IIndicator interface</summary>
        /// <param name="period">(From 1 to 100000) Number of period</param>
        public MinVol(int period, int minimum)
        {
            _period = period;
            _minimum = minimum;
            Initialize();
        }

        public void Initialize()
        {
            _name = "MinVol." + _period + "," + _minimum;
            _data = new ArrayList();
            _ready = false;
            _signal = 9;
        }


        public int Period
        {
            get { return _period; }
            set { _period = value; }
        }

        public string Name
        {
            get { return myName; }
        }
        public string Description
        {
            get { return myDescription; }
        }
        public string Author
        {
            get { return myAuthor; }
        }
        public string Version
        {
            get { return myVersion; }
        }

        /// <summary>Returns name</summary>
        /// <returns>string</returns>
        public string GetName()
        {
            return _name;
        }

        /// <summary>Returns readyness of the indicator</summary>
        /// <returns>bool</returns>
        public bool IsReady()
        {
            return _ready;
        }

        /// <summary>Returns direction</summary>
        /// <returns>int</returns>
        public int GetDirection()
        {
            return _signal;
        }

        /// <summary>Indicates buy signal</summary>
        /// <returns>bool</returns>
        public bool IsBuy()
        {
            return _signal == 1;
        }

        /// <summary>Indicates hold signal</summary>
        /// <returns>bool</returns>
        public bool IsHold()
        {
            return _signal == 0;
        }

        /// <summary>Indicates sell signal</summary>
        /// <returns>bool</returns>
        public bool IsSell()
        {
            return _signal == -1;
        }

        /// <summary>Adds TechnicalData object to the indicator</summary>
        /// <param name="data">TechnicalData object to be added</param>
        public virtual void AddData(TechnicalData data)
        {
            if (!_ready)
            {
                _data.Add(data);

                if (_data.Count >= _period)
                {
                    _ready = true;
                    GenerateSignal();
                }
            }
            else
            {
                _data.RemoveAt(0);
                _data.Add(data);
                GenerateSignal();
            }
        }

        private void GenerateSignal()
        {
            double ma = 0;

            for (int i = 0; i < _period; i++)
            {
                ma += ((TechnicalData)_data[i]).AdjVolume;
            }
            ma = ma / _period;

            if (ma < (_minimum))
            {
                _signal = -1;
            }
            else if (ma > (_minimum))
            {
                _signal = 1;
            }
            else
            {
                _signal = 0;
            }
        }
    }
}