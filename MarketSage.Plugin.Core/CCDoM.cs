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
    /// <summary>Calendrical Cycle - Day of Month</summary>
    public class CCDoM : IIndicator
    {
        private string _name;
        private bool _ready;
        private int _signal;
        private DateTime _date;
        private int _buyLevel = 25;
        private int _sellLevel = 5;
        string myName = "Calendrical Cycle - Day of Month";
        string myDescription = "Buy and sell on day of month";
        string myAuthor = "Jeffrey Morton";
        string myVersion = "1.0.0";

        /// <summary>
        /// 
        /// </summary>
        public CCDoM()
        {
            Initialize();
        }

        public CCDoM(int period)
        {
            Random rnd = new Random();
            _buyLevel = rnd.Next(23, 30);
            _sellLevel = rnd.Next(3, 8);
            Initialize();
        }

        public CCDoM(int period, int buylevel, int selllevel)
        {
            _buyLevel = buylevel;
            _sellLevel = selllevel;
            Initialize();
        }

        public void Initialize()
        {
            _name = "CCDoM." + "0," + _buyLevel.ToString() + "," + _sellLevel.ToString();
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
            if (_buyLevel == _date.Day)
                _signal = 1;
            if (_sellLevel == _date.Day)
                _signal = -1;
        }
    }
}