using System;

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
    public class Engine
    {
        #region Variables
        //private Market _market;
        public Instrument _instrument;
        private Trader _trader;
        //private Account _account;
        //private Portfolio _portfolio;
        private Observer _observer;
        private bool _isRunning;
        private DateTime _previousDate;
        private DateTime _currentDate;
        private bool _isIntitialized = false;
        private DateTime _dateTradeStart;
        private DateTime _dateTradeEnd;
        private DateTime _dateMarketStart;
        #endregion
        /*
        /// <summary>
        /// Engine constructor
        /// </summary>
        public Engine()
        {
            _trader = new Trader(5000);
            _dateTradeStart = DateTime.MinValue;
            _dateTradeEnd = DateTime.MaxValue;
            _dateMarketStart = DateTime.MinValue;
            _isRunning = false;
        }
         */

        /// <summary>
        /// Engine constructor
        /// </summary>
        public Engine(double initialCashBalance, double commission)
        {
            _trader = new Trader(initialCashBalance, commission);
            _dateTradeStart = DateTime.MinValue;
            _dateTradeEnd = DateTime.MaxValue;
            _dateMarketStart = DateTime.MinValue;
            _isRunning = false;
        }

        /// <summary>
        /// Engine constructor
        /// </summary>
        public Engine(double initialCashBalance, double commission, DateTime startDate, DateTime endDate)
        {
            _trader = new Trader(initialCashBalance, commission);
            _dateTradeStart = startDate;
            _dateTradeEnd = endDate;
            _dateMarketStart = startDate.Subtract(new TimeSpan(365 * 3, 0, 0, 0)); // Revise
            _isRunning = false;
        }

        /// <summary>
        /// Engine constructor
        /// </summary>
        public Engine(Trader trader, DateTime startDate, DateTime endDate)
        {
            _trader = trader;
            _dateTradeStart = startDate;
            _dateTradeEnd = endDate;
            _dateMarketStart = startDate.Subtract(new TimeSpan(365 * 3, 0, 0, 0)); // Revise
            _isRunning = false;
        }

        virtual public DateTime StartDate
        {
            get { return _dateTradeStart; }
            set { _dateTradeStart = value; }
        }

        virtual public DateTime EndDate
        {
            get { return _dateTradeEnd; }
            set { _dateTradeEnd = value; }
        }

        /// <summary>
        /// Get/Set market
        /// </summary>
        /// <returns>IMarket</returns>
        /// <param>IMarket</param>
        virtual public Instrument Market
        {
            get { return _instrument; }
            set { _instrument = value; }
        }

        /// <summary>
        /// Get/Set engine observer
        /// </summary>
        /// <returns>Observer</returns>
        /// <param>eObserver</param>
        virtual public Observer Observer
        {
            get { return _observer; }
            set { _observer = value; }
        }

        /// <summary>
        /// Get/Set trader
        /// </summary>
        /// <returns>Trader</returns>
        /// <param>Trader</param>
        virtual public Trader Trader
        {
            get { return _trader; }
            //set { _trader = value; }
        }

        /// <summary> Adds a new trader to the TraderContainer, it also creates a new IAccount
        /// for the trader, this must be use before starting the engine.
        /// 
        /// </summary>
        /// <param name="">trader
        /// </param>
        public virtual void register(AbstractStrategy trader)
        {
            _trader.AddTrader(trader);
        }

        /// <summary> 
        /// remove traders from TraderContainer
        /// </summary>
        public virtual void removeTraders()
        {
            _trader.Clear();
        }

        /// <summary>
        /// Run engine
        /// </summary>
        public virtual void Run()
        {
            while (_instrument.HasNext() && _isRunning)
            {
                _instrument.Next();
                if (_instrument.GetCurrent().Date >= _dateMarketStart)
                {
                    if (_observer != null)
                    {
                        _currentDate = _instrument.Date;
                        CalanderEvent evt = GetEvent(_currentDate);
                        _observer.onUpdate(evt);
                    }
                    if (_instrument.GetCurrent().Date >= _dateTradeStart && _instrument.GetCurrent().Date <= _dateTradeEnd)
                        _trader.ProcessOrders();
                    else
                        _trader.ClearOrders();
                    if (_instrument.NextToLast())
                        _trader.ClosePosition();
                    else
                        _trader.UpdateTraders();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Iterate()
        {
            _instrument.Next();
            if (_instrument.GetCurrent().Date >= _dateMarketStart)
            {
                if (_observer != null)
                {
                    _currentDate = _instrument.Date;
                    CalanderEvent evt = GetEvent(_currentDate);
                    _observer.onUpdate(evt);
                }
                if (_instrument.GetCurrent().Date >= _dateTradeStart && _instrument.GetCurrent().Date <= _dateTradeEnd)
                    _trader.ProcessOrders();
                else
                    _trader.ClearOrders();
                if (_instrument.NextToLast())
                    _trader.ClosePosition();
                else
                    _trader.UpdateTraders();
            }
        }

        /// <summary>
        /// Start engine
        /// </summary>
        public virtual void Start()
        {
            _instrument.Initialize();
            _trader.Market = _instrument;
            _trader.Initialize();
            if (_observer != null)
            {
                _observer.Clear();
                _observer.setMarket(_instrument);
                _observer.setTraderContainer(_trader);
                _observer.onStarted();
            }
            _isRunning = true;
        }

        /// <summary>
        /// Stop engine
        /// </summary>
        public virtual void Stop()
        {
            if (_observer != null)
            {
                _currentDate = _instrument.Date;
                CalanderEvent evt = GetEvent(_currentDate);
                _observer.onUpdate(evt);
                _observer.onStopped();
            }
            _isRunning = false;
        }

        public virtual void Tally(int count)
        {
            if (_observer != null)
            {
                _observer.UpdateTotal(count);
            }
        }

        public virtual CalanderEvent GetEvent(DateTime date)
        {
            CalanderEvent evt = null;
            if (_isIntitialized == false)
            {
                evt = CalanderEvent.DAY_EVENT;
                _isIntitialized = true;
            }
            else
            {
                if (date.Year != _previousDate.Year)
                {
                    evt = CalanderEvent.YEAR_EVENT;
                }
                else if (date.Month != _previousDate.Month)
                {
                    evt = CalanderEvent.MONTH_EVENT;
                }
                else
                {
                    evt = CalanderEvent.DAY_EVENT;
                }
            }
            _previousDate = date;
            return evt;
        }
    }
}