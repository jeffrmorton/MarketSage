using System;
using System.Collections.Generic;
using System.Text;

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
    public class StandardIndicator : IIndicator
    {
        private string _name;
        private bool _ready = true;
        private int _signal;
        private DateTime _date;
        private double _age = 0.0;          // Moon's age
        private int _phase = 0;             // Moon's phase  1 - 8
        private int _buyPhase = 5;
        private int _sellPhase = 1;
        string myName = "Lunar Cycle Phase";
        string myDescription = "";
        string myAuthor = "Jeffrey Morton";
        string myVersion = "1.0.0";

        /// <summary>
        /// 
        /// </summary>
        public StandardIndicator()
        {
            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <param name="buylevel"></param>
        /// <param name="selllevel"></param>
        public StandardIndicator(int period, int buylevel, int selllevel)
        {
            _buyPhase = buylevel;
            _sellPhase = selllevel;
            Initialize();
        }

        public void Initialize()
        {
            _name = "Standard." + "0," + _buyPhase.ToString() + "," + _sellPhase.ToString();
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
            double Y = _date.Year;
            double M = _date.Month;
            double D = _date.Day;

            double YY = 0.0;
            double MM = 0.0;
            double K1 = 0.0;
            double K2 = 0.0;
            double K3 = 0.0;
            double JD = 0.0;
            double IP = 0.0;

            // calculate the Julian date at 12h UT
            YY = Y - Math.Floor((12 - M) / 10);
            MM = M + 9;
            if (MM >= 12) MM = MM - 12;

            K1 = Math.Floor(365.25 * (YY + 4712));
            K2 = Math.Floor(30.6 * MM + 0.5);
            K3 = Math.Floor(Math.Floor((YY / 100) + 49) * 0.75) - 38;

            JD = K1 + K2 + D + 59;                 // for dates in Julian calendar
            if (JD > 2299160) JD = JD - K3;        // for Gregorian calendar

            // calculate moon's age in days
            IP = Normalize((JD - 2451550.1) / 29.530588853);
            _age = IP * 29.53;

            if (_age < 1.84566) _phase = 1;  // "New"
            else if (_age < 5.53699) _phase = 2;  // "Evening crescent"
            else if (_age < 9.22831) _phase = 3;  // "First quarter"
            else if (_age < 12.91963) _phase = 4;  // "Waxing gibbous"
            else if (_age < 16.61096) _phase = 5;  // "Full"
            else if (_age < 20.30228) _phase = 6;  // "Waning gibbous"
            else if (_age < 23.99361) _phase = 7;  // "Last quarter"
            else if (_age < 27.68493) _phase = 8;  // "Morning crescent"
            else _phase = 1;  // "New"

            _signal = 0;
            if (_buyPhase == _phase)
                _signal = 1;
            if (_sellPhase == _phase)
                _signal = -1;
        }

        /// <summary>
        /// Normalize values to range 0...1    
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private double Normalize(double v)
        {
            v = v - Math.Floor(v);
            if (v < 0)
                v = v + 1;
            return v;
        }
    }
}
