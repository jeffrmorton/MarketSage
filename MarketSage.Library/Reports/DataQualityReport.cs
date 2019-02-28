using System;
using System.Collections;
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
    public class DataQualityReport
    {
        private Instrument _instrument;
        private ArrayList _holidaySchedule;
        private ArrayList _data;
        private ArrayList _dataRange;
        private ArrayList _dataDeviation;
        private ArrayList _exceptionRange;
        private ArrayList _exceptionDeviance;
        private ArrayList _exceptionLogic;
        private ArrayList _exceptionMissingDate;
        private ArrayList _exceptionWeekendDate;
        private double _rangeHigh;
        private double _rangeLow;
        private double _deviationHigh;
        private double _deviationLow;
        private string _report;
        public DateTime _dateBegin;
        private DateTime _dateLast;
        public DateTime _dateEnd;
        private int _totalBars;
        private Histo _rangeHistogram;
        private Histo _deviationHistogram;

        public DataQualityReport(Instrument instrument)
        {
            _instrument = instrument;
            _data = _instrument.TechnicalData;

            _dateBegin = ((TechnicalData)_data[0]).Date;
            _dateEnd = ((TechnicalData)_data[_data.Count - 1]).Date;
            _totalBars = _data.Count;

            _exceptionRange = new ArrayList();
            _exceptionDeviance = new ArrayList();

            _dataDeviation = new ArrayList();
            _dataRange = new ArrayList();
            _exceptionMissingDate = new ArrayList();
            _exceptionWeekendDate = new ArrayList();
            _exceptionLogic = new ArrayList();

            _rangeHigh = -99999;
            _rangeLow = 99999;
            _deviationHigh = -99999;
            _deviationLow = 99999;

            for (int i = 0; i < _data.Count; i++)
            {
                CheckLogic(i);
                CheckRange(i);
                CheckDeviation(i);
                CheckDate(i);
            }
            _rangeHistogram = new Histo(10, _dataRange);
            _deviationHistogram = new Histo(10, _dataDeviation);
        }

        public DataQualityReport(Instrument instrument, ArrayList holidaySchedule)
        {
            _instrument = instrument;
            _data = _instrument.TechnicalData;

            _holidaySchedule = holidaySchedule;

            _dateBegin = ((TechnicalData)_data[0]).Date;
            _dateEnd = ((TechnicalData)_data[_data.Count - 1]).Date;
            _totalBars = _data.Count;

            _exceptionRange = new ArrayList();
            _exceptionDeviance = new ArrayList();

            _dataDeviation = new ArrayList();
            _dataRange = new ArrayList();
            _exceptionMissingDate = new ArrayList();
            _exceptionWeekendDate = new ArrayList();
            _exceptionLogic = new ArrayList();

            _rangeHigh = -99999;
            _rangeLow = 99999;
            _deviationHigh = -99999;
            _deviationLow = 99999;

            for (int i = 0; i < _data.Count; i++)
            {
                CheckLogic(i);
                CheckRange(i);
                CheckDeviation(i);
                CheckDate(i);
            }
            _rangeHistogram = new Histo(10, _dataRange);
            _deviationHistogram = new Histo(10, _dataDeviation);
        }

        public string GetReport()
        {
            _report += "Instrument:\t\t\t\t\t" + _instrument.Name + "\r\n";
            _report += "Date range:\t\t\t\t\t" + _dateBegin.ToShortDateString() + " - " + _dateEnd.ToShortDateString() + "\r\n";
            _report += "Total number of bars:\t\t\t\t" + _totalBars.ToString() + "\r\n";
            _report += "Bars with illogical price or volume:\t\t\t" + _exceptionLogic.Count.ToString() + "\r\n";
            if (_exceptionLogic.Count != 0)
                for (int i = 0; i < _exceptionLogic.Count; i++)
                {
                    _report += ((TechnicalData)_exceptionLogic[i]).Date + "\r\n";
                }
            int count = _rangeHistogram.Counts[7] + _rangeHistogram.Counts[8] + _rangeHistogram.Counts[9];
            if (_dataRange.Count > 0)
            {
                _report += "Bars with excessive High-Low range:\t\t\t" + count.ToString() + "\r\n";
                _report += "Distribution of High-Low range:";
                _report += "\t\t\t0:  " + _rangeHistogram.Counts[0].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t1:  " + _rangeHistogram.Counts[1].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t2:  " + _rangeHistogram.Counts[2].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t3:  " + _rangeHistogram.Counts[3].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t4:  " + _rangeHistogram.Counts[4].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t5:  " + _rangeHistogram.Counts[5].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t6:  " + _rangeHistogram.Counts[6].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t7:  " + _rangeHistogram.Counts[7].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t8:  " + _rangeHistogram.Counts[8].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t9:  " + _rangeHistogram.Counts[9].ToString() + "\r\n";
            }
            count = _deviationHistogram.Counts[7] + _deviationHistogram.Counts[8] + _deviationHistogram.Counts[9];
            if (_dataDeviation.Count > 0)
            {
                _report += "Bars with excessive Close deviation:\t\t\t" + count.ToString() + "\r\n";
                _report += "Distribution of Close deviation:";
                _report += "\t\t\t\t0:  " + _deviationHistogram.Counts[0].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t1:  " + _deviationHistogram.Counts[1].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t2:  " + _deviationHistogram.Counts[2].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t3:  " + _deviationHistogram.Counts[3].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t4:  " + _deviationHistogram.Counts[4].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t5:  " + _deviationHistogram.Counts[5].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t6:  " + _deviationHistogram.Counts[6].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t7:  " + _deviationHistogram.Counts[7].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t8:  " + _deviationHistogram.Counts[8].ToString() + "\r\n";
                _report += "\t\t\t\t\t\t9:  " + _deviationHistogram.Counts[9].ToString() + "\r\n";
            }
            _report += "Bars falling on the weekend:\t\t\t\t" + _exceptionWeekendDate.Count.ToString() + "\r\n";
            if (_exceptionWeekendDate.Count > 0)
                for (int i = 0; i < _exceptionWeekendDate.Count; i++)
                {
                    _report += ((DateTime)_exceptionWeekendDate[i]).ToShortDateString() + "\r\n";
                }
            _report += "Bars missing from data set:\t\t\t\t" + _exceptionMissingDate.Count.ToString() + "\r\n";
            if (_exceptionMissingDate.Count > 0)
            {
                _report += "Dates of missing bars:\t\t\t\t" + ((DateTime)_exceptionMissingDate[0]).ToShortDateString() + "\r\n";
                for (int i = 1; i < _exceptionMissingDate.Count; i++)
                {
                    _report += "\t\t\t\t\t\t" + ((DateTime)_exceptionMissingDate[i]).ToShortDateString() + "\r\n";
                }
            }
            return _report;
        }

        private void CheckLogic(int index)
        {
            bool _isLogical = true;
            // Volume out of bounds
            if (((TechnicalData)_data[index]).AdjVolume < 0)
                _isLogical = false;
            // High in relation to Open/Close
            if (((TechnicalData)_data[index]).AdjHigh < ((TechnicalData)_data[index]).AdjOpen)
                _isLogical = false;
            if (((TechnicalData)_data[index]).AdjHigh < ((TechnicalData)_data[index]).AdjClose)
                _isLogical = false;
            // High and Low relationship
            if (((TechnicalData)_data[index]).AdjHigh < ((TechnicalData)_data[index]).AdjLow)
                _isLogical = false;
            // Low in relation to Open/Close
            if (((TechnicalData)_data[index]).AdjLow > ((TechnicalData)_data[index]).AdjOpen)
                _isLogical = false;
            if (((TechnicalData)_data[index]).AdjLow > ((TechnicalData)_data[index]).AdjClose)
                _isLogical = false;
            if (_isLogical == false)
                _exceptionLogic.Add(_data[index]);
        }

        private void CheckRange(int index)
        {
            // Range = Abs(High today  - Low today)
            // Standardized Range = Range / 20 day Simple Moving Average of Ranges
            double rangeBar = Math.Abs(((TechnicalData)_data[index]).AdjHigh - ((TechnicalData)_data[index]).AdjLow);
            double rangeSMA = 0;
            double rangeStd = 0;
            // Calculate 20 day Simple Moving Average
            if (index >= 20)
            {
                for (int i = index - 20; i <= index; i++)
                {
                    rangeSMA += Math.Abs(((TechnicalData)_data[i]).AdjHigh - ((TechnicalData)_data[i]).AdjLow);
                }
                rangeSMA = rangeSMA / 20;
            }
            else if (index > 0)
            {
                for (int i = 0; i <= index; i++)
                {
                    rangeSMA += Math.Abs(((TechnicalData)_data[i]).AdjHigh - ((TechnicalData)_data[i]).AdjLow);
                }
                rangeSMA = rangeSMA / (index + 1);
            }
            else
                rangeSMA = rangeBar;
            rangeStd = rangeBar / rangeSMA;
            if (_rangeHigh < rangeStd)
                _rangeHigh = rangeStd;
            if (_rangeLow > rangeStd)
                _rangeLow = rangeStd;
            _dataRange.Add(rangeStd);
        }

        private void CheckDeviation(int index)
        {
            // Deviation = Abs(Close today - Close yesterday)
            // Standardized Deviation = Deviation / 20 day Simple Moving Average of Deviations
            double devianceBar = 0;
            double devianceSMA = 0;
            double devianceStd = 0;
            if (index > 0)
                devianceBar = Math.Abs(((TechnicalData)_data[index]).AdjClose - ((TechnicalData)_data[index - 1]).AdjClose);
            // Calculate 20 day Simple Moving Average
            if (index >= 20)
            {
                for (int i = index - 19; i <= index; i++)
                {
                    devianceSMA += Math.Abs(((TechnicalData)_data[i]).AdjClose - ((TechnicalData)_data[i - 1]).AdjClose);
                }
                if (devianceSMA != 0)
                    devianceSMA = devianceSMA / 20;
            }
            else if (index > 0)
            {
                for (int i = 1; i <= index; i++)
                {
                    devianceSMA += Math.Abs(((TechnicalData)_data[i]).AdjClose - ((TechnicalData)_data[i - 1]).AdjClose);
                }
                if (devianceSMA != 0)
                    devianceSMA = devianceSMA / (index + 1);
            }
            else
            {
                devianceBar = 0;
                devianceSMA = 0;
            }
            if (devianceSMA != 0)
                devianceStd = devianceBar / devianceSMA;
            else
                devianceStd = 0;
            if (_deviationHigh < devianceStd)
                _deviationHigh = devianceStd;
            if (_deviationLow > devianceStd)
                _deviationLow = devianceStd;
            _dataDeviation.Add(devianceStd);
        }

        public void CheckDate(int index)
        {
            DateTime dateNow = ((TechnicalData)_data[index]).Date;
            if ((int)dateNow.DayOfWeek < 1 || (int)dateNow.DayOfWeek > 5)
                _exceptionWeekendDate.Add(dateNow);
            else
            {
                if (index > 0)
                {
                    TimeSpan diff = dateNow.Subtract(_dateLast);
                    int days = diff.Days;
                    if (days > 1)
                    {
                        for (int x = days - 1; x > 0; x--)
                        {
                            DateTime date = dateNow.Subtract(new TimeSpan(x, 0, 0, 0));
                            if ((int)date.DayOfWeek > 0 && (int)date.DayOfWeek < 6)
                                if (!_holidaySchedule.Contains(date))
                                    _exceptionMissingDate.Add(date);
                        }
                    }
                }
                _dateLast = dateNow;
            }
        }

        public static double GetAverage(double[] data)
        {
            int len = data.Length;
            if (len == 0)
                throw new Exception("No data");
            double sum = 0;
            for (int i = 0; i < data.Length; i++)
                sum += data[i];
            return sum / len;
        }

        public static double GetVariance(double[] data)
        {
            int len = data.Length;
            double avg = GetAverage(data);
            double sum = 0;
            for (int i = 0; i < data.Length; i++)
                sum += Math.Pow((data[i] - avg), 2);
            return sum / len;
        }

        public static double GetStdev(double[] data)
        {
            return Math.Sqrt(GetVariance(data));
        }

        public ArrayList MissingDates
        {
            get { return _exceptionMissingDate; }
        }

        public ArrayList WeekendDates
        {
            get { return _exceptionWeekendDate; }
        }

        public ArrayList HolidaySchedule
        {
            get { return _holidaySchedule; }
            set { _holidaySchedule = value; }
        }
    }
}
