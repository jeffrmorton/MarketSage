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
    /// Order object
    /// </summary>
    public class Order
    {
        private bool _type;  // quantity or cash
        private double _value;  // value
        private string _instrument;  // instrument

        /// <summary>
        /// Order constructor
        /// </summary>
        /// <param name="instrument">string</param>
        /// <param name="type">bool</param>
        /// <param name="value">double</param>
        public Order(string instrument, bool type, double value)
        {
            _type = type;
            _value = value;
            _instrument = instrument;
        }

        /// <summary>
        /// Type indicator
        /// </summary>
        /// <returns>bool</returns>
        virtual public bool Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Get value
        /// </summary>
        /// <returns>double</returns>
        virtual public double Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Get instrument
        /// </summary>
        /// <returns>string</returns>
        virtual public string Instrument
        {
            get { return _instrument; }
        }
    }
}