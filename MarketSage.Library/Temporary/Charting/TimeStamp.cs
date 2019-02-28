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

namespace MarketSage.Library
{

	
	public struct TimeStamp : IComparable<TimeStamp>
	{
		private double _timeStamp;
		
		public static TimeStamp MaxValue { 
			get { return new TimeStamp( double.MaxValue); }
		}
		public static TimeStamp MinValue { 
			get { return new TimeStamp( double.MinValue); }
		}
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
		public const string DefaultFormatStr = "yyyy-MM-dd HH:mm:ss.fff";
		
		public void Assign( int year, int month, int day, int hour, int minute, int second, int millis) {
			_timeStamp = CalendarDateTotimeStamp( year, month, day, hour, minute, second, millis );
		}
		
//		#if FIXED
		public double TimeOfDay {
			get { int other;
				  int hour;
				  int minute;
				  int second;
				  int millis;
				  GetDate(out other,out other,out other,out hour,out minute,out second,out millis);
				  return millis/MillisecondsPerDay+
				  		second/SecondsPerDay+
				  		minute/MinutesPerDay+
				  		hour/HoursPerDay;
			}
		}
		
		public int Year {
			get { int other;
				  int year;
				  GetDate(out year,out other,out other,out other,out other,out other,out other);
				  return year;
			}
		}
		
		public WeekDay WeekDay {
			get { return (WeekDay) timeStampToWeekDay(_timeStamp); }
		}
		public int Month {
			get { int other;
				  int month;
				  GetDate(out other,out month,out other,out other,out other,out other,out other);
				  return month;
			}
		}
		public int Day {
			get { int other;
				  int day;
				  GetDate(out other,out other,out day,out other,out other,out other,out other);
				  return day;
			}
		}
		public int Hour {
			get { int other;
				  int hour;
				  GetDate(out other,out other,out other,out hour,out other,out other,out other);
				  return hour;
			}
		}
		
		public int Minute {
			get { int other;
				  int minute;
				  GetDate(out other,out other,out other,out other,out minute,out other,out other);
				  return minute;
			}
		}
		
		public int Second {
			get { int other;
				  int second;
				  GetDate(out other,out other,out other,out other,out other,out second,out other);
				  return second;
			}
		}
		
//		#endif
		
		public int TotalSeconds {
			get { return (int) (_timeStamp / SecondsPerDay); }
		}
	
		public TimeStamp( double timeStamp )
		{
			_timeStamp = timeStamp;
		}
		public TimeStamp( DateTime dateTime )
		{
			_timeStamp = CalendarDateTotimeStamp( dateTime.Year, dateTime.Month,
							dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second,
							dateTime.Millisecond );
		}
		public TimeStamp( int year, int month, int day )
		{
			_timeStamp = CalendarDateTotimeStamp( year, month, day, 0, 0, 0 );
		}
		public TimeStamp( int year, int month, int day, int hour, int minute, int second )
		{
			_timeStamp = CalendarDateTotimeStamp( year, month, day, hour, minute, second );
		}
		public TimeStamp( int year, int month, int day, int hour, int minute, double second )
		{
			_timeStamp = CalendarDateTotimeStamp( year, month, day, hour, minute, second );
		}
		public TimeStamp( int year, int month, int day, int hour, int minute, int second, int millisecond )
		{
			_timeStamp = CalendarDateTotimeStamp( year, month, day, hour, minute, second, millisecond );
		}
		public TimeStamp( TimeStamp rhs )
		{
			_timeStamp = rhs._timeStamp;
		}
	
		public double Internal
		{
			get { return _timeStamp; }
			set { _timeStamp = value; }
		}

		public bool IsValidDate
		{
			get { return _timeStamp >= XLDayMin && _timeStamp <= XLDayMax; }
		}
		
		public DateTime DateTime
		{
			get { return timeStampToDateTime( _timeStamp ); }
			set { _timeStamp = DateTimeTotimeStamp( value ); }
		}
		
		public double JulianDay
		{
			get { return timeStampToJulianDay( _timeStamp ); }
			set { _timeStamp = JulianDayTotimeStamp( value ); }
		}
		
		public double DecimalYear
		{
			get { return timeStampToDecimalYear( _timeStamp ); }
			set { _timeStamp = DecimalYearTotimeStamp( value ); }
		}
	
		private static bool CheckValidDate( double timeStamp )
		{
			return timeStamp >= XLDayMin && timeStamp <= XLDayMax;
		}

		public static double MakeValidDate( double timeStamp )
		{
			if ( timeStamp < XLDayMin )
				timeStamp = XLDayMin;
			if ( timeStamp > XLDayMax )
				timeStamp = XLDayMax;
			return timeStamp;
		}

		public void GetDate( out int year, out int month, out int day )
		{
			int hour, minute, second;
			
			timeStampToCalendarDate( _timeStamp, out year, out month, out day, out hour, out minute, out second );
		}
		
		public void GetDate( out int year, out int month, out int day,
						out int hour, out int minute, out int second, out int millis)
		{
			timeStampToCalendarDate( _timeStamp, out year, out month, out day, out hour, out minute, out second, out millis );
		}
		
		public void SetDate( int year, int month, int day )
		{
			_timeStamp = CalendarDateTotimeStamp( year, month, day, 0, 0, 0 );
		}
		
		public void GetDate( out int year, out int month, out int day,
						out int hour, out int minute, out int second )
		{
			timeStampToCalendarDate( _timeStamp, out year, out month, out day, out hour, out minute, out second );
		}

		public void SetDate( int year, int month, int day, int hour, int minute, int second )
		{
			_timeStamp = CalendarDateTotimeStamp( year, month, day, hour, minute, second );
		}
		
		public double GetDayOfYear()
		{
			return timeStampToDayOfYear( _timeStamp );
		}

		public static double CalendarDateTotimeStamp( int year, int month, int day,
			int hour, int minute, int second, int millisecond )
		{
			// Normalize the data to allow for negative and out of range values
			// In this way, setting month to zero would be December of the previous year,
			// setting hour to 24 would be the first hour of the next day, etc.
			//double dsec = second + (double) millisecond / MillisecondsPerSecond;
			double ms = millisecond;
			NormalizeCalendarDate( ref year, ref month, ref day, ref hour, ref minute, ref second,
						ref ms );
		
			return _CalendarDateTotimeStamp( year, month, day, hour, minute, second, ms );
		}
		
		public static double CalendarDateTotimeStamp( int year, int month, int day,
			int hour, int minute, int second )
		{
			// Normalize the data to allow for negative and out of range values
			// In this way, setting month to zero would be December of the previous year,
			// setting hour to 24 would be the first hour of the next day, etc.
			double ms = 0;
			NormalizeCalendarDate( ref year, ref month, ref day, ref hour, ref minute,
					ref second, ref ms );
		
			return _CalendarDateTotimeStamp( year, month, day, hour, minute, second, ms );
		}
		
		public static double CalendarDateTotimeStamp( int year, int month, int day,
			int hour, int minute, double second )
		{
			// Normalize the data to allow for negative and out of range values
			// In this way, setting month to zero would be December of the previous year,
			// setting hour to 24 would be the first hour of the next day, etc.
			int sec = (int)second;
			double ms = ( second - sec ) * MillisecondsPerSecond;
			NormalizeCalendarDate( ref year, ref month, ref day, ref hour, ref minute, ref sec,
					ref ms );
		
			return _CalendarDateTotimeStamp( year, month, day, hour, minute, sec, ms );
		}
		
		public static double CalendarDateToJulianDay( int year, int month, int day,
			int hour, int minute, int second )
		{
			// Normalize the data to allow for negative and out of range values
			// In this way, setting month to zero would be December of the previous year,
			// setting hour to 24 would be the first hour of the next day, etc.
			double ms = 0;
			NormalizeCalendarDate( ref year, ref month, ref day, ref hour, ref minute,
				ref second, ref ms );
		
			return _CalendarDateToJulianDay( year, month, day, hour, minute, second, ms );
		}
		
		public static double CalendarDateToJulianDay( int year, int month, int day,
			int hour, int minute, int second, int millisecond )
		{
			// Normalize the data to allow for negative and out of range values
			// In this way, setting month to zero would be December of the previous year,
			// setting hour to 24 would be the first hour of the next day, etc.
			double ms = millisecond;

			NormalizeCalendarDate( ref year, ref month, ref day, ref hour, ref minute,
						ref second, ref ms );
		
			return _CalendarDateToJulianDay( year, month, day, hour, minute, second, ms );
		}
		
		private static void NormalizeCalendarDate( ref int year, ref int month, ref int day,
											ref int hour, ref int minute, ref int second,
											ref double millisecond )
		{
			// Normalize the data to allow for negative and out of range values
			// In this way, setting month to zero would be December of the previous year,
			// setting hour to 24 would be the first hour of the next day, etc.

			// Normalize the milliseconds and carry over to seconds
			int carry = (int)Math.Floor( millisecond / MillisecondsPerSecond );
			millisecond -= carry * (int)MillisecondsPerSecond;
			second += carry;

			// Normalize the seconds and carry over to minutes
			carry = (int)Math.Floor( second / SecondsPerMinute );
			second -= carry * (int)SecondsPerMinute;
			minute += carry;
		
			// Normalize the minutes and carry over to hours
			carry = (int) Math.Floor( (double) minute / MinutesPerHour );
			minute -= carry * (int) MinutesPerHour;
			hour += carry;
		
			// Normalize the hours and carry over to days
			carry = (int) Math.Floor( (double) hour / HoursPerDay );
			hour -= carry * (int) HoursPerDay;
			day += carry;
		
			// Normalize the months and carry over to years
			carry = (int) Math.Floor( (double) month / MonthsPerYear );
			month -= carry * (int) MonthsPerYear;
			year += carry;
		}
		
		private static double _CalendarDateTotimeStamp( int year, int month, int day, int hour,
					int minute, int second, double millisecond )
		{
			return JulianDayTotimeStamp( _CalendarDateToJulianDay( year, month, day, hour, minute,
						second, millisecond ) );
		}
		
		private static double _CalendarDateToJulianDay( int year, int month, int day, int hour,
					int minute, int second, double millisecond )
		{
			// Taken from http://www.srrb.noaa.gov/highlights/sunrise/program.txt
			// routine calcJD()
		
			if ( month <= 2 )
			{
				year -= 1;
				month += 12;
			}
		
			double A = Math.Floor( (double) year / 100.0 );
			double B = 2 - A + Math.Floor( A / 4.0 );
		
			return	Math.Floor( 365.25 * ( (double) year + 4716.0 ) ) +
					Math.Floor( 30.6001 * (double) ( month + 1 ) ) +
					(double) day + B - 1524.5 +
					hour  / HoursPerDay + minute / MinutesPerDay + second / SecondsPerDay +
					millisecond / MillisecondsPerDay;
		
		}

		public static void timeStampToCalendarDate( double timeStamp, out int year, out int month,
			out int day, out int hour, out int minute, out int second )
		{
			double jDay = timeStampToJulianDay( timeStamp );
			
			JulianDayToCalendarDate( jDay, out year, out month, out day, out hour,
				out minute, out second );
		}
		
		public static void timeStampToCalendarDate( double timeStamp, out int year, out int month,
			out int day, out int hour, out int minute, out int second, out int millisecond )
		{
			double jDay = timeStampToJulianDay( timeStamp );
			
			double ms;
			JulianDayToCalendarDate( jDay, out year, out month, out day, out hour,
				out minute, out second, out ms );
			millisecond = (int)ms;
		}
		
		public static void timeStampToCalendarDate( double timeStamp, out int year, out int month,
			out int day, out int hour, out int minute, out double second )
		{
			double jDay = timeStampToJulianDay( timeStamp );
			
			JulianDayToCalendarDate( jDay, out year, out month, out day, out hour,
				out minute, out second );
		}
		
		public static void JulianDayToCalendarDate( double jDay, out int year, out int month,
			out int day, out int hour, out int minute, out int second )
		{
			double ms = 0;

			JulianDayToCalendarDate( jDay, out year, out month,
					out day, out hour, out minute, out second, out ms );
		}

		public static void JulianDayToCalendarDate( double jDay, out int year, out int month,
			out int day, out int hour, out int minute, out double second )
		{
			int sec;
			double ms;

			JulianDayToCalendarDate( jDay, out year, out month,
					out day, out hour, out minute, out sec, out ms );

			second = sec + ms / MillisecondsPerSecond;
		}

		public static void JulianDayToCalendarDate( double jDay, out int year, out int month,
			out int day, out int hour, out int minute, out int second, out double millisecond )
		{
			// add 5 ten-thousandths of a second to the day fraction to avoid roundoff errors
			jDay += 0.0005 / SecondsPerDay;

			double z = Math.Floor( jDay + 0.5);
			double f = jDay + 0.5 - z;
		
			double alpha = Math.Floor( ( z - 1867216.25 ) / 36524.25 );
			double A = z + 1.0 + alpha - Math.Floor( alpha / 4 );
			double B = A + 1524.0;
			double C = Math.Floor( ( B - 122.1 ) / 365.25 );
			double D = Math.Floor( 365.25 * C );
			double E = Math.Floor( ( B - D ) / 30.6001 );
		
			day = (int) Math.Floor( B - D - Math.Floor( 30.6001 * E ) + f );
			month = (int) ( ( E < 14.0 ) ? E - 1.0 : E - 13.0 );
			year = (int) ( ( month > 2 ) ? C - 4716 : C - 4715 );
		
			double fday =  ( jDay - 0.5 ) - Math.Floor( jDay - 0.5 );
		
			fday = ( fday - (long) fday ) * HoursPerDay;
			hour = (int) fday;
			fday = ( fday - (long) fday ) * MinutesPerHour;
			minute = (int) fday;
			fday = ( fday - (long) fday ) * SecondsPerMinute;
			second = (int) fday;
			fday = ( fday - (long) fday ) * MillisecondsPerSecond;
			millisecond = fday;
		}
		
		public static double timeStampToJulianDay( double timeStamp )
		{
			return timeStamp + XLDay1;
		}
		
		public static double JulianDayTotimeStamp( double jDay )
		{
			return jDay - XLDay1;
		}
		
		public static double timeStampToDecimalYear( double timeStamp )
		{
			int year, month, day, hour, minute, second;
			
			timeStampToCalendarDate( timeStamp, out year, out month, out day, out hour, out minute, out second );
			
			double jDay1 = CalendarDateToJulianDay( year, 1, 1, 0, 0, 0 );
			double jDay2 = CalendarDateToJulianDay( year + 1, 1, 1, 0, 0, 0 );
			double jDayMid = CalendarDateToJulianDay( year, month, day, hour, minute, second );
			
			
			return (double) year + ( jDayMid - jDay1 ) / ( jDay2 - jDay1 );
		}
		
		public static double DecimalYearTotimeStamp( double yearDec )
		{
			int year = (int) yearDec;
			
			double jDay1 = CalendarDateToJulianDay( year, 1, 1, 0, 0, 0 );
			double jDay2 = CalendarDateToJulianDay( year + 1, 1, 1, 0, 0, 0 );
			
			return JulianDayTotimeStamp( ( yearDec - (double) year ) * ( jDay2 - jDay1 ) + jDay1 );
		}
		
		public static double timeStampToDayOfYear( double timeStamp )
		{
			int year, month, day, hour, minute, second;
			timeStampToCalendarDate( timeStamp, out year, out month, out day,
									out hour, out minute, out second );
			return timeStampToJulianDay( timeStamp ) - CalendarDateToJulianDay( year, 1, 1, 0, 0, 0 ) + 1.0;
		}
		
		public static int timeStampToWeekDay( double timeStamp )
		{
			return (int) ( timeStampToJulianDay( timeStamp ) + 1.5 ) % 7;
		}
		
		public static DateTime timeStampToDateTime( double timeStamp )
		{
			int year, month, day, hour, minute, second, millisecond;
			timeStampToCalendarDate( timeStamp, out year, out month, out day,
									out hour, out minute, out second, out millisecond );
			return new DateTime( year, month, day, hour, minute, second, millisecond );
		}
		
		public static double DateTimeTotimeStamp( DateTime dt )
		{
			return CalendarDateTotimeStamp( dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second,
										dt.Millisecond );
		}

		public void AddMilliseconds( double dMilliseconds )
		{
			_timeStamp += dMilliseconds / MillisecondsPerDay;
		}

		public void AddSeconds( double dSeconds )
		{
			_timeStamp += dSeconds / SecondsPerDay;
		}

		public void AddMinutes( double dMinutes )
		{
			_timeStamp += dMinutes / MinutesPerDay;
		}
		
		public void AddHours( double dHours )
		{
			_timeStamp += dHours / HoursPerDay;
		}
		
		public void AddDays( double dDays )
		{
			_timeStamp += dDays;
		}
		
		public void AddMonths( double dMonths )
		{
			int iMon = (int) dMonths;
			double monFrac = Math.Abs( dMonths - (double) iMon );
			int sMon = Math.Sign( dMonths );
			
			int year, month, day, hour, minute, second;
			
			timeStampToCalendarDate( _timeStamp, out year, out month, out day, out hour, out minute, out second );
			if ( iMon != 0 )
			{
				month += iMon;
				_timeStamp = CalendarDateTotimeStamp( year, month, day, hour, minute, second );
			}
			
			if ( sMon != 0 )
			{
				double timeStamp2 = CalendarDateTotimeStamp( year, month+sMon, day, hour, minute, second );
				_timeStamp += (timeStamp2 - _timeStamp) * monFrac;
			}
		}
		
		public void AddYears( double dYears )
		{
			int iYear = (int) dYears;
			double yearFrac = Math.Abs( dYears - (double) iYear );
			int sYear = Math.Sign( dYears );
			
			int year, month, day, hour, minute, second;
			
			timeStampToCalendarDate( _timeStamp, out year, out month, out day, out hour, out minute, out second );
			if ( iYear != 0 )
			{
				year += iYear;
				_timeStamp = CalendarDateTotimeStamp( year, month, day, hour, minute, second );
			}
			
			if ( sYear != 0 )
			{
				double timeStamp2 = CalendarDateTotimeStamp( year+sYear, month, day, hour, minute, second );
				_timeStamp += (timeStamp2 - _timeStamp) * yearFrac;
			}
		}

		public static double operator -( TimeStamp lhs, TimeStamp rhs )
		{
			return lhs._timeStamp - rhs._timeStamp;
		}
		
		public static TimeStamp operator -( TimeStamp lhs, double rhs )
		{
			lhs._timeStamp -= rhs;
			return lhs;
		}
		
		public static TimeStamp operator +( TimeStamp lhs, double rhs )
		{
			lhs._timeStamp += rhs;
			return lhs;
		}
		
		public static TimeStamp operator ++( TimeStamp TimeStamp )
		{
			TimeStamp._timeStamp += 1.0;
			return TimeStamp;
		}
		
		public static TimeStamp operator --( TimeStamp TimeStamp )
		{
			TimeStamp._timeStamp -= 1.0;
			return TimeStamp;
		}
		
		public static explicit operator double( TimeStamp TimeStamp )
		{
			return TimeStamp._timeStamp;
		}
		
		public static explicit operator DateTime( TimeStamp TimeStamp )
		{
			
			return timeStampToDateTime( TimeStamp.Internal );
		}
		
		public static explicit operator TimeStamp( DateTime dt )
		{
			
			return new TimeStamp( DateTimeTotimeStamp( dt ) );
		}
		
		public static bool operator >=( TimeStamp lhs, TimeStamp rhs)
		{
			return lhs.CompareTo(rhs) >= 0;
		}
		
		public static bool operator ==( TimeStamp lhs, TimeStamp rhs)
		{
			return lhs.CompareTo(rhs) == 0;
		}
		
		public static bool operator !=( TimeStamp lhs, TimeStamp rhs)
		{
			return lhs.CompareTo(rhs) != 0;
		}
		
		public static bool operator <=( TimeStamp lhs, TimeStamp rhs)
		{
			return lhs.CompareTo(rhs) <= 0;
		}
		
		public static bool operator >( TimeStamp lhs, TimeStamp rhs)
		{
			return lhs.CompareTo(rhs) > 0;
		}
		
		public static bool operator <( TimeStamp lhs, TimeStamp rhs)
		{
			return lhs.CompareTo(rhs) < 0;
		}
		
		public override bool Equals( object obj )
		{
			if ( obj is TimeStamp )
			{
				return ((TimeStamp) obj)._timeStamp == _timeStamp;
			}
			else if ( obj is double )
			{
				return ((double) obj) == _timeStamp;
			}
			else
				return false;
		}
		
		public override int GetHashCode()
		{
			return _timeStamp.GetHashCode();
		}

		public int CompareTo( TimeStamp target )
		{
			return this._timeStamp > target._timeStamp ? 1 :
				this._timeStamp < target._timeStamp ? -1 : 0;
		}

		public string ToString( double timeStamp )
		{
			return ToString( timeStamp, DefaultFormatStr );
		}
		
		public override string ToString()
		{
			return ToString( _timeStamp, DefaultFormatStr );
		}
		
		public string ToString( string fmtStr )
		{
			return ToString( this._timeStamp, fmtStr );
		}
		
		public void Add( Elapsed elapsed) {
			_timeStamp += elapsed.elapsed;
		}

		public static string ToString( double timeStamp, string fmtStr )
		{
			int		year, month, day, hour, minute, second, millisecond;

			if ( !CheckValidDate( timeStamp ) )
				return "Date Error";

			timeStampToCalendarDate( timeStamp, out year, out month, out day, out hour, out minute,
											out second, out millisecond );
			if ( year <= 0 )
			{
				year = 1 - year;
				fmtStr = fmtStr + " (BC)";
			}

			if ( fmtStr.IndexOf("[d]") >= 0 )
			{
				fmtStr = fmtStr.Replace( "[d]", ((int) timeStamp).ToString() );
				timeStamp -= (int) timeStamp;
			}
			if ( fmtStr.IndexOf("[h]") >= 0 || fmtStr.IndexOf("[hh]") >= 0 )
			{
				fmtStr = fmtStr.Replace( "[h]", ((int) (timeStamp * 24)).ToString("d") );
				fmtStr = fmtStr.Replace( "[hh]", ((int) (timeStamp * 24)).ToString("d2") );
				timeStamp = ( timeStamp * 24 - (int) (timeStamp * 24) ) / 24.0;
			}
			if ( fmtStr.IndexOf("[m]") >= 0 || fmtStr.IndexOf("[mm]") >= 0 )
			{
				fmtStr = fmtStr.Replace( "[m]", ((int) (timeStamp * 1440)).ToString("d") );
				fmtStr = fmtStr.Replace( "[mm]", ((int) (timeStamp * 1440)).ToString("d2") );
				timeStamp = ( timeStamp * 1440 - (int) (timeStamp * 1440) ) / 1440.0;
			}
			if ( fmtStr.IndexOf("[s]") >= 0 || fmtStr.IndexOf("[ss]") >= 0 )
			{
				fmtStr = fmtStr.Replace( "[s]", ((int) (timeStamp * 86400)).ToString("d") );
				fmtStr = fmtStr.Replace( "[ss]", ((int) (timeStamp * 86400)).ToString("d2") );
				timeStamp = ( timeStamp * 86400 - (int) (timeStamp * 86400) ) / 86400.0;
			}
			if ( fmtStr.IndexOf("[f]") >= 0 )
				fmtStr = fmtStr.Replace( "[f]", ((int) (timeStamp * 864000)).ToString("d") );
			if ( fmtStr.IndexOf("[ff]") >= 0 )
				fmtStr = fmtStr.Replace( "[ff]", ((int) (timeStamp * 8640000)).ToString("d") );
			if ( fmtStr.IndexOf("[fff]") >= 0 )
				fmtStr = fmtStr.Replace( "[fff]", ((int) (timeStamp * 86400000)).ToString("d") );
			if ( fmtStr.IndexOf("[ffff]") >= 0 )
				fmtStr = fmtStr.Replace( "[ffff]", ((int) (timeStamp * 864000000)).ToString("d") );
			if ( fmtStr.IndexOf("[fffff]") >= 0 )
				fmtStr = fmtStr.Replace( "[fffff]", ((int) (timeStamp * 8640000000)).ToString("d") );

			//DateTime dt = timeStampToDateTime( timeStamp );
			if ( year > 9999 )
				year = 9999;
			DateTime dt = new DateTime( year, month, day, hour, minute, second, millisecond );
			return dt.ToString( fmtStr );
		}
	}
}
