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
    /// <summary>Linear Regression</summary>
    public class LR : IIndicator
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _signal;
        private int _period;
        string myName = "Linear Regression";
        string myDescription = "";
        string myAuthor = "";
        string myVersion = "1.0.0";

        /// <summary>Creates new LR object of IIndicator interface</summary>
        public LR()
        {
            _period = 14;
            Initialize();
        }

        /// <summary>Creates new LR object of IIndicator interface</summary>
        /// <param name="period">(From 1 to 100000) Number of period</param>
        public LR(int period)
        {
            _period = period;
            Initialize();
        }

        public void Initialize()
        {
            _name = "LR." + _period;
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

        public int Period
        {
            get { return _period; }
            set { _period = value; }
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
            double lr = 0;
            double close = ((TechnicalData)_data[_period - 1]).AdjClose;
            double sumx = _period * (_period - 1) * 0.5;
            double sumxsqr = _period * (_period - 1) * (2 * _period - 1) / 6;
            double divisor = sumx * sumx - _period * sumxsqr;
            double sumxy, sumy, m, b;

            sumxy = 0;
            sumy = 0;
            m = 0;
            b = 0;
            for (int i = _data.Count - 1; i >= 0; i--)
            {
                sumy += ((TechnicalData)_data[i]).AdjClose;
                sumxy += (double)i * ((TechnicalData)_data[i]).AdjClose;
            }
            m = (_period * sumxy - sumx * sumy) / divisor;
            b = (sumy - m * sumx) / (double)_period;
            lr = b + m * (double)(_period - 1);

            if (lr < close)
            {
                _signal = -1;
            }
            else if (lr > close)
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