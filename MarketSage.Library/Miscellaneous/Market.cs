using System;
using System.Collections;
using System.Xml;
using System.Data;
using System.IO;

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
    public class Market
    {
        private string _name;
        private string _directory;
        public ArrayList _instruments = new ArrayList();
        public ArrayList _instrumentNames = new ArrayList();
        private DataSet dataset = new DataSet("Instruments");
        public bool random = false;

        public ArrayList GetTechnicalData(string instrument)
        {
            Instrument qfile = new Instrument(_name, "", _directory + _name + "\\" + instrument + ".csv");
            return qfile.TechnicalData;
        }

        public void Set(Hashtable instruments)
        {
            IDictionaryEnumerator en = instruments.GetEnumerator();
            _instruments.Clear();
            _instrumentNames.Clear();

            while (en.MoveNext())
            {
                _instruments.Add(en.Key);
                _instrumentNames.Add(en.Value);
            }
        }

        public void Set(string directory, string market, string symbol, string name)
        {
            _instruments.Clear();
            _instrumentNames.Clear();
            _directory = directory;
            _name = market;
            _instruments.Add(symbol);
            _instrumentNames.Add(name);
        }

        public void Load(string directory, string market, bool isexceptions)
        {
            _directory = directory;
            _name = market;
            ArrayList exceptions = new ArrayList();

            try
            {
                if (isexceptions && File.Exists(_directory + _name + "\\" + "!EXCEPTIONS_SYMBOLS.txt"))
                {
                    StreamReader sr = new StreamReader(_directory + _name + "\\" + "!EXCEPTIONS_SYMBOLS.txt");
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        exceptions.Add(line);
                        line = sr.ReadLine();
                    }
                    sr.Close();
                }

                dataset.ReadXml(_directory + _name + "\\" + "!INDEX.xml", System.Data.XmlReadMode.InferSchema);
                foreach (DataRow row in dataset.Tables[1].Rows)
                {
                    if (!exceptions.Contains(row["symbol"].ToString()))
                    {
                        _instruments.Add(row["symbol"].ToString());
                        _instrumentNames.Add(row["name"].ToString());
                    }
                }
                /*
                if (isexceptions && File.Exists(_directory + _name + "\\" + _name + ".exceptions"))
                {
                    if (File.Exists(_directory + _name + "\\_" + _name + ".processed"))
                        File.Delete(_directory + _name + "\\_" + _name + ".processed");
                    StreamWriter sw = new StreamWriter(_directory + _name + "\\_" + _name + ".processed");
                    for (int i = 0; i < _instruments.Count; i++)
                    {
                        sw.WriteLine(_instruments[i].ToString());
                    }
                    sw.Close();
                }
                 */
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Load(string directory, string market)
        {
            _directory = directory;
            _name = market;

            try
            {
                dataset.ReadXml(_directory + _name + "\\" + "!INDEX.xml", System.Data.XmlReadMode.InferSchema);
                foreach (DataRow row in dataset.Tables[1].Rows)
                {
                    _instruments.Add(row["symbol"].ToString());
                    _instrumentNames.Add(row["name"].ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}