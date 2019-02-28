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
    /// Transaction object
    /// </summary>
    public class Transaction
    {
        private DateTime _date;
        private string _instrument;
        private double _amount;
        private double _quantity;

        /// <summary>
        /// Transaction constructor
        /// </summary>
        /// <param name="date">DateTime</param>
        /// <param name="instrument">string</param>
        /// <param name="quantity">double</param>
        /// <param name="amount">double</param>
        internal Transaction(DateTime date, string instrument, double quantity, double amount)
        {
            _date = date;
            _instrument = instrument;
            _quantity = quantity;
            _amount = amount;
        }

        /// <summary>
        /// Get date
        /// </summary>
        /// <returns>DateTime</returns>
        virtual public DateTime Date
        {
            get { return _date; }
        }

        /// <summary>
        /// Get instrument
        /// </summary>
        /// <returns>string</returns>
        virtual public string Instrument
        {
            get { return _instrument; }
        }

        /// <summary>
        /// Get quantity
        /// </summary>
        /// <returns>double</returns>
        virtual public double Quantity
        {
            get { return _quantity; }
        }

        /// <summary>
        /// Get amount
        /// </summary>
        /// <returns>double</returns>
        virtual public double Amount
        {
            get { return _amount; }
        }
    }
}