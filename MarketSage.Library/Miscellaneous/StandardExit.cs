using System;
using System.Collections;

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
    /// </summary>
    public class StandardExit
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _signal;
        private int _period = 50; // 50 Day Moving Average
        private int _maximumHoldingPeriodBars;
        private int _profitTargetVolatilityUnits;
        private int _moneyManagementStopVolatilityUnits;
        //private int _maximumLossPercentage = 7;
        private double _top;
        string myName = "Standard Exit";
        string myDescription = "";
        string myAuthor = "Jeffrey Morton";
        string myVersion = "1.0.0";
        private Account _account;

        public StandardExit()
        {
            _maximumHoldingPeriodBars = 20;
            _profitTargetVolatilityUnits = 4;
            _moneyManagementStopVolatilityUnits = 2;
            _name = "StandardExit." + _maximumHoldingPeriodBars + "," + _profitTargetVolatilityUnits + "," + _moneyManagementStopVolatilityUnits;
            _data = new ArrayList();
        }

        public StandardExit(int maximumHoldingPeriodBars, int profitTragetVolatilityUnits, int moneyManagementStopVolatilityUnits)
        {
            _maximumHoldingPeriodBars = maximumHoldingPeriodBars;
            _profitTargetVolatilityUnits = profitTragetVolatilityUnits;
            _moneyManagementStopVolatilityUnits = moneyManagementStopVolatilityUnits;
            _name = "StandardExit." + _maximumHoldingPeriodBars + "," + _profitTargetVolatilityUnits + "," + _moneyManagementStopVolatilityUnits;
            _data = new ArrayList();
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
        public virtual void AddData(MarketSage.Library.TechnicalData data, ref Account account)
        {
            _account = account;
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
            if (_account._inPlay == true)
            {
                double closeEntry = ((TechnicalData)_data[0]).AdjClose;
                double closeToday = ((TechnicalData)_data[_data.Count - 1]).AdjClose;
                double[] tr = new double[_data.Count];
                double atr;
                _signal = 0;

                if (closeToday > _top)
                    _top = closeToday;

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

                // Generate signal
                if (_account.DaysInPlay >= _maximumHoldingPeriodBars)
                {
                    _signal = -1;
                }
                /*
                if (_top - ((_top / 100) * _maximumLossPercentage) > closeToday)
                {
                    _signal = -1;
                }
                if (_account.Basis - ((_account.Basis / 100) * _maximumLossPercentage) > _account.PositionValue)
                {
                    _signal = -1;
                }
                 */
                if (closeToday > (closeEntry + (atr * _profitTargetVolatilityUnits)))
                {
                    _signal = -1;
                }
                //if (closeToday < (closeEntry - (atr * _moneyManagementStopVolatilityUnits)))
                //{
                //    _signal = -1;
                //}
                if (closeToday < (_top - (atr * _moneyManagementStopVolatilityUnits)))
                {
                    _signal = -1;
                }
            }
            else
            {
                _top = 0;
                _signal = 0;
            }
        }
    }
}