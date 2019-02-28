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
    /// <summary>Aroon "Dawn’s early light" Oscillator</summary>
    public class AroonO : IIndicator
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _signal;
        private int _period;
        private double _buylevel;
        private double _selllevel;
        string myName = "AroonO";
        string myDescription = "Developed by Tushar Chande, the Aroon Oscillator is based upon his Aroon Indicator. Much like the Aroon Indicator, the Aroon Oscillator measures the strength of a trend.";
        string myAuthor = "Tushar Chande";
        string myVersion = "1.0.0";

        /// <summary>Creates new AroonO object of IIndicator interface</summary>
        public AroonO()
        {
            _period = 25;
            _buylevel = 30;
            _selllevel = -30;
            Initialize();
        }

        /// <summary>Creates new AroonO object of IIndicator interface</summary>
        /// <param name="period">(From 1 to 100000) Number of period</param>
        public AroonO(int period)
        {
            _period = period;
            _buylevel = 30;
            _selllevel = -30;
            Initialize();
        }

        /// <summary>Creates new AroonO object of IIndicator interface</summary>
        /// <param name="period">(From 1 to 100000) Number of period</param>
        /// <param name="buylevel">(From 1 to 100) Number of buy level</param>
        /// <param name="selllevel">(From -1 to -100) Number of sell Level</param>
        public AroonO(int period, double buylevel, double selllevel)
        {
            _period = period;
            _buylevel = buylevel;
            _selllevel = selllevel;
            Initialize();
        }

        public void Initialize()
        {
            _name = "AroonO." + _period + "," + _buylevel + "," + _selllevel;
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
            set
            {
                _period = value;
                _name = "AroonO." + _period;
            }
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
                if (_data.Count > _period)
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
            int lowestIdx = 0;
            int highestIdx = 0;
            double lowestLow = 9999.99;
            double highestHigh = 0000.00;
            double aroonUp, aroonDown;
            double aroonOscillator;

            for (int i = 0; i < _data.Count; i++)
            {
                if (((TechnicalData)_data[i]).AdjLow <= lowestLow)
                {
                    lowestLow = ((TechnicalData)_data[i]).AdjLow;
                    lowestIdx = i;
                }

                if (((TechnicalData)_data[i]).AdjHigh >= highestHigh)
                {
                    highestHigh = ((TechnicalData)_data[i]).AdjHigh;
                    highestIdx = i;
                }
            }

            aroonUp = ((_period - (_period - highestIdx)) / _period) * 100;
            aroonDown = ((_period - (_period - lowestIdx)) / _period) * 100;
            aroonOscillator = aroonUp - aroonDown;

            // Generate signal
            if (aroonOscillator >= _buylevel)
            {
                _signal = 1;
            }
            else if (aroonOscillator <= _selllevel)
            {
                _signal = -1;
            }
            else
            {
                _signal = 0;
            }
        }
    }
}