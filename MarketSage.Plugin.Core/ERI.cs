using System;
using System.Collections;
using MarketSage.Library;

/* MarketSage
   Copyright © 2009 Jeffrey Morton

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
    /// <summary>Elder Ray Indicator</summary>
    public class ERI : IIndicator
    {

        private ArrayList _data;

        private string _name;

        private bool _ready;

        private int _signal;

        private int _period;

        string myName = "Elder Ray Indicator";

        string myDescription = "Developed by Dr Alexander Elder, the Elder-ray indicator measures buying and selling pressure in the market. The Elder-ray is often used as part of the Triple Screen trading system but may also be used on its own.";

        string myAuthor = "";

        string myVersion = "1.0.1";



        /// <summary>Creates new ERI object of IIndicator interface</summary>

        public ERI()
        {

            _period = 13;  // Default

            Initialize();

        }



        /// <summary>Creates new ERI object of IIndicator interface</summary>

        /// <param name="period">(From 1 to 100000) Number of period</param>

        public ERI(int period)
        {

            _period = period;

            Initialize();

        }



        public void Initialize()
        {

            _name = "ERI." + _period;

            _data = new ArrayList();

            _ready = false;

            _signal = 9;

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



        public int Period
        {

            get { return _period; }

            set { _period = value; }

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



                if (_data.Count > _period)
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

            double may = 0;

            double mat = 0;
            
            double smoothingfactor = (double)2 / (double)(_period + 1);


            for (int i = 0; i < _period; i++)
            {
                may = ((((TechnicalData)_data[i]).AdjClose - may) * smoothingfactor) + may;
            }
            for (int i = 1; i <= _period; i++)
            {
                mat = ((((TechnicalData)_data[i]).AdjClose - mat) * smoothingfactor) + mat;
            }

            double bullpowery = ((TechnicalData)_data[_period - 1]).AdjHigh - may;
            double bearpowery = ((TechnicalData)_data[_period - 1]).AdjLow - may;
            double bullpowert = ((TechnicalData)_data[_period]).AdjHigh - mat;
            double bearpowert = ((TechnicalData)_data[_period]).AdjLow - mat;

            if (mat < may)
            {
                if (bullpowert > 0 && bullpowert < bullpowery)
                    _signal = -1;
                else
                    _signal = 0;
            }
            else if (mat > may)
            {
                if (bearpowert < 0 && bearpowert > bearpowery)
                    _signal = 1;
                else
                    _signal = 0;
            }
            else
            {
                _signal = 0;
            }
        }
    }
}
