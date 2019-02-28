using System;
using System.Collections;

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
    /// CompoundIndicator object of IIndicator interface
    /// <summary>
    public class CompoundIndicator
    {
        private ArrayList _indicators;
        private string _name;
        private ArrayList _data;
        private bool _ready;
        private int _direction;
        private int _voteBuy;
        private int _voteHold;
        private int _voteSell;
        private int _period;
        string myName = "";
        string myDescription = "";
        string myAuthor = "";
        string myVersion = "";

        /// <summary>
        /// CompoundIndicator constructor
        /// </summary>
        /// <param name="list">ArrayList</param>
        public CompoundIndicator(ArrayList list, string name)
        {
            _indicators = list;
            _name = name;
            Initialize();
        }

        public CompoundIndicator(IIndicator indicator)
        {
            _indicators = new ArrayList();
            _indicators.Add(indicator);
            _name = indicator.GetName();
            Initialize();
        }

        public void Initialize()
        {
            _data = new ArrayList();
            _ready = false;
            _direction = 9;
            for (int x = 0; x < _indicators.Count; x++)
            {
                ((IIndicator)_indicators[x]).Initialize();
            }
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

        /// <summary>
        /// Returns name
        /// </summary>
        /// <returns>string</returns>
        public string GetName()
        {
            return _name;
        }

        /// <summary>
        /// Returns readyness of the indicator
        /// </summary>
        /// <returns>bool</returns>
        public bool IsReady()
        {
            return _ready;
        }

        /// <summary>
        /// Returns direction
        /// </summary>
        /// <returns>int</returns>
        public int GetDirection()
        {
            return _direction;
        }

        /// <summary>
        /// Indicates buy signal
        /// </summary>
        /// <returns>bool</returns>
        public bool IsBuy()
        {
            return _direction == 1;
        }

        /// <summary>
        /// Indicates hold signal
        /// </summary>
        /// <returns>bool</returns>
        public bool IsHold()
        {
            return _direction == 0;
        }

        /// <summary>
        /// Indicates sell signal
        /// </summary>
        /// <returns>bool</returns>
        public bool IsSell()
        {
            return _direction == -1;
        }

        public int GetCommitee()
        {
            return _indicators.Count;
        }

        public double Getconsensus()
        {
            double result = 0;
            if (GetDirection() == -1)
                result = (double)_voteSell / (double)GetCommitee();
            if (GetDirection() == 0)
                result = (double)_voteHold / (double)GetCommitee();
            if (GetDirection() == 1)
                result = (double)_voteBuy / (double)GetCommitee();
            return result;
        }

        /// <summary>
        /// Adds TechnicalData object to the indicator
        /// </summary>
        /// <param name="data">TechnicalData</param>
        public void AddData(MarketSage.Library.TechnicalData data)
        {
            if (!_ready)
            {
                int count = 0;
                for (int i = 0; i < _indicators.Count; i++)
                {
                    IIndicator indicator = (IIndicator)_indicators[i];
                    indicator.AddData(data);
                    if (indicator.IsReady())
                        count++;
                }
                if (count == _indicators.Count)
                {
                    _ready = true;
                    _voteBuy = 0;
                    _voteSell = 0;
                    _voteHold = 0;
                    for (int i = 0; i < _indicators.Count; i++)
                    {
                        IIndicator indicator = (IIndicator)_indicators[i];
                        if (indicator.IsBuy())
                            _voteBuy++;
                        if (indicator.IsSell())
                            _voteSell++;
                        if (indicator.IsHold())
                            _voteHold++;
                    }
                    if ((_voteBuy > _voteSell) && (_voteBuy > _voteHold))
                        _direction = 1;
                    else
                        if ((_voteSell > _voteBuy) && (_voteSell > _voteHold))
                            _direction = -1;
                        else
                            _direction = 0;
                }
            }
            else
            {
                _voteBuy = 0;
                _voteSell = 0;
                _voteHold = 0;
                for (int i = 0; i < _indicators.Count; i++)
                {
                    IIndicator indicator = (IIndicator)_indicators[i];
                    indicator.AddData(data);
                    if (indicator.IsBuy())
                        _voteBuy++;
                    if (indicator.IsSell())
                        _voteSell++;
                    if (indicator.IsHold())
                        _voteHold++;
                }
                if ((_voteBuy > _voteSell) && (_voteBuy > _voteHold))
                    _direction = 1;
                else
                    if ((_voteSell > _voteBuy) && (_voteSell > _voteHold))
                        _direction = -1;
                    else
                        _direction = 0;
            }
        }
    }
}
