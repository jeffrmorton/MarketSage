using System;
using System.Collections;
using MarketSage.Library;

/* MarketBot
   Copyright (C) 2005, 2006 Jeff Morton
 
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

namespace Technical
{
    /// <summary>Previous Close</summary>
    public class PrevClose : IIndicator
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _signal;
        private int _period = 14;
        private double _low = 2.00;
        private double _high = 9.95;
        string myName = "Previous Close";
        string myDescription = "";
        string myAuthor = "Jeffrey Morton";
        string myVersion = "1.0.0";


        /// <summary>Creates new PrevClose object of IIndicator interface</summary>
        public PrevClose()
        {
            _name = "PrevClose." + _period + "," + _low + "," + _high;
            _data = new ArrayList();
        }

        /// <summary>Creates new PrevClose object of IIndicator interface</summary>
        /// <param name="period">Period</param>
        public PrevClose(int period)
        {
            _period = period;
            _name = "PrevClose." + _period + "," + _low + "," + _high;
            _data = new ArrayList();
        }

        /// <summary>Creates new PrevClose object of IIndicator interface</summary>
        /// <param name="period">Period</param>
        /// <param name="buylevel">Buy level</param>
        /// <param name="selllevel">Sell Level</param>
        public PrevClose(int period, double buylevel, double selllevel)
        {
            _period = period;
            if (buylevel >= selllevel)
            {
                throw new System.ArgumentException("PrevClose : low be < high");
            }
            _low = buylevel;
            _high = selllevel;
            _name = "PrevClose." + _period + "," + _low + "," + _high;
            _data = new ArrayList();
        }
        public string Name
        {
            get { return myName; }
        }
        public string Description
        {
            get { return myDescription; }
        }
        public string Author
        {
            get { return myAuthor; }
        }
        public string Version
        {
            get { return myVersion; }
        }
        public int Period
        {
            get { return _period; }
            set { _period = value; }
        }

        /// <summary>Returns name</summary>
        /// <returns>string</returns>
        public string GetName()
        {
            return _name;
        }

        /// <summary>Returns readyness of the indicator</summary>
        /// <returns>bool</returns>
        public bool IsReady()
        {
            return _ready;
        }

        /// <summary>Returns direction</summary>
        /// <returns>int</returns>
        public int GetDirection()
        {
            return _signal;
        }

        /// <summary>Indicates buy signal</summary>
        /// <returns>bool</returns>
        public bool IsBuy()
        {
            return _signal == 1;
        }

        /// <summary>Indicates hold signal</summary>
        /// <returns>bool</returns>
        public bool IsHold()
        {
            return _signal == 0;
        }

        /// <summary>Indicates sell signal</summary>
        /// <returns>bool</returns>
        public bool IsSell()
        {
            return _signal == -1;
        }

        /// <summary>Adds TechnicalData object to the indicator</summary>
        /// <param name="data">TechnicalData object to be added</param>
        public virtual void AddData(TechnicalData data)
        {
            if (!_ready)
            {
                _data.Add(data);

                if (_data.Count >= _period)
                {
                    _ready = true;
                    GenerateSignal();
                }
            }
            else
            {
                _data.RemoveAt(0);
                _data.Add(data);
                GenerateSignal();
            }
        }

        private void GenerateSignal()
        {
            double ma = 0;

            for (int i = 0; i < _period; i++)
            {
                ma += ((TechnicalData)_data[i]).AdjClose;
            }
            ma = ma / _period;

            if (ma < _high && ma > _low)
            {
                _signal = 1;
            }
            else
            {
                _signal = -1;
            }
        }
    }
}