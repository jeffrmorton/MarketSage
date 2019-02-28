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
    /// Trader object
    /// </summary>
    public class Trader
    {
        public Account _account;
        public ArrayList _agents = new ArrayList();
        private IDictionary _accounts = new Hashtable();
        private Instrument _market;
        private Portfolio _portfolio;

        /// <summary> 
        /// Trader constructor.
        /// </summary>
        internal Trader(double cash, double commission)
        {
            _account = new Account(cash, commission);
        }

        /// <summary> 
        /// Trader constructor.
        /// </summary>
        internal Trader(Portfolio portfolio, double cash, double commission)
        {
            _portfolio = portfolio;
            _account = new Account(cash, commission);
        }

        /// <summary>
        /// Get traders
        /// </summary>
        /// <returns>ArrayList</returns>
        virtual public ArrayList Agents
        {
            get { return _agents; }
        }

        /// <summary>
        /// 
        /// </summary>
        virtual public Instrument Market
        {
            set { _market = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Clear()
        {
            _agents.Clear();
            _accounts.Clear();
        }

        public Account Account
        {
            get { return _account; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trader"></param>
        internal virtual void AddTrader(AbstractStrategy strategy)
        {
            Account account = _account;
            _agents.Add(strategy);
            strategy.Account = account;
            _accounts[strategy] = account;
        }

        /// <summary>
        /// 
        /// </summary>
        internal virtual void UpdateTraders()
        {
            for (System.Collections.IEnumerator it = _agents.GetEnumerator(); it.MoveNext(); )
            {
                ((AbstractStrategy)it.Current).Update();
            }
        }

        internal virtual void ClosePosition()
        {
            for (System.Collections.IEnumerator it = _agents.GetEnumerator(); it.MoveNext(); )
            {
                AbstractStrategy trader = (AbstractStrategy)it.Current;
                if (trader.GetPosition(trader._instrument.Name) != 0)
                    trader.AddQuantityOrder(trader._instrument.Name, -trader.GetPosition(trader._instrument.Name));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal virtual void ProcessOrders()
        {
            for (System.Collections.IEnumerator it = _agents.GetEnumerator(); it.MoveNext(); )
            {
                AbstractStrategy trader = (AbstractStrategy)it.Current;
                if (!(trader.GetPendingOrders().Count == 0))
                {
                    Account account = (Account)_accounts[trader];
                    for (System.Collections.IEnumerator itord = trader.GetPendingOrders().GetEnumerator(); itord.MoveNext(); )
                    {
                        Order order = (Order)itord.Current;
                        ProcessOrder(trader, account, order);
                    }
                    trader.GetPendingOrders().Clear();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal virtual void ClearOrders()
        {
            for (System.Collections.IEnumerator it = _agents.GetEnumerator(); it.MoveNext(); )
            {
                AbstractStrategy trader = (AbstractStrategy)it.Current;
                if (!(trader.GetPendingOrders().Count == 0))
                {
                    trader.GetPendingOrders().Clear();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trader"></param>
        /// <param name="account"></param>
        /// <param name="order"></param>
        private void ProcessOrder(AbstractStrategy trader, Account account, Order order)
        {
            double result = 0;
            if (order.Type)
                result = account.NewQuantityTransaction(order.Instrument, order.Value);
            else
                result = account.NewCashTransaction(order.Instrument, order.Value);
            trader.GetTickEvents().Clear();
            if (result == 0)
            {
                trader.GetTickEvents().Add(new TransactionEvent(TransactionEvent.ORDER_REJECT));
            }
            else
            {
                trader.GetTickEvents().Add(new TransactionEvent(TransactionEvent.ORDER_EXEC));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Initialize()
        {
            for (System.Collections.IEnumerator it = _agents.GetEnumerator(); it.MoveNext(); )
            {
                AbstractStrategy trader = (AbstractStrategy)it.Current;
                Account account = (Account)_accounts[trader];
                trader.Market = _market;
                account.Market = _market;
                account.init();
                trader.Initialize();
            }
        }
    }
}