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
    /// <summary>Interface for indicator objects</summary>
    public interface IIndicator
    {
        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }

        //int Period { get; set;}

        /// <summary>Returns name</summary>
        /// <returns>string</returns>
        string GetName();

        /// <summary>Returns readyness of the indicator</summary>
        /// <returns>bool</returns>
        bool IsReady();

        /// <summary>Returns direction</summary>
        /// <returns>int</returns>
        int GetDirection();

        /// <summary>Indicates buy signal</summary>
        /// <returns>bool</returns>
        bool IsBuy();

        /// <summary>Indicates hold signal</summary>
        /// <returns>bool</returns>
        bool IsHold();

        /// <summary>Indicates sell signal</summary>
        /// <returns>bool</returns>
        bool IsSell();

        /// <summary>Adds TechnicalData object to the indicator</summary>
        /// <param name="data">TechnicalData object to be added</param>
        void AddData(TechnicalData data);

        void Initialize();
    }
}
