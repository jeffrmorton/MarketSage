using System;
using System.Collections;
using System.Data;

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
    public class Account
    {
        private double _currentCashBalance;
        private double _initialCashBalance;
        private double _position;
        private double _commission; // Scottrade $7.00 market
        private double _daysToClear = 3;  // Average 2-4?
        private int _daysHeld;
        private int _daysInPlay;
        private ArrayList _transactions;
        public bool _inPlay = false;
        private DateTime _dateBought;
        private double _basis;
        private double _profitInPlay;
        private int _transactionCycles;
        private double _stopLossPrice;
        private Instrument _instrument;
        private DataTable _dataset;
        private string _fileAccount;

        public bool _isMarginAccount = false;
        private double _marginMinimumPrincipal = 2000;  // $2000.00 minimum principal
        private int _marginLeverage = 2;  // 2:1
        //private double _marginInitialRequirement = .5;  // 50% initial margin requirement
        //private double _margineMaintenanceRequirement = .35;  //  35% maintenance requirement
        private double _marginMinimumEquityValue = 5.00;  // $5.00 minimum equity price
        //private double _marginInterestRate = .05;  // 5% interest rate
        private bool _marginDirection = false;  //  short
        private double _marginBalance = 0;

        /*
        public Account()
        {
            _initialCashBalance = 5000.00;
            _currentCashBalance = _initialCashBalance;
            _daysInPlay = 0;
            _profitInPlay = 0;
            _transactionCycles = 0;
            _transactions = new ArrayList();
        }

        public Account(double balance)
        {
            _initialCashBalance = balance;
            _currentCashBalance = _initialCashBalance;
            _daysInPlay = 0;
            _profitInPlay = 0;
            _transactionCycles = 0;
            _transactions = new ArrayList();
        }
         */

        public Account(double balance, double commission)
        {
            _initialCashBalance = balance;
            _currentCashBalance = _initialCashBalance;
            _commission = commission;
            _daysInPlay = 0;
            _profitInPlay = 0;
            _transactionCycles = 0;
            _transactions = new ArrayList();
        }

        public Account(double balance, double commission, string filename)
        {

            _fileAccount = filename;

            _dataset = new DataTable("account");
            _dataset.Columns.Add("inception", typeof(DateTime));
            _dataset.Columns.Add("initial_balance", typeof(double));
            _dataset.Columns.Add("current_balance", typeof(double));

            try
            {
                _dataset.ReadXml(filename);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                _initialCashBalance = balance;
                _currentCashBalance = _initialCashBalance;
                _commission = commission;
                _daysInPlay = 0;
                _profitInPlay = 0;
                _transactionCycles = 0;
                _transactions = new ArrayList();

                DataRow newrow = _dataset.NewRow();

                newrow["inception"] = DateTime.Now;
                newrow["initial_balance"] = _initialCashBalance;
                newrow["current_balance"] = _currentCashBalance;
                _dataset.Rows.Add(newrow);
            }
        }

        public void Save()
        {
            _dataset.WriteXml(_fileAccount);
        }

        virtual public double Cash
        {
            get { return _currentCashBalance; }
            set { _currentCashBalance = value; }
        }

        public double TradingBalance(Instrument instrument)
        {
            if (_isMarginAccount == false)
                return _currentCashBalance;
            else
            {
                if ((_currentCashBalance >= _marginMinimumPrincipal) && (instrument.AdjClose >= _marginMinimumEquityValue))
                    return(_currentCashBalance * _marginLeverage);
                else
                    return _currentCashBalance;
            }
        }

        virtual public double Commission
        {
            get { return _commission; }
            set { _commission = value; }
        }

        virtual public double Basis
        {
            get { return _basis; }
        }

        virtual public int DaysInPlay
        {
            get { return _daysHeld; }
        }

        virtual public double AccountValue
        {
            get
            {
                if (_isMarginAccount == false)
                    return _currentCashBalance + (_position * _instrument.AdjClose);
                else
                {
                    if (_marginDirection == false)  // short
                    {
                        return _currentCashBalance + (_position * _instrument.AdjClose) - _marginBalance;
                    }
                    else
                    {
                        return _currentCashBalance + (_position * _instrument.AdjClose);
                    }
                }
            }
        }

        public double PositionValue
        {
            get
            {
                return _position * _instrument.AdjClose;
            }
        }

        virtual public double TransactionPnL
        {
            get { return _profitInPlay; }
        }

        virtual public double CumulativePnL
        {
            get { return AccountValue - _initialCashBalance; }
        }

        virtual public double InitialBalance
        {
            get { return _initialCashBalance; }
        }

        virtual public int TransactionCount
        {
            get { return _transactionCycles; }
        }

        virtual public double DaysHeld
        {
            get { return _daysInPlay; }
        }

        virtual public IList Transactions
        {
            get { return _transactions; }
        }

        virtual public double StopLossPrice
        {
            get { return _stopLossPrice; }
        }

        virtual internal Instrument Market
        {
            get { return _instrument; }
            set { _instrument = value; }
        }

        public virtual double GetPosition(string instrument)
        {
            return _position;
        }

        public virtual double GetPrice(string instrument)
        {
            double temp = _instrument.AdjHigh + _instrument.AdjLow;
            double temp2 = temp / 2;
            return temp2;  // For simulation only
            //return (_instrument.AdjClose);
        }

        /// <summary></summary>
        /// <param name="instrument">Instrument</param>
        /// <param name="quantity">number of shares to trade</param>
        /// <returns> total price of the transaction</returns>
        internal virtual double NewQuantityTransaction(string instrument, double quantity)
        {
            if (_isMarginAccount == false)
            {
                double total = 0;
                if (_transactions.Count > 0)
                {
                    TimeSpan timespan = new TimeSpan();
                    timespan = _instrument.Date.Subtract(((Transaction)_transactions[_transactions.Count - 1]).Date);

                    if (timespan.Days < _daysToClear)
                    {
                        return 0;
                    }
                }
                if (quantity > 0)  // buying
                {
                    total = -(quantity * GetPrice(instrument));

                    if (System.Math.Abs(total) < 0.01)
                    {
                        return 0;
                    }
                    else
                    {
                        if ((_currentCashBalance + total) > 0)
                        {
                            _currentCashBalance += total;
                            _currentCashBalance -= _commission;
                            _position += quantity;
                            _inPlay = true;
                            _dateBought = _instrument.Date;
                            _basis = System.Math.Abs(total);
                            _transactions.Add(new Transaction(_instrument.Date, instrument, quantity, total));
                            _stopLossPrice = (GetPrice(instrument) / 100) * 90;
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
                if (quantity < 0) // selling
                {
                    total = -quantity * GetPrice(instrument);
                    if (_position >= -quantity && _transactions.Count > 0)
                    {
                        _currentCashBalance += total;
                        _currentCashBalance -= _commission;
                        _position += quantity;
                        TimeSpan timespan2 = new TimeSpan();
                        if (_inPlay == true)
                        {
                            timespan2 = _instrument.Date.Subtract(((Transaction)_transactions[_transactions.Count - 1]).Date);
                            _daysInPlay += timespan2.Days;
                            _daysHeld = timespan2.Days;
                            _profitInPlay += AccountValue - _basis;
                            _transactionCycles++;
                            _inPlay = false;
                        }
                        _transactions.Add(new Transaction(_instrument.Date, instrument, quantity, total));
                        return 1;
                    }
                }
                return 0;
                /*
                if (System.Math.Abs(total) < 0.01)
                {
                    return 0;
                }
                else if (total > 0)
                {
                    if (_currentBalance >= total)
                    {
                        _currentBalance -= total;
                        _currentBalance -= _commission;
                        _position += quantity;
                        _inPlay = true;
                        _dateBought = _instrument.Date;
                        _basis = total;
                        _transactions.Add(new Transaction(_instrument.Date, instrument, quantity, total));
                        _stopLossPrice = (GetPrice(instrument) / 100) * 90;
                        return total;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    if (_position >= -quantity && _transactions.Count > 0)
                    {
                        _currentBalance -= total;
                        _currentBalance -= _commission;
                        _position += quantity;
                        TimeSpan timespan2 = new TimeSpan();
                        if (_inPlay == true)
                        {
                            timespan2 = _instrument.Date.Subtract(((Transaction)_transactions[_transactions.Count - 1]).Date);
                            _daysInPlay += timespan2.Days;
                            _daysHeld = timespan2.Days;
                            _profitInPlay += AccountValue - _basis;
                            _transactionCycles++;
                            _inPlay = false;
                        }
                        _transactions.Add(new Transaction(_instrument.Date, instrument, quantity, total));
                        return -total;
                    }
                    else
                    {
                        return 0;
                    }
                }
                 */
            }
            else
            {
                return 0;  // todo
            }
        }

        internal virtual double NewCashTransaction(string instrument, double amount)
        {
            if (_isMarginAccount == false)
            {
                double q = Math.Floor(amount / GetPrice(instrument));
                return NewQuantityTransaction(instrument, q);
            }
            else
            {
                return 0;  //todo
            }
        }

        internal virtual void init()
        {
            _currentCashBalance = _initialCashBalance;
            _transactions.Clear();
            _position = 0;
            _daysInPlay = 0;
            _profitInPlay = 0;
            _transactionCycles = 0;
        }
    }
}