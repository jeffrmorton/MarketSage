using System;
using System.Collections;
using MarketSage.Library;

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

namespace MarketSage.Plugin
{
    /// <summary>Williams %R is a momentum indicator that measures overbought/oversold levels, and was developed by Larry Williams</summary>
    public class WillR : IIndicator
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _direction;
        private int _period = 14;
        private double _buylevel;
        private double _selllevel;
        string myName = "Williams %R";
        string myDescription = "Developed by Larry Williams, Williams %R is a momentum indicator that works much like the Stochastic Oscillator. It is especially popular for measuring overbought and oversold levels. The scale ranges from 0 to -100 with readings from 0 to -20 considered overbought, and readings from -80 to -100 considered oversold.";
        string myAuthor = "Larry Williams";
        string myVersion = "1.0.0";

        /// <summary>Creates new %R object of IIndicator interface</summary>
        public WillR()
        {
            _period = 14;
            _buylevel = 80;
            _selllevel = 20;
            Initialize();
        }

        /// <summary>Creates new %R object of IIndicator interface</summary>
        /// <param name="period">Period</param>
        public WillR(int period)
        {
            _period = period;
            _buylevel = 80;
            _selllevel = 20;
            Initialize();
        }

        /// <summary>Creates new %R object of IIndicator interface</summary>
        /// <param name="period">Period</param>
        /// <param name="buylevel">Buy level</param>
        /// <param name="selllevel">Sell Level</param>
        public WillR(int period, double buylevel, double selllevel)
        {
            _period = period;
            if (selllevel >= buylevel)
            {
                throw new System.ArgumentException("%R : buy must be > sell level");
            }
            _buylevel = buylevel;
            _selllevel = selllevel;
            Initialize();
        }

        public void Initialize()
        {
            _name = "WillR." + _period + "," + _buylevel + "," + _selllevel;
            _data = new ArrayList();
            _ready = false;
            _direction = 9;
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
            return _direction;
        }

        /// <summary>Indicates buy signal</summary>
        /// <returns>bool</returns>
        public bool IsBuy()
        {
            return _direction == 1;
        }

        /// <summary>Indicates hold signal</summary>
        /// <returns>bool</returns>
        public bool IsHold()
        {
            return _direction == 0;
        }

        /// <summary>Indicates sell signal</summary>
        /// <returns>bool</returns>
        public bool IsSell()
        {
            return _direction == -1;
        }

        /// <summary>Adds TechnicalData object to the indicator</summary>
        /// <param name="data">TechnicalData object to be added</param>
        public virtual void AddData(TechnicalData data)
        {
            double current = data.AdjClose;
            Value v = new Value(this, data.AdjHigh, data.AdjLow);

            if (!_ready)
            {
                _data.Add(v);

                if (_data.Count >= _period)
                {
                    _ready = true;
                    GenerateIndicator(current);
                }
            }
            else
            {
                _data.RemoveAt(0);
                _data.Add(v);
                GenerateIndicator(current);
            }
        }

        private void GenerateIndicator(double current)
        {
            // wr = (100 * (currhigh - current)) / (currhigh - currlow);

            double currhigh = current;
            double currlow = current;

            for (int i = 0; i < _data.Count; i++)
            {
                Value v = (Value)_data[i];

                if (v.AdjHigh > currhigh)
                {
                    currhigh = v.AdjHigh;
                }
                else if (v.AdjLow < currlow)
                {
                    currlow = v.AdjLow;
                }
            }

            double wr = (100 * (currhigh - current)) / (currhigh - currlow);

            if (wr >= _buylevel)
            {
                _direction = 2;
            }
            else if (wr <= _selllevel)
            {
                _direction = -2;
            }
            else
            {
                if (_direction == -2)
                {
                    _direction = -1;
                }
                else if (_direction == 2)
                {
                    _direction = 1;
                }
                else
                {
                    _direction = 0;
                }
            }
        }

        internal class Value
        {
            private WillR _enclosingInstance;
            private double _high;
            private double _low;

            internal Value(WillR enclosingInstance, double high, double low)
            {
                _enclosingInstance = enclosingInstance;
                _high = high;
                _low = low;
            }

            virtual internal double AdjHigh
            {
                get { return _high; }
            }

            virtual internal double AdjLow
            {
                get { return _low; }
            }

            public WillR Enclosing_Instance
            {
                get { return _enclosingInstance; }
            }
        }
    }
}