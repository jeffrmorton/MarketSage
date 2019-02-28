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
    /// Lunar Cycle - Zodiac
    /// </summary>
    public class LCZ : IIndicator
    {
        private string _name;
        private bool _ready;
        private int _signal;
        private DateTime _date;
        private double _age = 0.0;          // Moon's age
        private double _distance = 0.0;     // Moon's distance in earth radii
        private double _latitude = 0.0;     // Moon's ecliptic latitude
        private double _longtitude = 0.0;   // Moon's ecliptic longitude
        private int _zodiac = 0;            // Moon's zodiac sign 1 - 12
        private int _buySign = 1;
        private int _sellSign = 7;
        string myName = "Lunar Cycle - Zodiac";
        string myDescription = "Buy and sell at lunar zodiac";
        string myAuthor = "Jeffrey Morton";
        string myVersion = "1.0.0";

        /// <summary>Creates new LCZ object of IIndicator interface</summary>
        public LCZ()
        {
            Initialize();
        }

        public LCZ(int period)
        {
            Random rnd = new Random();
            _buySign = rnd.Next(1, 12);
            _sellSign = rnd.Next(1, 12);
            Initialize();
        }

        public LCZ(int period, double buylevel, double selllevel)
        {
            _buySign = (int)buylevel;
            _sellSign = (int)selllevel;
            Initialize();
        }

        public void Initialize()
        {
            _name = "LCZ." + "0," + _buySign.ToString() + "," + _sellSign.ToString();
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
            double DP = 0.0;
            double NP = 0.0;
            double RP = 0.0;

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

            IP = IP * 2 * Math.PI;                      // Convert phase to radians

            // calculate moon's distance
            DP = 2 * Math.PI * Normalize((JD - 2451562.2) / 27.55454988);
            _distance = 60.4 - 3.3 * Math.Cos(DP) - 0.6 * Math.Cos(2 * IP - DP) - 0.5 * Math.Cos(2 * IP);

            // calculate moon's ecliptic latitude
            NP = 2 * Math.PI * Normalize((JD - 2451565.2) / 27.212220817);
            _latitude = 5.1 * Math.Sin(NP);

            // calculate moon's ecliptic longitude
            RP = Normalize((JD - 2451555.8) / 27.321582241);
            _longtitude = 360 * RP + 6.3 * Math.Sin(DP) + 1.3 * Math.Sin(2 * IP - DP) + 0.7 * Math.Sin(2 * IP);

            if (_longtitude < 33.18) _zodiac = 1;  // "Pisces"
            else if (_longtitude < 51.16) _zodiac = 2;  // "Aries"
            else if (_longtitude < 93.44) _zodiac = 3;  // "Taurus"
            else if (_longtitude < 119.48) _zodiac = 4;  // "Gemini"
            else if (_longtitude < 135.30) _zodiac = 5;  // "Cancer"
            else if (_longtitude < 173.34) _zodiac = 6;  // "Leo"
            else if (_longtitude < 224.17) _zodiac = 7;  // "Virgo"
            else if (_longtitude < 242.57) _zodiac = 8;  // "Libra"
            else if (_longtitude < 271.26) _zodiac = 9;  // "Scorpio"
            else if (_longtitude < 302.49) _zodiac = 10;  // "Sagittarius"
            else if (_longtitude < 311.72) _zodiac = 11;  // "Capricorn"
            else if (_longtitude < 348.58) _zodiac = 12;  // "Aquarius"
            else _zodiac = 1;  // "Pisces"
            // so longitude is not greater than 360!
            if (_longtitude > 360) _longtitude = _longtitude - 360;

            _signal = 0;
            if (_buySign == _zodiac)
                _signal = 1;
            if (_sellSign == _zodiac)
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