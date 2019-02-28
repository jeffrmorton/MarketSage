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
    /// <summary>The DeMark Range Expansion Index is a market-timing oscillator described in DeMark on Day Trading Options, by T.R. DeMark and T.R. Demark, Jr., McGraw Hill, 1999</summary>
    public class DeM : IIndicator
    {
        private ArrayList _data;
        private bool _ready;
        private double _buylevel;
        private string _name;
        private double _prevhigh;
        private double _prevlow;
        private double _selllevel;
        private int _direction;
        private int _period;
        string myName = "DeMarker";
        string myDescription = "An indicator used in technical analysis that compares the most recent price action to the previous period's price in an attempt to measure the demand of the underlying asset. This indicator is generally used to identify price exhaustion and can also be used to identify market tops and bottoms. This oscillator is bounded between -100 and +100 and, unlike many other oscillators, it does not use smoothed data.";
        string myAuthor = "Thomas DeMark";
        string myVersion = "1.0.0";

        /// <summary>Creates new DeM object of IIndicator interface</summary>
        public DeM()
        {
            _period = 14;
            _buylevel = 30;
            _selllevel = 70;
            Initialize();
        }

        /// <summary>Creates new DeM object of IIndicator interface</summary>
        /// <param name="period">Period</param>
        public DeM(int period)
        {
            _period = period;
            _buylevel = 30;
            _selllevel = 70;
            Initialize();
        }

        /// <summary>Creates new DeM object of IIndicator interface</summary>
        /// <param name="period">Period</param>
        /// <param name="buylevel">Buy level</param>
        /// <param name="selllevel">Sell Level</param>
        public DeM(int period, double buylevel, double selllevel)
        {
            _period = period;
            if (buylevel >= selllevel)
            {
                throw new System.ArgumentException("DeM : buy level must be < sell level");
            }
            _buylevel = buylevel;
            _selllevel = selllevel;
            Initialize();
        }

        public void Initialize()
        {
            _name = "DeM." + _period + "," + _buylevel + "," + _selllevel;
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
            double high = data.AdjHigh;
            double low = data.AdjLow;

            if (!_ready)
            {
                _data.Add(MakeValue(high, low));
                if (_data.Count >= _period)
                {
                    _ready = true;
                    GenerateIndicator();
                }
            }
            else
            {
                _data.RemoveAt(0);
                _data.Add(MakeValue(high, low));
                GenerateIndicator();
            }

            _prevhigh = high;
            _prevlow = low;
        }

        private void GenerateIndicator()
        {
            // DeMax(i) is calculated:  If high(i) > high(i-1) , then DeMax(i) = high(i)-high(i-1), otherwise DeMax(i) = 0
            // DeMin(i) is calculated:  If low(i) < low(i-1), then DeMin(i) = low(i-1)-low(i), otherwise DeMin(i) = 0
            // The value of the DeMarker is calculated as:  Mark(i) = SMA(DeMax, N)/(SMA(DeMax, N)+SMA(DeMin, N))

            double d = 0;
            double d1 = 0;
            for (int i = 0; i < _data.Count; i++)
            {
                Value value = (Value)_data[i];
                d += value.getDeMax;
                d1 += value.getDeMax + value.getDeMin;
            }
            double d2 = (100D * d) / d1;
            if (d2 <= _buylevel)
                _direction = 1;
            else
                if (d2 >= _selllevel)
                    _direction = -1;
                else
                    _direction = 0;
        }

        private Value MakeValue(double high, double low)
        {
            double d2 = 0.0D;
            double d3 = 0.0D;
            if (high > _prevhigh)
                d2 = high - _prevhigh;
            if (low < _prevlow)
                d3 = _prevlow - low;
            Value value = new Value(this, d2, d3);
            return value;
        }

        internal class Value
        {
            private DeM _enclosingInstance;
            private double _demax;
            private double _demin;

            internal Value(DeM enclosingInstance, double demax, double demin)
            {
                _enclosingInstance = enclosingInstance;
                _demin = demin;
                _demax = demax;
            }

            virtual internal double getDeMax
            {
                get { return _demax; }
            }

            virtual internal double getDeMin
            {
                get { return _demin; }
            }

            public DeM Enclosing_Instance
            {
                get { return _enclosingInstance; }
            }
        }
    }
}