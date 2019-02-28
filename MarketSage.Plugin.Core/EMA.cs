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
    /// <summary>Exponential Moving Average</summary>
    public class EMA : IIndicator
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _signal;
        private int _period;
        string myName = "Exponential Moving Average";
        string myDescription = "";
        string myAuthor = "";
        string myVersion = "1.0.0";

        /// <summary>Creates new EMA object of IIndicator interface</summary>
        public EMA()
        {
            _period = 20;  // Default
            Initialize();
        }

        /// <summary>Creates new EMA object of IIndicator interface</summary>
        /// <param name="period">(From 1 to 100000) Number of period</param>
        public EMA(int period)
        {
            _period = period;
            Initialize();
        }

        public void Initialize()
        {
            _name = "EMA." + _period;
            _data = new ArrayList();
            _ready = false;
            _signal = 9;
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

        public int Period
        {
            get { return _period; }
            set { _period = value; }
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
            double close = ((TechnicalData)_data[_period - 1]).AdjClose;
            double smoothingfactor = 2 / (_period + 1);

            for (int i = 0; i < _period; i++)
            {
                ma = ((((TechnicalData)_data[i]).AdjClose - ma) * smoothingfactor) + ma;
            }

            if (ma > close)
            {
                _signal = -1;
            }
            else if (ma < close)
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