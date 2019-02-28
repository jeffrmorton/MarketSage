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
    /// <summary></summary>
    public class FibR : IIndicator
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _direction;
        private int _period;
        string myName = "Fibonacci Retracement";
        string myDescription = "A term used in technical analysis that refers to the likelihood that a financial asset's price will retrace a large portion of an original move and find support or resistance at the key Fibonacci levels before it continues in the original direction. These levels are created by drawing a trendline between two extreme points and then dividing the vertical distance by the key Fibonacci ratios of 23.6%, 38.2%, 50%, 61.8% and 100%.";
        string myAuthor = "";
        string myVersion = "1.0.0";

        /// <summary>Creates new FibonacciRetracement object of IIndicator interface</summary>
        public FibR()
        {
            _period = 14;
            Initialize();
        }

        /// <summary>Creates new FibonacciRetracement object of IIndicator interface</summary>
        /// <param name="period">Period</param>
        public FibR(int period)
        {
            _period = period;
            Initialize();
        }

        public void Initialize()
        {
            _name = "FibR." + _period;
            _data = new ArrayList();
            _ready = false;
            _direction = 9;
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
            return _direction;
        }

        /// <summary>Indicates buy signal</summary>
        /// <returns>bool</returns>
        public bool IsBuy()
        {
            return _direction == 1;
        }

        /// <summary>Indicates hold signal</summary>
        /// <returns>bool</returns>
        public bool IsHold()
        {
            return _direction == 0;
        }

        /// <summary>Indicates sell signal</summary>
        /// <returns>bool</returns>
        public bool IsSell()
        {
            return _direction == -1;
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
            double peak = 0;
            double trough = 0;
            double support;
            double resistance;
            double current = ((TechnicalData)_data[_data.Count - 1]).AdjClose;

            for (int i = 0; i < _data.Count; i++)
            {
                if (peak == 0 || peak > ((TechnicalData)_data[i]).AdjClose)
                    peak = ((TechnicalData)_data[i]).AdjClose;
                if (trough == 0 || trough < ((TechnicalData)_data[i]).AdjClose)
                    trough = ((TechnicalData)_data[i]).AdjClose;
            }

            support = peak - ((peak - trough) * .618);
            resistance = peak - ((peak - trough) * .382);

            if (current > resistance)
                _direction = 1;
            else if (current < support)
                _direction = -1;
            else
                _direction = 0;
        }
    }
}