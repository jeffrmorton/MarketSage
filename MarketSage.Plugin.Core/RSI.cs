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
    /// <summary>Relative Strength Index</summary>
    public class RSI : IIndicator
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _direction;
        private int _period;
        private double _buylevel;
        private double _selllevel;
        private double _previous;
        string myName = "Relative Strength Index";
        string myDescription = "Developed by J. Welles Wilder and introduced in his 1978 book, New Concepts in Technical Trading Systems, the Relative Strength Index (RSI) is an extremely useful and popular momentum oscillator. The RSI compares the magnitude of a stock's recent gains to the magnitude of its recent losses and turns that information into a number that ranges from 0 to 100. It takes a single parameter, the number of time periods to use in the calculation. In his book, Wilder recommends using 14 periods.";
        string myAuthor = "J. Welles Wilder";
        string myVersion = "1.0.0";

        /// <summary>Creates new RSI object of IIndicator interface</summary>
        public RSI()
        {
            _period = 14;
            _buylevel = 30;
            _selllevel = 70;
            Initialize();
        }

        /// <summary>Creates new RSI object of IIndicator interface</summary>
        /// <param name="period">Period</param>
        public RSI(int period)
        {
            _period = period;
            _buylevel = 30;
            _selllevel = 70;
            Initialize();
        }

        /// <summary>Creates new RSI object of IIndicator interface</summary>
        /// <param name="period">Period</param>
        /// <param name="buylevel">Buy level</param>
        /// <param name="selllevel">Sell level</param>
        public RSI(int period, double buylevel, double selllevel)
        {
            _period = period;
            if (buylevel >= selllevel)
            {
                throw new System.ArgumentException("RSI : buy level must be < sell level");
            }
            _buylevel = buylevel;
            _selllevel = selllevel;
            Initialize();
        }

        public void Initialize()
        {
            _name = "RSI." + _period + "," + _buylevel + "," + _selllevel;
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
        public void AddData(TechnicalData data)
        {
            double current = data.AdjClose;

            if (_previous > 0)
            {
                if (!_ready)
                {
                    _data.Add(makeValue(current));

                    if (_data.Count >= _period)
                    {
                        _ready = true;
                        GenerateIndicator();
                    }
                }
                else
                {
                    _data.RemoveAt(0);
                    _data.Add(makeValue(current));
                    GenerateIndicator();
                }
            }

            _previous = current;
        }

        private void GenerateIndicator()
        {
            // rsi = (100d - 100d / (upsum / downsum + 1d))

            double upsum = 0;
            double downsum = 0;

            for (int i = 0; i < _data.Count; i++)
            {
                Value v = (Value)_data[i];

                switch (v.Direction)
                {

                    case 1:
                        upsum += v.Price;
                        break;

                    case -1:
                        downsum += v.Price;
                        break;

                    default:
                        break;

                }
            }

            double rsi = 100d - (100d / (upsum / downsum + 1d));

            if (rsi <= _buylevel)
            {
                _direction = 1;
            }
            else if (rsi >= _selllevel)
            {
                _direction = -1;
            }
            else
            {
                _direction = 0;
            }
        }

        private Value makeValue(double current)
        {
            int direction = 9;

            if (current > _previous)
            {
                direction = 1;
            }
            else if (current < _previous)
            {
                direction = -1;
            }
            else
            {
                direction = 0;
            }

            Value val = new Value(this, current, direction);

            return val;
        }

        internal class Value
        {
            private RSI _enclosingInstance;
            private double _price;
            private int _direction;

            internal Value(RSI enclosingInstance, double price, int direction)
            {
                _enclosingInstance = enclosingInstance;
                _price = price;
                _direction = direction;
            }

            virtual internal int Direction
            {
                get { return _direction; }
            }

            virtual internal double Price
            {
                get { return _price; }
            }

            public RSI Enclosing_Instance
            {
                get { return _enclosingInstance; }
            }
        }
    }
}