#region Copyright
/*
 * Software: TickZoom Trading Platform
 * Copyright 2009 M. Wayne Walter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, see <http://www.tickzoom.org/wiki/Licenses>
 * or write to Free Software Foundation, Inc., 51 Franklin Street,
 * Fifth Floor, Boston, MA  02110-1301, USA.
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;

namespace MarketSage.Library
{
	public enum TimeUnit
	{
	   None,
	   Volume,
	   /// <summary>
	   /// Constant range bars. These reset every day by default.
	   /// You can override the reset interval for a bar period 
	   /// by setting a secondary time frame.
	   /// </summary>
	   Range,
	   PointFigure,
	   Change,
	   Tick,
	   Second,
	   Minute,
	   Hour,
	   Day,
	   Session,
	   Week,
	   Month,
	   Year,
	}
}
