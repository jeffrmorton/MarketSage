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
    /// <summary>On Balance Volume</summary>
    public class OBV : IIndicator
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _signal;
        private int _period;
        string myName = "On Balance Volume";
        string myDescription = "Joe Granville introduced the On Balance Volume (OBV) indicator in his 1963 book, Granville's New Key to Stock Market Profits. This was one of the first and most popular indicators to measure positive and negative volume flow. The concept behind the indicator: volume precedes price. OBV is a simple indicator that adds a period's volume when the close is up and subtracts the period's volume when the close is down. A cumulative total of the volume additions and subtractions forms the OBV line. This line can then be compared with the price chart of the underlying security to look for divergences or confirmation.";
        string myAuthor = "Joe Granville";
        string myVersion = "1.0.0";

        /// <summary>Creates new OBV object of IIndicator interface</summary>
        public OBV()
        {
            _period = 14;
            Initialize();
        }

        /// <summary>Creates new OBV object of IIndicator interface</summary>
        /// <param name="period">(From 1 to 100000) Number of period</param>
        public OBV(int period)
        {
            _period = period;
            Initialize();
        }

        public void Initialize()
        {
            _name = "OBV." + _period;
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
        public void AddData(TechnicalData data)
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
            double upsum = 0;
            double downsum = 0;
            double closetoday, closeyesterday, volumetoday;

            for (int i = 1; i <= _period; i++)
            {
                closetoday = ((TechnicalData)_data[i]).AdjClose;
                closeyesterday = ((TechnicalData)_data[i - 1]).AdjClose;
                volumetoday = ((TechnicalData)_data[i]).AdjVolume;
                if (closetoday > closeyesterday)
                {
                    upsum += volumetoday;
                }
                if (closetoday < closeyesterday)
                {
                    downsum += volumetoday;
                }
            }

            if (upsum < downsum)
            {
                _signal = -1;
            }
            else if (upsum > downsum)
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