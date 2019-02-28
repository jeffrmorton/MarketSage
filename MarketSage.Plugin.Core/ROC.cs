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
    /// <summary>The Rate of Change</summary>
    public class ROC : IIndicator
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _direction;
        private int _period;
        private double _buylevel;
        private double _selllevel;
        string myName = "Rate of Change";
        string myDescription = "The Rate of Change (ROC) indicator is a very simple yet effective momentum oscillator that measures the percent change in price from one period to the next. The ROC calculation compares the current price with the price n periods ago.";
        string myAuthor = "";
        string myVersion = "1.0.0";

        /// <summary>Creates new ROC object of IIndicator interface</summary>
        public ROC()
        {
            _period = 10;
            _buylevel = -8;
            _selllevel = 8;
            Initialize();
        }

        /// <summary>Creates new ROC object of IIndicator interface</summary>
        /// <param name="period">Period</param>
        public ROC(int period)
        {
            _period = period;
            _buylevel = -8;
            _selllevel = 8;
            Initialize();
        }

        /// <summary>Creates new ROC object of IIndicator interface</summary>
        /// <param name="period">Period</param>
        /// <param name="buylevel">Buy level</param>
        /// <param name="selllevel">Sell Level</param>
        public ROC(int period, int buylevel, int selllevel)
        {
            _period = period;
            if (buylevel >= selllevel)
            {
                throw new System.ArgumentException("ROC : buy must be < sell level");
            }
            _buylevel = buylevel;
            _selllevel = selllevel;
            Initialize();
        }

        public void Initialize()
        {
            _name = "ROC." + _period + "," + _buylevel + "," + _selllevel;
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
        public virtual void AddData(TechnicalData data)
        {
            double current = data.AdjClose;
            Value v = new Value(this, current);

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
            // 100 * (current - oldclose) / oldclose

            double oldclose = ((Value)_data[0]).AdjClose;
            double roc = 100 * (current - oldclose) / oldclose;

            if (roc <= _buylevel)
            {
                _direction = 2;
            }
            else if (roc >= _selllevel)
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
            private ROC _enclosingInstance;
            private double _close;

            internal Value(ROC enclosingInstance, double close)
            {
                _enclosingInstance = enclosingInstance;
                _close = close;
            }

            virtual internal double AdjClose
            {
                get { return _close; }
            }

            public ROC Enclosing_Instance
            {
                get { return _enclosingInstance; }
            }
        }
    }
}