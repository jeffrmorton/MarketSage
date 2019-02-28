using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.OleDb;
using System.Globalization;



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
    /// Contains support elements such as classes, interfaces and static methods.
    /// </summary>
    public class SupportClass
    {
        /// <summary>
        /// Convert milliseconds to minutes
        /// </summary>
        /// <param name="milliDiff"></param>
        /// <returns></returns>
        public static int ConvertToMinutes(double milliDiff)
        {
            bool negative = false;
            int minutesDiff = -1;
            if (milliDiff < 0)
            {
                negative = true;
                milliDiff = -milliDiff; // Make positive (is easier)
            }
            if (milliDiff == 0) // Watch out for exceptional value 0
                minutesDiff = 0;
            else
                minutesDiff = (int)(milliDiff / 1000) / 60;
            if (negative) minutesDiff = -minutesDiff; // Make it negative again
            return minutesDiff;
        }

        /// <summary>
        /// Get lunar phase
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public static int GetLunarPhase(DateTime now)
        {
            double Y = now.Year;
            double M = now.Month;
            double D = now.Day;

            int _phase = 0;
            double _age = 0;

            double YY = 0.0;
            double MM = 0.0;
            double K1 = 0.0;
            double K2 = 0.0;
            double K3 = 0.0;
            double JD = 0.0;
            double IP = 0.0;

            // calculate the Julian date at 12h UT
            YY = Y - Math.Floor((12 - M) / 10);
            MM = M + 9;
            if (MM >= 12) MM = MM - 12;

            K1 = Math.Floor(365.25 * (YY + 4712));
            K2 = Math.Floor(30.6 * MM + 0.5);
            K3 = Math.Floor(Math.Floor((YY / 100) + 49) * 0.75) - 38;

            JD = K1 + K2 + D + 59;                 // for dates in Julian calendar
            if (JD > 2299160) JD = JD - K3;        // for Gregorian calendar

            // calculate moon's age in days
            IP = Normalize((JD - 2451550.1) / 29.530588853);
            _age = IP * 29.53;

            if (_age < 1.84566) _phase = 0;  // "New"
            else if (_age < 5.53699) _phase = 1;  // "Evening crescent"
            else if (_age < 9.22831) _phase = 2;  // "First quarter"
            else if (_age < 12.91963) _phase = 3;  // "Waxing gibbous"
            else if (_age < 16.61096) _phase = 4;  // "Full"
            else if (_age < 20.30228) _phase = 5;  // "Waning gibbous"
            else if (_age < 23.99361) _phase = 6;  // "Last quarter"
            else if (_age < 27.68493) _phase = 7;  // "Morning crescent"

            return _phase;
        }

        /// <summary>
        /// Normalize
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double Normalize(double v)
        {
            v = v - Math.Floor(v);
            if (v < 0)
                v = v + 1;
            return v;
        }

        /// <summary>
        /// Get statistics
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double[] GetStatistics(ref double[] data)
        {
            ArrayList numbers = new ArrayList();
            int count = 0;
            double minimum = 0.0;
            double median = 0.0;
            double maximum = 0.0;
            double sum = 0.0;
            double mean = 0.0;
            double average_deviation = 0.0;
            double standard_deviation = 0.0;
            double variance = 0.0;
            double skew = 0.0;
            double kurtosis = 0.0;
            double signal_to_noise_ratio = 0.0; // mean / standard_deviation;
            double coefficient_of_variation = 0.0; // (standard_deviation / mean) * 100;
            count = data.Length;
            for (int x = 0; x < count; x++)
            {
                sum += data[x];
                numbers.Add(data[x]);
            }
            mean = sum / count;
            for (int x = 0; x < count; x++)
            {
                double deviation = 0.0;
                deviation = (double)numbers[x] - mean;
                average_deviation += Math.Abs(deviation);
                variance += Math.Pow(deviation, 2);
                skew += Math.Pow(deviation, 3);
                kurtosis += Math.Pow(deviation, 4);
            }
            average_deviation /= count;
            variance /= (count - 1);
            standard_deviation = Math.Sqrt(variance);
            if (variance != 0.0)
            {
                skew /= (count * variance * standard_deviation);
                kurtosis = kurtosis / (count * variance * variance) - 3.0;
            }
            signal_to_noise_ratio = mean / standard_deviation;
            coefficient_of_variation = (standard_deviation / mean) * 100;
            numbers.Sort();
            minimum = (double)numbers[0];
            median = (count % 2 != 0) ? (double)numbers[count / 2] : ((double)numbers[count / 2] + (double)numbers[(count / 2) - 1]) / 2;
            maximum = (double)numbers[count - 1];
            //Console.WriteLine("count: {0:d}", count);
            //Console.WriteLine("sum: {0:f6}", sum);
            //Console.WriteLine("minimum: {0:f6}", minimum);
            //Console.WriteLine("median: {0:f6}", median);
            //Console.WriteLine("maximum: {0:f6}", maximum);
            //Console.WriteLine("mean: {0:f6}", mean);
            //Console.WriteLine("average_deviation: {0:f6}", average_deviation);
            //Console.WriteLine("standard_deviation: {0:f6}", standard_deviation);
            //Console.WriteLine("variance: {0:f6}", variance);
            //Console.WriteLine("skew: {0:f6}", skew);
            //Console.WriteLine("kurtosis: {0:f6}", kurtosis);
            // Constructs a NormalDist instance with mean of ten and variance of five.
            //NormalDist dist = new NormalDist(mean, variance);
            // The PDF() method computes the probability density function
            // evaluated at a given value. The probability of
            // observing a value of 8.7 is:
            //Console.WriteLine("PDF: " + dist.PDF(8.7));
            // The CDF() method computes the cumulative density function
            // evaluated at a given value. Find the probability of
            // observing 9.2 or less.
            //Console.WriteLine("CDF: " + dist.CDF(9.2));
            return new double[11] { minimum, median, maximum, mean, average_deviation, standard_deviation, variance, skew, kurtosis, signal_to_noise_ratio, coefficient_of_variation };
        }

        /// <summary>
        /// Get statistics
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static double[] GetStatistics(ref Hashtable hash)
        {
            double[] data = new double[hash.Count];
            int y = 0;
            IDictionaryEnumerator _enumerator = hash.GetEnumerator();
            while (_enumerator.MoveNext())
            {
                data[y] = double.Parse(_enumerator.Key.ToString());
                y++;
            }
            ArrayList numbers = new ArrayList();
            int count = 0;
            double minimum = 0.0;
            double median = 0.0;
            double maximum = 0.0;
            double sum = 0.0;
            double mean = 0.0;
            double average_deviation = 0.0;
            double standard_deviation = 0.0;
            double variance = 0.0;
            double skew = 0.0;
            double kurtosis = 0.0;
            double signal_to_noise_ratio = 0.0; // mean / standard_deviation;
            double coefficient_of_variation = 0.0; // (standard_deviation / mean) * 100;
            count = data.Length;
            for (int x = 0; x < count; x++)
            {
                sum += data[x];
                numbers.Add(data[x]);
            }
            mean = sum / count;
            for (int x = 0; x < count; x++)
            {
                double deviation = 0.0;
                deviation = (double)numbers[x] - mean;
                average_deviation += Math.Abs(deviation);
                variance += Math.Pow(deviation, 2);
                skew += Math.Pow(deviation, 3);
                kurtosis += Math.Pow(deviation, 4);
            }
            average_deviation /= count;
            variance /= (count - 1);
            standard_deviation = Math.Sqrt(variance);
            if (variance != 0.0)
            {
                skew /= (count * variance * standard_deviation);
                kurtosis = kurtosis / (count * variance * variance) - 3.0;
            }
            signal_to_noise_ratio = mean / standard_deviation;
            coefficient_of_variation = (standard_deviation / mean) * 100;
            numbers.Sort();
            minimum = (double)numbers[0];
            median = (count % 2 != 0) ? (double)numbers[count / 2] : ((double)numbers[count / 2] + (double)numbers[(count / 2) - 1]) / 2;
            maximum = (double)numbers[count - 1];
            //Console.WriteLine("count: {0:d}", count);
            //Console.WriteLine("sum: {0:f6}", sum);
            //Console.WriteLine("minimum: {0:f6}", minimum);
            //Console.WriteLine("median: {0:f6}", median);
            //Console.WriteLine("maximum: {0:f6}", maximum);
            //Console.WriteLine("mean: {0:f6}", mean);
            //Console.WriteLine("average_deviation: {0:f6}", average_deviation);
            //Console.WriteLine("standard_deviation: {0:f6}", standard_deviation);
            //Console.WriteLine("variance: {0:f6}", variance);
            //Console.WriteLine("skew: {0:f6}", skew);
            //Console.WriteLine("kurtosis: {0:f6}", kurtosis);
            // Constructs a NormalDist instance with mean of ten and variance of five.
            //NormalDist dist = new NormalDist(mean, variance);
            // The PDF() method computes the probability density function
            // evaluated at a given value. The probability of
            // observing a value of 8.7 is:
            //Console.WriteLine("PDF: " + dist.PDF(8.7));
            // The CDF() method computes the cumulative density function
            // evaluated at a given value. Find the probability of
            // observing 9.2 or less.
            //Console.WriteLine("CDF: " + dist.CDF(9.2));
            return new double[11] { minimum, median, maximum, mean, average_deviation, standard_deviation, variance, skew, kurtosis, signal_to_noise_ratio, coefficient_of_variation };
        }

        /// <summary>
        /// Get report of data quality
        /// </summary>
        /// <param name="marketname"></param>
        /// <param name="instrument"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDataQualityReport(string marketname, string instrument, string path)
        {
            Instrument inst = new Instrument(instrument, "stock", path + instrument + ".csv");
            ArrayList _holidaySchedule = new ArrayList();
            if (File.Exists(path + "!EXCEPTIONS_DATES.txt"))
            {
                StreamReader sr = new StreamReader(path + "!EXCEPTIONS_DATES.txt");
                string line = sr.ReadLine();
                while (line != null)
                {
                    _holidaySchedule.Add(DateTime.Parse(line));
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            DataQualityReport dataReport = new DataQualityReport(inst, _holidaySchedule);
            /*
            for (int x = 0; x < dataReport.MissingDates.Count; x++)
            {
                if (!_marketDateExceptions.ContainsKey(dataReport.MissingDates[x]))
                {
                    _marketDateExceptions.Add(dataReport.MissingDates[x], 1);
                }
                else
                    _marketDateExceptions[dataReport.MissingDates[x]] = (int)_marketDateExceptions[dataReport.MissingDates[x]] + 1;
            }
            if (!_lastDataDate.ContainsKey(dataReport._dateEnd))
            {
                _lastDataDate.Add(dataReport._dateEnd, 1);
            }
            else
                _lastDataDate[dataReport._dateEnd] = (int)_lastDataDate[dataReport._dateEnd] + 1;
            //if (dataReport.WeekendDates.Count > 0)
            //    MessageBox.Show("Weekend date detected!");
             */
            return dataReport.GetReport();
        }

        /// <summary>
        /// Get last date time of market close
        /// </summary>
        /// <returns></returns>
        public static DateTime GetPreviousMarketClose()
        {
            DateTime temp;
            DateTime lastClose = new DateTime();
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    temp = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));
                    lastClose = new DateTime(temp.Year, temp.Month, temp.Day, 17, 30, 0);
                    break;
                case DayOfWeek.Monday:
                    if (lastClose.Hour < 17)
                    {
                        temp = DateTime.Now.Subtract(new TimeSpan(3, 0, 0, 0));
                        lastClose = new DateTime(temp.Year, temp.Month, temp.Day, 17, 30, 0);
                    }
                    else
                        lastClose = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 30, 0);
                    break;
                case DayOfWeek.Tuesday:
                    if (lastClose.Hour < 17)
                    {
                        temp = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
                        lastClose = new DateTime(temp.Year, temp.Month, temp.Day, 17, 30, 0);
                    }
                    else
                        lastClose = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 30, 0);
                    break;
                case DayOfWeek.Wednesday:
                    if (lastClose.Hour < 17)
                    {
                        temp = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
                        lastClose = new DateTime(temp.Year, temp.Month, temp.Day, 17, 30, 0);
                    }
                    else
                        lastClose = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 30, 0);
                    break;
                case DayOfWeek.Thursday:
                    if (lastClose.Hour < 17)
                    {
                        temp = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
                        lastClose = new DateTime(temp.Year, temp.Month, temp.Day, 17, 30, 0);
                    }
                    else
                        lastClose = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 30, 0);
                    break;
                case DayOfWeek.Friday:
                    if (lastClose.Hour < 17)
                    {
                        temp = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
                        lastClose = new DateTime(temp.Year, temp.Month, temp.Day, 17, 30, 0);
                    }
                    else
                        lastClose = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 30, 0);
                    break;
                case DayOfWeek.Saturday:
                    temp = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
                    lastClose = new DateTime(temp.Year, temp.Month, temp.Day, 17, 30, 0);
                    break;
            }
            return lastClose;
        }

        public static DataTable ParseCSV(string inputString)
        {

            DataTable dt = new DataTable();

            // declare the Regular Expression that will match versus the input string
            Regex re = new Regex("((?<field>[^\",\\r\\n]+)|\"(?<field>([^\"]|\"\")+)\")(,|(?<rowbreak>\\r\\n|\\n|$))");

            ArrayList colArray = new ArrayList();
            ArrayList rowArray = new ArrayList();

            int colCount = 0;
            int maxColCount = 0;
            string rowbreak = "";
            string field = "";

            MatchCollection mc = re.Matches(inputString);

            foreach (Match m in mc)
            {

                // retrieve the field and replace two double-quotes with a single double-quote
                field = m.Result("${field}").Replace("\"\"", "\"");

                rowbreak = m.Result("${rowbreak}");

                if (field.Length > 0)
                {
                    colArray.Add(field);
                    colCount++;
                }

                if (rowbreak.Length > 0)
                {

                    // add the column array to the row Array List
                    rowArray.Add(colArray.ToArray());

                    // create a new Array List to hold the field values
                    colArray = new ArrayList();

                    if (colCount > maxColCount)
                        maxColCount = colCount;

                    colCount = 0;
                }
            }

            if (rowbreak.Length == 0)
            {
                // this is executed when the last line doesn't
                // end with a line break
                rowArray.Add(colArray.ToArray());
                if (colCount > maxColCount)
                    maxColCount = colCount;
            }

            // create the columns for the table
            for (int i = 0; i < maxColCount; i++)
                dt.Columns.Add(String.Format("col{0:000}", i));

            // convert the row Array List into an Array object for easier access
            Array ra = rowArray.ToArray();
            for (int i = 0; i < ra.Length; i++)
            {

                // create a new DataRow
                DataRow dr = dt.NewRow();

                // convert the column Array List into an Array object for easier access
                Array ca = (Array)(ra.GetValue(i));

                // add each field into the new DataRow
                for (int j = 0; j < ca.Length; j++)
                    dr[j] = ca.GetValue(j);

                // add the new DataRow to the DataTable
                dt.Rows.Add(dr);
            }

            // in case no data was parsed, create a single column
            if (dt.Columns.Count == 0)
                dt.Columns.Add("NoData");
            return dt;
        }

        public static DataTable ParseCSVFile(string path)
        {
            string inputString = "";
            // check that the file exists before opening it
            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path);
                inputString = sr.ReadToEnd();
                sr.Close();
            }
            return ParseCSV(inputString);
        }

        public static DataTable GetDataTableFromCsv(string path, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";

            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            string sql = @"SELECT * FROM [" + fileName + "]";

            using (OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly + ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public static DataTable GetDataTable(string filePath)
        {
            OleDbConnection conn = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + Path.GetDirectoryName(filePath) + ";Extended Properties=Text;HDR=YES;FMT=Delimited");
            conn.Open();
            string strQuery = "SELECT * FROM [" + Path.GetFileName(filePath) + "]";
            OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);
            DataSet ds = new System.Data.DataSet("CSV File");
            adapter.Fill(ds);
            return ds.Tables[0];
        }

        public static DataTable GetCSV(string path)
        {
            string CSVFilePathName = path;
            string[] Lines = File.ReadAllLines(CSVFilePathName);
            string[] Fields;
            Fields = Lines[0].Split(new char[] { ',' });
            int Cols = Fields.GetLength(0);
            DataTable dt = new DataTable();
            //1st row must be column names; force lower case to ensure matching later on.
            for (int i = 0; i < Cols; i++)
                dt.Columns.Add(Fields[i].ToLower(), typeof(string));
            DataRow Row;
            for (int i = 1; i < Lines.GetLength(0); i++)
            {
                Fields = Lines[i].Split(new char[] { ',' });
                Row = dt.NewRow();
                for (int f = 0; f < Cols; f++)
                    Row[f] = Fields[f];
                dt.Rows.Add(Row);
            }
            return dt;   
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsStart"></param>
        /// <param name="iTbl"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="ad"></param>
        /// <returns></returns>
        public static DataSet FilterSortData(DataSet dsStart, int iTbl, string filter, string sort, int ad)
        {
            DataTable dt = dsStart.Tables[iTbl].Clone();

            if (ad == 0)
            {
                DataRow[] drs = dsStart.Tables[iTbl].Select(filter, sort);
                foreach (DataRow dr in drs)
                {
                    dt.ImportRow(dr);
                }
            }
            else
            {
                DataRow[] drs = dsStart.Tables[iTbl].Select(filter, sort + " DESC");
                foreach (DataRow dr in drs)
                {
                    dt.ImportRow(dr);
                }
            }
            DataSet ds = new DataSet();
            for (int i = 0; i < dsStart.Tables.Count; i++)
            {
                if (i == iTbl)
                {
                    ds.Tables.Add(dt);
                }
                else
                {
                    ds.Tables.Add(dsStart.Tables[i].Copy());
                }
            }
            return ds;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsStart"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="ad"></param>
        /// <returns></returns>
        public static DataTable FilterSortDataTable(DataTable dsStart, string filter, string sort, int ad)
        {
            DataTable dt = dsStart.Clone();

            if (ad == 0)
            {
                DataRow[] drs = dsStart.Select(filter, sort);
                foreach (DataRow dr in drs)
                {
                    dt.ImportRow(dr);
                }
            }
            else
            {
                DataRow[] drs = dsStart.Select(filter, sort + " DESC");
                foreach (DataRow dr in drs)
                {
                    dt.ImportRow(dr);
                }
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="yyyy"></param>
        /// <returns></returns>
        public static bool IsLeapYear(int yyyy)
        {

            if ((yyyy % 4 == 0 && yyyy % 100 != 0) || (yyyy % 400 == 0))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int ISOWeekNumber(DateTime dt)
        {

            // Set Year
            int yyyy = dt.Year;

            // Set Month
            int mm = dt.Month;

            // Set Day
            int dd = dt.Day;

            // Declare other required variables
            int DayOfYearNumber;
            int Jan1WeekDay;
            int WeekNumber = 0, WeekDay;


            int i, j, k, l, m, n;
            int[] Mnth = new int[12] { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334 };

            int YearNumber;


            // Set DayofYear Number for yyyy mm dd
            DayOfYearNumber = dd + Mnth[mm - 1];

            // Increase of Dayof Year Number by 1, if year is leapyear and month is february
            if ((IsLeapYear(yyyy) == true) && (mm == 2))
                DayOfYearNumber += 1;

            // Find the Jan1WeekDay for year 
            i = (yyyy - 1) % 100;
            j = (yyyy - 1) - i;
            k = i + i / 4;
            Jan1WeekDay = 1 + (((((j / 100) % 4) * 5) + k) % 7);

            // Calcuate the WeekDay for the given date
            l = DayOfYearNumber + (Jan1WeekDay - 1);
            WeekDay = 1 + ((l - 1) % 7);

            // Find if the date falls in YearNumber set WeekNumber to 52 or 53
            if ((DayOfYearNumber <= (8 - Jan1WeekDay)) && (Jan1WeekDay > 4))
            {
                YearNumber = yyyy - 1;
                if ((Jan1WeekDay == 5) || ((Jan1WeekDay == 6) && (Jan1WeekDay > 4)))
                    WeekNumber = 53;
                else
                    WeekNumber = 52;
            }
            else
                YearNumber = yyyy;


            // Set WeekNumber to 1 to 53 if date falls in YearNumber
            if (YearNumber == yyyy)
            {
                if (IsLeapYear(yyyy) == true)
                    m = 366;
                else
                    m = 365;
                if ((m - DayOfYearNumber) < (4 - WeekDay))
                {
                    YearNumber = yyyy + 1;
                    WeekNumber = 1;
                }
            }

            if (YearNumber == yyyy)
            {
                n = DayOfYearNumber + (7 - WeekDay) + (Jan1WeekDay - 1);
                WeekNumber = n / 7;
                if (Jan1WeekDay > 4)
                    WeekNumber -= 1;
            }

            return (WeekNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int WeekDay(DateTime dt)
        {

            // Set Year
            int yyyy = dt.Year;

            // Set Month
            int mm = dt.Month;

            // Set Day
            int dd = dt.Day;

            // Declare other required variables
            int DayOfYearNumber;
            int Jan1WeekDay;
            int WeekDay;


            int i, j, k, l;
            int[] Mnth = new int[12] { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334 };


            // Set DayofYear Number for yyyy mm dd
            DayOfYearNumber = dd + Mnth[mm - 1];

            // Increase of Dayof Year Number by 1, if year is leapyear and month is february
            if ((IsLeapYear(yyyy) == true) && (mm == 2))
                DayOfYearNumber += 1;

            // Find the Jan1WeekDay for year 
            i = (yyyy - 1) % 100;
            j = (yyyy - 1) - i;
            k = i + i / 4;
            Jan1WeekDay = 1 + (((((j / 100) % 4) * 5) + k) % 7);

            // Calcuate the WeekDay for the given date
            l = DayOfYearNumber + (Jan1WeekDay - 1);
            WeekDay = 1 + ((l - 1) % 7);

            return WeekDay;
        }

        /// <summary>
        /// This class has static methods to manage collections.
        /// </summary>
        public class CollectionsSupport
        {
            /// <summary>
            /// Copies the IList to other IList.
            /// </summary>
            /// <param name="SourceList">IList source.</param>
            /// <param name="TargetList">IList target.</param>
            public static void Copy(System.Collections.IList SourceList, System.Collections.IList TargetList)
            {
                for (int i = 0; i < SourceList.Count; i++)
                    TargetList[i] = SourceList[i];
            }

            /// <summary>
            /// Replaces the elements of the specified list with the specified element.
            /// </summary>
            /// <param name="List">The list to be filled with the specified element.</param>
            /// <param name="Element">The element with which to fill the specified list.</param>
            public static void Fill(System.Collections.IList List, System.Object Element)
            {
                for (int i = 0; i < List.Count; i++)
                    List[i] = Element;
            }

            /// <summary>
            /// This class implements System.Collections.IComparer and is used for Comparing two String objects by evaluating 
            /// the numeric values of the corresponding Char objects in each string.
            /// </summary>
            class CompareCharValues : System.Collections.IComparer
            {
                public int Compare(System.Object x, System.Object y)
                {
                    return string.CompareOrdinal((string)x, (string)y);
                }
            }

            /// <summary>
            /// Obtain the maximum element of the given collection with the specified comparator.
            /// </summary>
            /// <param name="Collection">Collection from which the maximum value will be obtained.</param>
            /// <param name="Comparator">The comparator with which to determine the maximum element.</param>
            /// <returns></returns>
            public static System.Object Max(System.Collections.ICollection Collection, System.Collections.IComparer Comparator)
            {
                System.Collections.ArrayList tempArrayList;

                if (((System.Collections.ArrayList)Collection).IsReadOnly)
                    throw new System.NotSupportedException();

                if ((Comparator == null) || (Comparator is System.Collections.Comparer))
                {
                    try
                    {
                        tempArrayList = new System.Collections.ArrayList(Collection);
                        tempArrayList.Sort();
                    }
                    catch (System.InvalidOperationException e)
                    {
                        throw new System.InvalidCastException(e.Message);
                    }
                    return (System.Object)tempArrayList[Collection.Count - 1];
                }
                else
                {
                    try
                    {
                        tempArrayList = new System.Collections.ArrayList(Collection);
                        tempArrayList.Sort(Comparator);
                    }
                    catch (System.InvalidOperationException e)
                    {
                        throw new System.InvalidCastException(e.Message);
                    }
                    return (System.Object)tempArrayList[Collection.Count - 1];
                }
            }
            /// <summary>
            /// Obtain the minimum element of the given collection with the specified comparator.
            /// </summary>
            /// <param name="Collection">Collection from which the minimum value will be obtained.</param>
            /// <param name="Comparator">The comparator with which to determine the minimum element.</param>
            /// <returns></returns>
            public static System.Object Min(System.Collections.ICollection Collection, System.Collections.IComparer Comparator)
            {
                System.Collections.ArrayList tempArrayList;

                if (((System.Collections.ArrayList)Collection).IsReadOnly)
                    throw new System.NotSupportedException();

                if ((Comparator == null) || (Comparator is System.Collections.Comparer))
                {
                    try
                    {
                        tempArrayList = new System.Collections.ArrayList(Collection);
                        tempArrayList.Sort();
                    }
                    catch (System.InvalidOperationException e)
                    {
                        throw new System.InvalidCastException(e.Message);
                    }
                    return (System.Object)tempArrayList[0];
                }
                else
                {
                    try
                    {
                        tempArrayList = new System.Collections.ArrayList(Collection);
                        tempArrayList.Sort(Comparator);
                    }
                    catch (System.InvalidOperationException e)
                    {
                        throw new System.InvalidCastException(e.Message);
                    }
                    return (System.Object)tempArrayList[0];
                }
            }


            /// <summary>
            /// Sorts an IList collections
            /// </summary>
            /// <param name="list">The System.Collections.IList instance that will be sorted</param>
            /// <param name="Comparator">The Comparator criteria, null to use natural comparator.</param>
            public static void Sort(System.Collections.IList list, System.Collections.IComparer Comparator)
            {
                if (((System.Collections.ArrayList)list).IsReadOnly)
                    throw new System.NotSupportedException();

                if ((Comparator == null) || (Comparator is System.Collections.Comparer))
                {
                    try
                    {
                        ((System.Collections.ArrayList)list).Sort();
                    }
                    catch (System.InvalidOperationException e)
                    {
                        throw new System.InvalidCastException(e.Message);
                    }
                }
                else
                {
                    try
                    {
                        ((System.Collections.ArrayList)list).Sort(Comparator);
                    }
                    catch (System.InvalidOperationException e)
                    {
                        throw new System.InvalidCastException(e.Message);
                    }
                }
            }

            /// <summary>
            /// Shuffles the list randomly.
            /// </summary>
            /// <param name="List">The list to be shuffled.</param>
            public static void Shuffle(System.Collections.IList List)
            {
                System.Random RandomList = new System.Random(unchecked((int)DateTime.Now.Ticks));
                Shuffle(List, RandomList);
            }

            /// <summary>
            /// Shuffles the list randomly.
            /// </summary>
            /// <param name="List">The list to be shuffled.</param>
            /// <param name="RandomList">The random to use to shuffle the list.</param>
            public static void Shuffle(System.Collections.IList List, System.Random RandomList)
            {
                System.Object source = null;
                int target = 0;

                for (int i = 0; i < List.Count; i++)
                {
                    target = RandomList.Next(List.Count);
                    source = (System.Object)List[i];
                    List[i] = List[target];
                    List[target] = source;
                }
            }
        }

        /// <summary>
        /// SupportClass for the HashSet class.
        /// </summary>
        [Serializable]
        public class HashSetSupport : System.Collections.ArrayList, SetSupport
        {
            public HashSetSupport()
                : base()
            {
            }

            public HashSetSupport(System.Collections.ICollection c)
            {
                this.AddAll(c);
            }

            public HashSetSupport(int capacity)
                : base(capacity)
            {
            }

            /// <summary>
            /// Adds a new element to the ArrayList if it is not already present.
            /// </summary>		
            /// <param name="obj">Element to insert to the ArrayList.</param>
            /// <returns>Returns true if the new element was inserted, false otherwise.</returns>
            new public virtual bool Add(System.Object obj)
            {
                bool inserted;

                if ((inserted = this.Contains(obj)) == false)
                {
                    base.Add(obj);
                }

                return !inserted;
            }

            /// <summary>
            /// Adds all the elements of the specified collection that are not present to the list.
            /// </summary>
            /// <param name="c">Collection where the new elements will be added</param>
            /// <returns>Returns true if at least one element was added, false otherwise.</returns>
            public bool AddAll(System.Collections.ICollection c)
            {
                System.Collections.IEnumerator e = new System.Collections.ArrayList(c).GetEnumerator();
                bool added = false;

                while (e.MoveNext() == true)
                {
                    if (this.Add(e.Current) == true)
                        added = true;
                }

                return added;
            }

            /// <summary>
            /// Returns a copy of the HashSet instance.
            /// </summary>		
            /// <returns>Returns a shallow copy of the current HashSet.</returns>
            public override System.Object Clone()
            {
                return base.MemberwiseClone();
            }
        }

        /// <summary>
        /// Represents a collection ob objects that contains no duplicate elements.
        /// </summary>	
        public interface SetSupport : System.Collections.ICollection, System.Collections.IList
        {
            /// <summary>
            /// Adds a new element to the Collection if it is not already present.
            /// </summary>
            /// <param name="obj">The object to add to the collection.</param>
            /// <returns>Returns true if the object was added to the collection, otherwise false.</returns>
            new bool Add(System.Object obj);

            /// <summary>
            /// Adds all the elements of the specified collection to the Set.
            /// </summary>
            /// <param name="c">Collection of objects to add.</param>
            /// <returns>true</returns>
            bool AddAll(System.Collections.ICollection c);
        }
    }
}
