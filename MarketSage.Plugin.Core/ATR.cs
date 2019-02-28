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
    /// <summary> 
    /// Average True Range
    /// Determines average true range for the period and compares the true
    /// range from the current day to the first day fo the period.  If the
    /// true range crosses some multiple of the average than a signal is produced.
    /// </summary>
    public class ATR : IIndicator
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _signal;
        private int _period;
        private int _multiple;
        string myName = "Average True Range";
        string myDescription = "Developed by J. Welles Wilder and introduced in his book, New Concepts in Technical Trading Systems (1978), the Average True Range (ATR) indicator measures a security's volatility. As such, the indicator does not provide an indication of price direction or duration, simply the degree of price movement or volatility.";
        string myAuthor = "J. Welles Wilder";
        string myVersion = "1.0.0";

        /// <summary>Creates new ATR object of IIndicator interface</summary>
        public ATR()
        {
            _period = 14;
            _multiple = 2;
            Initialize();
        }

        /// <summary>Creates new ATR object of IIndicator interface</summary>
        /// <param name="period">(From 1 to 100000) Number of period</param>
        public ATR(int period)
        {
            _period = period;
            _multiple = 2;
            Initialize();
        }

        /// <summary>Creates new ATR object of IIndicator interface</summary>
        /// <param name="period">(From 1 to 100000) Number of period</param>
        /// <param name="multiple">(From 1 to 100000) Number of multiple</param>
        public ATR(int period, int multiple)
        {
            _period = period;
            _multiple = multiple;
            Initialize();
        }

        public void Initialize()
        {
            _name = "ATR." + _period + "," + _multiple;
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

        private double GetTrueRange(double lowtoday, double hightoday, double closeyesterday)
        {
            double val1, val2, val3, greatest;

            val1 = hightoday - lowtoday;
            val2 = Math.Abs(closeyesterday - hightoday);
            val3 = Math.Abs(closeyesterday - lowtoday);

            greatest = val1;
            if (val2 > greatest)
                greatest = val2;
            if (val3 > greatest)
                greatest = val3;

            return greatest;
        }

        private void GenerateSignal()
        {
            double closeEntry = ((TechnicalData)_data[0]).AdjClose;
            double closeToday = ((TechnicalData)_data[_data.Count - 1]).AdjClose;
            double[] tr = new double[_data.Count];
            double atr, points;

            // Calculate ATR
            for (int i = 0; i < _data.Count; i++)
            {
                if (i == 0)
                    tr[i] = ((TechnicalData)_data[i]).AdjHigh - ((TechnicalData)_data[i]).AdjLow;
                else
                {
                    tr[i] = GetTrueRange(((TechnicalData)_data[i]).AdjLow, ((TechnicalData)_data[i]).AdjHigh, ((TechnicalData)_data[i - 1]).AdjClose);
                }
            }
            atr = GetAverage(tr);
            points = atr * _multiple;

            // Generate signal
            if (closeToday > (closeEntry + points))
            {
                _signal = 1;
            }
            if (closeToday < (closeEntry - points))
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