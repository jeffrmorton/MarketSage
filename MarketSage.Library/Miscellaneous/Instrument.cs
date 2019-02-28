using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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
    public class Instrument
    {
        private string _name;
        private string _type;
        private string _file;
        private ArrayList _technicalData;
        private DateTime _technicalDataStartDate;
        private DateTime _technicalDataEndDate;
        private TechnicalData _technicalDataCurrent;
        private int _technicalDataIndex;

        public Instrument(ArrayList technicalData, string name)
        {
            _technicalData = technicalData;
            _name = name;
        }

        public Instrument(string name, string type, string file)
        {
            _technicalData = new ArrayList();
            _name = name;
            _type = type;
            _file = file;
            try
            {
                Load(_file);
                Initialize();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(file + "\r\n" + e.ToString());
            }
        }

        public void Load(string file)
        {
            if (File.Exists(file))
            {
                try
                {
                    StreamReader file_reader = new StreamReader(file, Encoding.Default);
                    StreamReader buffered_reader = new System.IO.StreamReader(file_reader.BaseStream, file_reader.CurrentEncoding);
                    string line;
                    ArrayList temp = new ArrayList();
                    buffered_reader.ReadLine();  // skip header
                    while ((line = buffered_reader.ReadLine()) != null)
                    {
                        TechnicalData data = ParseLine(line);
                        if (data != null)
                        {
                            temp.Add(data);
                        }
                    }
                    for (int i = temp.Count - 1; i >= 0; i--)
                    {
                        _technicalData.Add(temp[i]);
                    }
                    _technicalDataStartDate = ((TechnicalData)_technicalData[0]).Date;
                    _technicalDataEndDate = ((TechnicalData)_technicalData[_technicalData.Count - 1]).Date;
                    buffered_reader.Close();
                    file_reader.Close();
                }
                finally
                {
                }
            }
        }

        private TechnicalData ParseLine(string line)
        {
            string[] dataArray = new string[7];
            Regex rex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            dataArray = rex.Split(line);
            DateTime dateTime = DateTime.Parse(dataArray[0]);
            TechnicalData data = new TechnicalData(dateTime, double.Parse(dataArray[1]), double.Parse(dataArray[2]), double.Parse(dataArray[3]), double.Parse(dataArray[4]), long.Parse(dataArray[5]), double.Parse(dataArray[6]));
            return data;
        }

        public ArrayList TechnicalData
        {
            get { return _technicalData; }
        }

        public DateTime Date
        {
            get { return GetCurrent().Date; }
        }

        public double Close
        {
            get { return GetCurrent().Close; }
        }

        public double High
        {
            get { return GetCurrent().High; }
        }

        public double AdjHigh
        {
            get { return GetCurrent().AdjHigh; }
        }


        public double Low
        {
            get { return GetCurrent().Low; }
        }

        public double AdjLow
        {
            get { return GetCurrent().AdjLow; }
        }


        public double Open
        {
            get { return GetCurrent().Open; }
        }

        public double AdjOpen
        {
            get { return GetCurrent().AdjOpen; }
        }


        public long Volume
        {
            get { return GetCurrent().Volume; }
        }

        public long AdjVolume
        {
            get { return GetCurrent().AdjVolume; }
        }


        public double AdjClose
        {
            get { return GetCurrent().AdjClose; }
        }

        public string Name
        {
            get { return _name; }
        }

        public void Initialize()
        {
            _technicalDataIndex = 0;
            _technicalDataCurrent = null;
        }

        public TechnicalData GetCurrent()
        {
            if (_technicalDataCurrent == null)
            {
                _technicalDataCurrent = (TechnicalData)_technicalData[_technicalDataIndex];
            }
            return _technicalDataCurrent;
        }

        public virtual bool HasNext()
        {
            if (_technicalData.Count == 0)
                return false;
            else
                return (_technicalDataIndex < _technicalData.Count - 1);
        }

        public virtual bool NextToLast()
        {
            if (_technicalData.Count == 0)
                return false;
            else
                return !(_technicalDataIndex < _technicalData.Count - 2);
        }

        public virtual void Next()
        {
            if (HasNext())
            {
                _technicalDataIndex++;
                _technicalDataCurrent = null;
            }
        }
    }
}
