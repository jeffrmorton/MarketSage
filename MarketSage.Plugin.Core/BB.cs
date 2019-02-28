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
    /// <summary>Bollinger Bands</summary>
    public class BB : IIndicator
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _signal;
        private int _period;
        private int _stdDeviations;
        string myName = "Bollinger Bands";
        string myDescription = "Developed by John Bollinger, Bollinger Bands are an indicator that allows users to compare volatility and relative price levels over a period time. The indicator consists of three bands designed to encompass the majority of a security's price action.";
        string myAuthor = "John Bollinger";
        string myVersion = "1.0.0";

        /// <summary>Creates new BB object of IIndicator interface</summary>
        public BB()
        {
            _period = 20;
            _stdDeviations = 2;
            Initialize();
        }

        /// <summary>Creates new BB object of IIndicator interface</summary>
        /// <param name="period">(From 1 to 100000) Number of period</param>
        public BB(int period)
        {
            _period = period;
            _stdDeviations = 2;
            Initialize();
        }

        /// <summary>Creates new BB object of IIndicator interface</summary>
        /// <param name="period">(From 1 to 100000) Number of period</param>
        /// <param name="stdDeviations">(From 1 to 10) Number of standard deviations</param>
        public BB(int period, int stdDeviations)
        {
            _period = period;
            _stdDeviations = stdDeviations;
            Initialize();
        }

        public void Initialize()
        {
            _name = "BB." + _period + "," + _stdDeviations;
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

        /// <summary>Get average</summary>
        public static double GetAverage(double[] data)
        {
            int len = data.Length;
            if (len == 0)
                throw new Exception("No data");
            double sum = 0;
            for (int i = 0; i < data.Length; i++)
                sum += data[i];
            return sum / len;
        }

        /// <summary>Get variance</summary>
        public static double GetVariance(double[] data)
        {
            int len = data.Length;
            double avg = GetAverage(data);
            double sum = 0;
            for (int i = 0; i < data.Length; i++)
                sum += Math.Pow((data[i] - avg), 2);
            return sum / len;
        }

        /// <summary>Get standard deviation</summary>
        public static double GetStdev(double[] data)
        {
            return Math.Sqrt(GetVariance(data));
        }

        private void GenerateSignal()
        {
            double[] data = new double[_data.Count];
            for (int i = 0; i < _data.Count; i++)
                data[i] = ((TechnicalData)_data[i]).AdjClose;
            double middleBand = GetAverage(data);
            double upperBand = middleBand + (GetStdev(data) * _stdDeviations);
            double lowerBand = middleBand - (GetStdev(data) * _stdDeviations);
            double closeToday = ((TechnicalData)_data[_data.Count - 1]).AdjClose;

            if (closeToday >= upperBand)
            {
                _signal = -1; // SELL
            }
            else if (closeToday <= lowerBand)
            {
                _signal = 1;  // BUY
            }
            else
            {
                _signal = 0;  // HOLD
            }
        }
    }
}