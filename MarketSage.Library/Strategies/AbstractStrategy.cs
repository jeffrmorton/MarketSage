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
    /// Abstract object of a basic strategy
    /// </summary>
    public abstract class AbstractStrategy
    {
        public CompoundIndicator _entryIndicator;
        public string _entryName;
        public CompoundIndicator _exitIndicator;
        public string _exitName;
        public bool _disposition = true;
        public Account _account;
        public Instrument _instrument;
        public ArrayList _pendingOrders = new ArrayList();
        public ArrayList _tickEvents = new ArrayList();

        /// <summary>
        /// Get/Set indicator
        /// </summary>
        /// <returns>IIndicator</returns>
        /// <param>IIndicator</param>
        public CompoundIndicator EntryIndicator
        {
            get { return _entryIndicator; }
            set { _entryIndicator = value; }
        }

        public CompoundIndicator ExitIndicator
        {
            get { return _exitIndicator; }
            set { _exitIndicator = value; }
        }

        /// <summary>
        /// Get/Set account
        /// </summary>
        /// <returns>Account</returns>
        /// <param>Account</param>
        public Account Account
        {
            get { return _account; }
            set { _account = value; }
        }

        /// <summary>
        /// Get/Set market
        /// </summary>
        /// <returns>IMarket</returns>
        /// <param>IMarket</param>
        public Instrument Market
        {
            get { return _instrument; }
            set { _instrument = value; }
        }

        /// <summary>
        /// Get/Set name
        /// </summary>
        /// <returns>string</returns>
        public string EntryIndicatorName
        {
            get { return _entryName; }
            set { _entryName = value; }
        }

        /// <summary>
        /// Get/Set name
        /// </summary>
        /// <returns>string</returns>
        public string ExitIndicatorName
        {
            get { return _exitName; }
            set { _exitName = value; }
        }

        public string Genome
        {
            get { return _entryName + "&" + _exitName; }
        }

        /// <summary>
        /// Returns pending orders
        /// </summary>
        /// <returns>ArrayList</returns>
        virtual public ArrayList GetPendingOrders()
        {
            return _pendingOrders;
        }

        /// <summary>
        /// Returns tick events
        /// </summary>
        /// <returns>ArrayList</returns>
        virtual public ArrayList GetTickEvents()
        {
            return _tickEvents;
        }

        /// <summary>
        /// Returns available cash from IAccount object
        /// </summary>
        /// <returns>double</returns>
        public virtual double GetCash()
        {
            return _account.Cash;
        }

        /// <summary>
        /// Returns available cash from IAccount object
        /// </summary>
        /// <returns>double</returns>
        public virtual double GetDaysHeld()
        {
            return _account.DaysHeld;
        }

        /// <summary>
        /// Returns total value of IAccount object
        /// </summary>
        /// <returns>double</returns>
        public virtual double GetValue()
        {
            return _account.AccountValue;
        }

        /// <summary>
        /// Returns profit and loss from IAccount object
        /// </summary>
        /// <returns>double</returns>
        public virtual double GetPnL()
        {
            //return _account.TransactionPnL;
            return _account.CumulativePnL;
        }

        public virtual double GetCumulativePnL()
        {
            return _account.CumulativePnL;
        }

        public virtual double GetInitialBalance()
        {
            return _account.InitialBalance;
        }

        /// <summary>
        /// Returns transaction count from IAccount object
        /// </summary>
        /// <returns>int</returns>
        public virtual int GetTransactionCount()
        {
            return _account.TransactionCount;
        }

        /// <summary>
        /// Returns list of transactions from IAccount object
        /// </summary>
        /// <returns>IList</returns>
        public virtual IList GetTransactions()
        {
            return _account.Transactions;
        }

        /// <summary>
        /// Returns commission from IAccount object
        /// </summary>
        /// <returns>double</returns>
        public virtual double GetCommission()
        {
            return _account.Commission;
        }

        /// <summary>
        /// Returns last transaction
        /// </summary>
        /// <returns>Transaction</returns>
        public virtual Transaction GetLastTransaction()
        {
            if (GetTransactions().Count < 1)
                return null;
            return (Transaction)GetTransactions()[GetTransactions().Count - 1];
        }

        /// <summary>
        /// Returns current open position from IAccount object
        /// </summary>
        /// <param name="instrument">string</param>
        /// <returns>double</returns>
        public double GetPosition(string instrument)
        {
            return _account.GetPosition(instrument);
        }

        /// <summary>
        /// Returns price from IAccount object
        /// </summary>
        /// <param name="instrument">string</param>
        /// <returns>double</returns>
        public double GetPrice(string instrument)
        {
            return _account.GetPrice(instrument);
        }

        /// <summary>
        /// Adds order at current market price for quantity
        /// </summary>
        /// <param name="instrument">string</param>
        /// <param name="quantity">double</param>
        public void AddQuantityOrder(string instrument, double quantity)
        {
            _pendingOrders.Add(new Order(instrument, true, quantity));
        }

        /// <summary>Adds order at current market price for cash amount</summary>
        /// <param name="instrument">Instrument</param>
        /// <param name="amount">Cash amount</param>
        public void AddCashOrder(string instrument, double amount)
        {
            _pendingOrders.Add(new Order(instrument, false, amount));
        }

        public virtual void Update()
        {
            // Check for valid tick data
            if ((Market.GetCurrent()) != null && Market.GetCurrent().AdjClose > 0)
            {
                _entryIndicator.AddData(Market.GetCurrent());
                _exitIndicator.AddData(Market.GetCurrent());
                if (_entryIndicator.IsReady() && _exitIndicator.IsReady())
                {
                    // Trade on concensus
                    if (GetPosition(_instrument.Name) == 0)
                    {
                        if (_account._isMarginAccount == false)
                        {
                            if (_entryIndicator.GetDirection() > 0 && _exitIndicator.GetDirection() >= 0)
                               // AddCashOrder(_instrument.Name, (GetCash() - _account.Commission) * .6180339887);
                            AddCashOrder(_instrument.Name, (GetCash() - _account.Commission));

                        }
                        else
                        {
                          //  if (_entryIndicator.GetDirection() > 0 && _exitIndicator.GetDirection() >= 0)
                          //      AddCashOrder(_instrument.Name, (GetCash() - _account.Commission) * .6180339887);
                          //  if (_entryIndicator.GetDirection() <= 0 && _exitIndicator.GetDirection() < 0)
                          //      AddCashOrder(_instrument.Name, (GetCash() - _account.Commission) * .6180339887);
                        }
                    }
                    if (GetPosition(_instrument.Name) > 0)
                    {
                        if (_exitIndicator.GetDirection() < 0 && _entryIndicator.GetDirection() <= 0)
                            AddQuantityOrder(_instrument.Name, -GetPosition(_instrument.Name));
                    }
                    if (GetPosition(_instrument.Name) < 0)
                    {
                       // if (_exitIndicator.GetDirection() < 0 && _entryIndicator.GetDirection() <= 0)
                       //     AddQuantityOrder(_instrument.Name, -GetPosition(_instrument.Name));
                    }
                }
            }
        }

        /*
        /// <summary>
        /// Updates position
        /// </summary>
        public virtual void Update()
        {
            if (GetPosition(_instrument.Name) == 0)
            {
                if (_entryIndicator != null)
                {
                    if ((Market.GetCurrent()) != null && Market.GetCurrent().Close > 0)
                    {
                        _entryIndicator.AddData(Market.GetCurrent());
                        if (_exitIndicatorStandard != null)
                        {
                            _exitIndicatorStandard.AddData(Market.GetCurrent());
                        }

                        if (_entryIndicator.IsReady())
                        {
                            if (_entryIndicator.IsBuy() && GetCash() > 0)
                            {
                                //double d2 = (GetCash() - _account.Commission) / GetPrice(_instrument);
                                double d2 = (GetCash() - _account.Commission) * .6180339887;
                                AddCashOrder(_instrument.Name, d2);
                            }
                        }
                    }
                }
                else if (_entryIndicatorStandard != null)
                {
                    if ((Market.GetCurrent()) != null && Market.GetCurrent().Close > 0)
                    {
                        _entryIndicatorStandard.AddData(Market.GetCurrent());
                        if (_exitIndicator != null)
                        {
                            _exitIndicator.AddData(Market.GetCurrent());
                        }

                        if (_entryIndicatorStandard.IsReady())
                        {
                            if (_entryIndicatorStandard.IsBuy() && GetCash() > 0)
                            {
                                //double d2 = (GetCash() - _account.Commission) / GetPrice(_instrument);
                                double d2 = (GetCash() - _account.Commission) * .6180339887;
                                AddCashOrder(_instrument.Name, d2);
                            }
                        }
                    }

                }
            }
            else
            {
                if (_entryIndicator != null)
                {
                    if ((Market.GetCurrent()) != null && Market.GetCurrent().Close > 0)
                    {
                        _entryIndicator.AddData(Market.GetCurrent());
                        if (_exitIndicatorStandard != null)
                        {
                            _exitIndicatorStandard.AddData(Market.GetCurrent());
                        }

                        if (_exitIndicatorStandard != null)
                        {
                            if (_exitIndicatorStandard.IsReady())
                            {
                                if (_exitIndicatorStandard.IsSell() && GetPosition(_instrument.Name) > 0)
                                {
                                    AddQuantityOrder(_instrument.Name, -GetPosition(_instrument.Name));
                                }
                            }
                        }
                        else
                        {
                            if (_entryIndicator.IsReady())
                            {
                                if (_entryIndicator.IsSell() && GetPosition(_instrument.Name) > 0)
                                {
                                    AddQuantityOrder(_instrument.Name, -GetPosition(_instrument.Name));
                                }
                            }
                        }
                    }
                }
                else if (_entryIndicatorStandard != null)
                {
                    if ((Market.GetCurrent()) != null && Market.GetCurrent().Close > 0)
                    {
                        _entryIndicatorStandard.AddData(Market.GetCurrent());
                        if (_exitIndicator != null)
                        {
                            _exitIndicator.AddData(Market.GetCurrent());
                        }

                        if (_exitIndicator != null)
                        {
                            if (_exitIndicator.IsReady())
                            {
                                if (_exitIndicator.IsSell() && GetPosition(_instrument.Name) > 0)
                                {
                                    AddQuantityOrder(_instrument.Name, -GetPosition(_instrument.Name));
                                }
                            }
                        }
                        else
                        {
                            if (_entryIndicatorStandard.IsReady())
                            {
                                if (_entryIndicatorStandard.IsSell() && GetPosition(_instrument.Name) > 0)
                                {
                                    AddQuantityOrder(_instrument.Name, -GetPosition(_instrument.Name));
                                }
                            }
                        }
                    }

                }
            }
        }
         */

        /// <summary>
        /// Initializes trader
        /// </summary>
        public abstract void Initialize();
    }
}