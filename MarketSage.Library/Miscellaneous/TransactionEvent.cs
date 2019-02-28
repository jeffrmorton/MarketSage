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
    /// Trader event object
    /// </summary>
    public class TransactionEvent
    {
        public const int ORDER_EXEC = -1;
        public const int ORDER_REJECT = -2;
        private int _type;

        /// <summary>
        /// TraderEvent constructor
        /// </summary>
        /// <param name="type">int</param>
        public TransactionEvent(int type)
        {
            _type = type;
        }

        /// <summary>
        /// Get type
        /// </summary>
        /// <returns>int</returns>
        virtual public int Type
        {
            get { return _type; }
        }
    }
}