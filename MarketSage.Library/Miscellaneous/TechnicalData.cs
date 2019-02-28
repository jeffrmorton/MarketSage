using System;

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
    /// TechnicalData object
    /// </summary>
    public class TechnicalData
    {
        private DateTime _date;
        private double _open;
        private double _adjopen;
        private double _high;
        private double _adjhigh;
        private double _low;
        private double _adjlow;
        private double _close;
        private double _adjclose;
        private long _volume;
        private long _adjvolume;

        /*
        /// <summary>
        /// TechnicalData constructor
        /// </summary>
        /// <param name="date">DateTime</param>
        /// <param name="open">double</param>
        /// <param name="high">double</param>
        /// <param name="low">double</param>
        /// <param name="close">double</param>
        /// <param name="volume">int</param>
        public TechnicalData(DateTime date, double open, double high, double low, double close, long volume)
        {
            _date = date;
            _open = open;
            _high = high;
            _low = low;
            _close = close;
            _volume = volume;
        }
         */

        /// <summary>
        /// TechnicalData constructor
        /// </summary>
        /// <param name="date">DateTime</param>
        /// <param name="open">double</param>
        /// <param name="high">double</param>
        /// <param name="low">double</param>
        /// <param name="close">double</param>
        /// <param name="volume">int</param>
        /// <param name="adjclose">double</param>
        public TechnicalData(DateTime date, double open, double high, double low, double close, long volume, double adjclose)
        {
            _date = date;
            _open = open;
            _high = high;
            _low = low;
            _close = close;
            _volume = volume;
            _adjclose = adjclose;
            _adjlow = _low * adjclose / close;
            _adjhigh = _high * adjclose / close;
            _adjopen = _open * adjclose / close;
            _adjvolume = volume;  // TO DO
        }

        /// <summary>
        /// Get date
        /// </summary>
        /// <reutrns>DateTime</reutrns>
        virtual public DateTime Date
        {
            get { return _date; }
        }

        /// <summary>
        /// Get open
        /// </summary>
        /// <returns>double</returns>
        virtual public double Open
        {
            get { return _open; }
        }

        /// <summary>
        /// Get open
        /// </summary>
        /// <returns>double</returns>
        virtual public double AdjOpen
        {
            get { return _adjopen; }
        }
        
        /// <summary>
        /// Get high
        /// </summary>
        /// <returns>double</returns>
        virtual public double High
        {
            get { return _high; }
        }

        /// <summary>
        /// Get high
        /// </summary>
        /// <returns>double</returns>
        virtual public double AdjHigh
        {
            get { return _adjhigh; }
        }

        /// <summary>
        /// Get low
        /// </summary>
        /// <returns>double</returns>
        virtual public double Low
        {
            get { return _low; }
        }

        /// <summary>
        /// Get low
        /// </summary>
        /// <returns>double</returns>
        virtual public double AdjLow
        {
            get { return _adjlow; }
        }

        /// <summary>
        /// Get close
        /// </summary>
        /// <returns>double</returns>
        virtual public double Close
        {
            get { return _close; }
        }

        /// <summary>
        /// Get adjusted close
        /// </summary>
        /// <returns>double</returns>
        virtual public double AdjClose
        {
            get { return _adjclose; }
        }

        /// <summary>
        /// Get volume
        /// </summary>
        /// <returns>long</returns>
        virtual public long Volume
        {
            get { return _volume; }
        }

        /// <summary>
        /// Get volume
        /// </summary>
        /// <returns>long</returns>
        virtual public long AdjVolume
        {
            get { return _adjvolume; }
        }
    }
}