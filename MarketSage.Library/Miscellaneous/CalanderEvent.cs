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
    /// CalanderEvent class
    /// </summary>
    public class CalanderEvent
    {
        private const int YEAR = 0;
        private const int MONTH = 1;
        private const int DAY = 2;
        private int _type;

        public static readonly CalanderEvent YEAR_EVENT = new CalanderEvent(YEAR);
        public static readonly CalanderEvent MONTH_EVENT = new CalanderEvent(MONTH);
        public static readonly CalanderEvent DAY_EVENT = new CalanderEvent(DAY);

        /// <summary>
        /// Event constructor
        /// </summary>
        /// <param name="type">int</param>
        internal CalanderEvent(int type)
        {
            _type = type;
        }

        /// <summary>
        /// Get event type
        /// </summary>
        /// <returns>int</returns>
        virtual public int Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Day event indicator
        /// </summary>
        /// <returns>bool</returns>
        virtual public bool DayEvent
        {
            get { return ((_type == YEAR) || (_type == DAY) || (_type == MONTH)); }
        }

        /// <summary>
        /// Month event indicator
        /// </summary>
        /// <returns>bool</returns>
        virtual public bool MonthEvent
        {
            get { return ((_type == YEAR) || (_type == MONTH)); }
        }

        /// <summary>
        /// Year event indicator
        /// </summary>
        /// <returns>bool</returns>
        virtual public bool YearEvent
        {
            get { return (_type == YEAR); }
        }
    }
}