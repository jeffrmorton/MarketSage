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
    /// <summary>Calendrical Cycle - Month of Year</summary>
    public class CCMoY : IIndicator
    {
        private string _name;
        private bool _ready;
        private int _signal;
        private DateTime _date;
        private int _buyLevel = 11;  // Buy in November
        private int _sellLevel = 5;  // "Sell in May and go away"
        string myName = "Calendrical Cycle - Month of Year";
        string myDescription = "Buy and sell on month of year";
        string myAuthor = "Jeffrey Morton";
        string myVersion = "1.0.0";

        /// <summary>
        /// 
        /// </summary>
        public CCMoY()
        {
            Initialize();
        }

        public CCMoY(int period)
        {
            Random rnd = new Random();
            _buyLevel = rnd.Next(11, 16);
            if (_buyLevel > 12)
                _buyLevel = _buyLevel - 12;
            _sellLevel = rnd.Next(5, 10);
            Initialize();
        }

        public CCMoY(int period, int buylevel, int selllevel)
        {
            _buyLevel = buylevel;
            _sellLevel = selllevel;
            Initialize();
        }

        public void Initialize()
        {
            _name = "CCMoY." + "0," + _buyLevel.ToString() + "," + _sellLevel.ToString();
            _ready = true;
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

        /// <summary>
        /// Name of indicator
        /// </summary>
        /// <returns>Name</returns>
        public string GetName()
        {
            return _name;
        }

        /// <summary>Returns direction</summary>
        /// <returns>int</returns>
        public int GetDirection()
        {
            return _signal;
        }

        /// <summary>Returns readyness of the indicator</summary>
        /// <returns>bool</returns>
        public bool IsReady()
        {
            return _ready;
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
            _date = ((TechnicalData)data).Date;
            GenerateSignal();
        }

        /// <summary>
        /// 
        /// </summary>
        private void GenerateSignal()
        {
            _signal = 0;
            if (_buyLevel == _date.Month)
                _signal = 1;
            if (_sellLevel == _date.Month)
                _signal = -1;
        }
    }
}