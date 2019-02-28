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
    /// <summary>Moving Average Convergence/Divergence</summary>
    public class MACD : IIndicator
    {
        private ArrayList _data;
        private ArrayList _avgdata;
        private string _name;
        private bool _ready;
        private int _direction;
        private int _longperiod = 26;
        private int _intperiod = 12;
        private int _signalperiod = 9;
        string myName = "Moving Average Convergence / Divergence";
        string myDescription = "Developed by Gerald Appel, Moving Average Convergence/Divergence (MACD) is one of the simplest and most reliable indicators available. MACD uses moving averages, which are lagging indicators, to include some trend-following characteristics. These lagging indicators are turned into a momentum oscillator by subtracting the longer moving average from the shorter moving average. The resulting plot forms a line that oscillates above and below zero, without any upper or lower limits. MACD is a centered oscillator and the guidelines for using centered oscillators apply.";
        string myAuthor = "Gerald Appel";
        string myVersion = "1.0.0";

        /// <summary>Creates new %R object of IIndicator interface</summary>
        public MACD()
        {
            _longperiod = 26;
            _intperiod = 12;
            _signalperiod = 9;
            Initialize();
        }

        public MACD(int longperiod)
        {
            longperiod = 26;
            _intperiod = 12;
            _signalperiod = 9;
            Initialize();
        }

        public MACD(int longperiod, int intperiod, int signalperiod)
        {
            _longperiod = longperiod;
            _intperiod = intperiod;
            _signalperiod = signalperiod;
            Initialize();
        }

        public void Initialize()
        {
            _name = "MACD." + _longperiod + "," + _intperiod + "," + _signalperiod;
            _data = new ArrayList();
            _avgdata = new ArrayList();
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
            _data.Add(data);

            if (_ready)
            {
                _data.RemoveAt(0);
            }

            if (_data.Count >= _longperiod)
            {
                double MACD = MovingAverage(_intperiod) - MovingAverage(_longperiod);
                _avgdata.Add((double)MACD);

                if (_avgdata.Count > _signalperiod + 1)
                {
                    _avgdata.RemoveAt(0);
                }
            }

            if (_data.Count > _longperiod + _signalperiod)
            {
                _ready = true;
                GenerateIndicator();
            }
        }

        private double MovingAverage(double period)
        {
            int end = _data.Count - 1;
            int start = (int)(_data.Count - period);
            double sum = 0;
            for (int i = start; i < end; i++)
            {
                sum += ((TechnicalData)_data[i]).AdjClose;
            }
            return sum / period;
        }

        private void GenerateIndicator()
        {
            double mov1 = 0;
            double mov2 = 0;

            double p1 = (double)(_avgdata[_signalperiod - 1]);
            double p2 = (double)(_avgdata[_signalperiod]);

            for (int i = 0; i < _signalperiod; i++)
            {
                mov1 += (double)(_avgdata[i]);
            }

            for (int i = 1; i <= _signalperiod; i++)
            {
                mov2 += (double)(_avgdata[i]);
            }

            mov1 = mov1 / _signalperiod;
            mov2 = mov2 / _signalperiod;

            if ((mov1 < p1) && (mov2 > p2))
            {
                _direction = -1;
            }
            else if ((mov1 > p1) && (mov2 < p2))
            {
                _direction = 1;
            }
            else
            {
                _direction = 0;
            }
        }
    }
}