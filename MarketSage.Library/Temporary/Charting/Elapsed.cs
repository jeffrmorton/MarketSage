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

namespace MarketSage.Library
{
	/// <summary>
	/// Description of Elapsed.
	/// </summary>
	public struct Elapsed : IEquatable<Elapsed>
	{
		double _elapsed; // this is just an example member, replace it with your own struct members!
		
		public double elapsed {
			get { return _elapsed; }
		}
		
		public const double MaxValue = double.MaxValue;
		public const double MinValue = double.MinValue;
		public const double XLDay1 = 2415018.5;
		public const double JulDayMin = 0.0;
		public const double JulDayMax = 5373483.5;
		public const double XLDayMin = JulDayMin - XLDay1;
		public const double XLDayMax = JulDayMax - XLDay1;
		public const double MonthsPerYear = 12.0;
		public const double HoursPerDay = 24.0;
		public const double MinutesPerHour = 60.0;
		public const double SecondsPerMinute = 60.0;
		public const double MinutesPerDay = 1440.0;
		public const double SecondsPerDay = 86400.0;
		public const double MillisecondsPerSecond = 1000.0;
		public const double MillisecondsPerDay = 86400000.0;
		public const string DefaultFormatStr = "g";
		
		public Elapsed( double elapsed) {
			this._elapsed = elapsed;
		}
		
		public Elapsed( int hour, int minute, int second) {
			this._elapsed = hour / HoursPerDay + minute / MinutesPerDay + second / SecondsPerDay;
		}
		
		public int TotalSeconds {
			get { return (int) (elapsed * SecondsPerDay); }
		}
		
		public int TotalMilliseconds {
			get { return (int) (elapsed * MillisecondsPerDay); }
		}
		
		public int TotalHours {
			get { return (int) (elapsed * HoursPerDay); }
		}
		
		public int TotalDays {
			get { return (int) (elapsed); }
		}
		
		public int TotalMinutes {
			get { return (int) (elapsed * MinutesPerDay); }
		}
		
		public static implicit operator Elapsed( double elapsed )
		{
			Elapsed retVal = default(Elapsed);
			retVal.Internal = elapsed;
			return retVal;
		}
		
		public static implicit operator double( Elapsed elapsed )
		{
			return elapsed.elapsed;
		}
		
		#region Equals and GetHashCode implementation
		// The code in this region is useful if you want to use this structure in collections.
		// If you don't need it, you can just remove the region and the ": IEquatable<Elapsed>" declaration.
		
		public override bool Equals(object obj)
		{
			if (obj is Elapsed)
				return Equals((Elapsed)obj); // use Equals method below
			else
				return false;
		}
		
		public bool Equals(Elapsed other)
		{
			// add comparisions for all members here
			return this.elapsed == other.elapsed;
		}
		
		public override int GetHashCode()
		{
			// combine the hash codes of all members here (e.g. with XOR operator ^)
			return elapsed.GetHashCode();
		}
		
		public static bool operator ==(Elapsed lhs, Elapsed rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(Elapsed lhs, Elapsed rhs)
		{
			return !(lhs.Equals(rhs)); // use operator == and negate result
		}
		
		public double Internal {
			get { return _elapsed; }
			set { _elapsed = value; }
		}
		#endregion
	}
}
