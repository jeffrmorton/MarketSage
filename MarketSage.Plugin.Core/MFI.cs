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
    /// <summary>The Money Flow Index (MFI) tracks the flow of money into or out of a market</summary>
    public class MFI : IIndicator
    {
        private ArrayList _data;
        private string _name;
        private bool _ready;
        private int _direction;
        private int _period;
        private double _buylevel;
        private double _selllevel;
        string myName = "Money Flow Index";
        string myDescription = "A momentum indicator that is used to determine the conviction in a current trend by analyzing the price and volume of a given security. The MFI is used as a measure of the strength of money going in and out of a security and can be used to predict a trend reversal. The MFI is range-bound between 0 and 100 and is interpreted in a similar fashion as the RSI.";
        string myAuthor = "";
        string myVersion = "1.0.0";

        /// <summary>Creates new MFI object of IIndicator interface</summary>
        public MFI()
        {
            _period = 14;
            _buylevel = 20;
            _selllevel = 80;
            Initialize();
        }

        /// <summary>Creates new MFI object of IIndicator interface</summary>
        /// <param name="period">Period</param>
        public MFI(int period)
        {
            _period = period;
            _buylevel = 20;
            _selllevel = 80;
            Initialize();
        }

        /// <summary>Creates new MFI object of IIndicator interface</summary>
        /// <param name="period">Period</param>
        /// <param name="buylevel">Buy level</param>
        /// <param name="selllevel">Sell Level</param>
        public MFI(int period, double buylevel, double selllevel)
        {
            _period = period;
            if (buylevel >= selllevel)
            {
                throw new System.ArgumentException("MFI : buy level must be < sell level");
            }
            _buylevel = buylevel;
            _selllevel = selllevel;
            Initialize();
        }

        public void Initialize()
        {
            _name = "MFI." + _period + "," + _buylevel + "," + _selllevel;
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
            double tp = data.AdjClose + data.AdjLow + data.AdjHigh;
            tp = tp / 3;
            double mf = tp * data.AdjVolume;

            if (!_ready)
            {
                _data.Add(new Value(this, mf, tp));
                if (_data.Count >= _period)
                {
                    _ready = true;
                    GenerateIndicator();
                }
            }
            else
            {
                _data.RemoveAt(0);
                _data.Add(new Value(this, mf, tp));
                GenerateIndicator();
            }
        }

        private void GenerateIndicator()
        {
            //  mfi = 100 - 100 / (1 + moneyratio)

            double upsum = 0;
            double downsum = 0;
            double last = -1;
            double current = 0;

            for (int i = 0; i < _data.Count; i++)
            {
                Value v = (Value)_data[i];
                current = v.TypicalPrice;

                if (last >= 0)
                {
                    if (current > last)
                    {
                        upsum += v.MoneyFlow;
                    }
                    else if (current < last)
                    {
                        downsum += v.MoneyFlow;
                    }
                }
                last = v.TypicalPrice;
            }

            if (downsum == 0)
            {
                _direction = -1;
            }

            double moneyratio = upsum / downsum;
            double mfi = 100 - 100 / (1 + moneyratio);

            if (mfi <= _buylevel)
            {
                _direction = 1;
            }
            else if (mfi >= _selllevel)
            {
                _direction = -1;
            }
            else
            {
                _direction = 0;
            }
        }

        internal class Value
        {
            private double _moneyFlow;
            private double _typicalPrice;
            private MFI _enclosingInstance;

            internal Value(MFI enclosingInstance, double mf, double tp)
            {
                _enclosingInstance = enclosingInstance;
                _moneyFlow = mf;
                _typicalPrice = tp;
            }

            virtual public double MoneyFlow
            {
                get { return _moneyFlow; }
            }

            virtual public double TypicalPrice
            {
                get { return _typicalPrice; }
            }

            public MFI Enclosing_Instance
            {
                get { return _enclosingInstance; }
            }
        }
    }
}