using System;
using System.Data;
using System.Threading;

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
    public class Portfolio
    {
        private string _portfolioFile;
        private DataSet _portfolioDataSet;
        private string _indexDJI;
        public bool _dispositionDJI;
        private string _indexIXIC;
        public bool _dispositionIXIC;
        private string _indexGSPC;
        public bool _dispositionGSPC;
        private QuoteServices _quoteService;
        public double _initial;  // change this

        /// <summary>
        /// Portfolio constructor
        /// </summary>
        /// <param name="filename">string</param>
        /// <param name="directoryLogs">string</param>
        public Portfolio(double initialbalance, string filename, string directoryLogs)
        {
            _initial = initialbalance;
            _portfolioFile = filename;
            _quoteService = new QuoteServices(directoryLogs);

            _portfolioDataSet = new DataSet("portfolio");

            DataTable tblHistory = new DataTable("history");
            tblHistory.Columns.Add("inception", typeof(DateTime));
            _portfolioDataSet.Tables.Add(tblHistory);

            DataTable tblAccount = new DataTable("account");
            tblAccount.Columns.Add("initial_balance", typeof(double));
            tblAccount.Columns.Add("current_balance", typeof(double));
            _portfolioDataSet.Tables.Add(tblAccount);

            DataTable tblPositions = new DataTable("position");
            tblPositions.Columns.Add("symbol", typeof(string));
            tblPositions.Columns.Add("name", typeof(string));
            tblPositions.Columns.Add("shares", typeof(double));
            tblPositions.Columns.Add("basis", typeof(double));
            tblPositions.Columns.Add("price", typeof(double));
            tblPositions.Columns.Add("gain", typeof(double));
            tblPositions.Columns.Add("change", typeof(double));
            tblPositions.Columns.Add("volume", typeof(int));
            tblPositions.Columns.Add("date", typeof(DateTime));
            tblPositions.Columns.Add("return", typeof(double));
            _portfolioDataSet.Tables.Add(tblPositions);

            DataTable tblTransactions = new DataTable("transaction");
            tblTransactions.Columns.Add("date", typeof(DateTime));
            tblTransactions.Columns.Add("position", typeof(string));
            tblTransactions.Columns.Add("trade", typeof(string));
            tblTransactions.Columns.Add("symbol", typeof(string));
            tblTransactions.Columns.Add("shares", typeof(double));
            tblTransactions.Columns.Add("price", typeof(double));
            tblTransactions.Columns.Add("commission", typeof(double));
            _portfolioDataSet.Tables.Add(tblTransactions);

            DataTable tblPerformance = new DataTable("performance");
            tblPerformance.Columns.Add("opened_date", typeof(DateTime));
            tblPerformance.Columns.Add("opened_price", typeof(double));
            tblPerformance.Columns.Add("closed_date", typeof(DateTime));
            tblPerformance.Columns.Add("closed_price", typeof(double));
            tblPerformance.Columns.Add("trade", typeof(string));
            tblPerformance.Columns.Add("symbol", typeof(string));
            tblPerformance.Columns.Add("shares", typeof(double));
            tblPerformance.Columns.Add("overhead", typeof(double));
            tblPerformance.Columns.Add("profit", typeof(double));
            tblPerformance.Columns.Add("percentage", typeof(double));
            tblPerformance.Columns.Add("length", typeof(int));
            _portfolioDataSet.Tables.Add(tblPerformance);

            try
            {
                _portfolioDataSet.ReadXml(filename, System.Data.XmlReadMode.ReadSchema);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                DataRow newrow1 = _portfolioDataSet.Tables["history"].NewRow();
                newrow1["inception"] = DateTime.Now;
                _portfolioDataSet.Tables["history"].Rows.Add(newrow1);
                DataRow newrow2 = _portfolioDataSet.Tables["account"].NewRow();
                newrow2["initial_balance"] = _initial;
                newrow2["current_balance"] = _initial;
                _portfolioDataSet.Tables["account"].Rows.Add(newrow2);
                _portfolioDataSet.AcceptChanges();
                Save();
            }
            Update();
        }

        /// <summary>
        /// Add position
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="name"></param>
        /// <param name="shares"></param>
        /// <param name="basis"></param>
        public void AddPosition(string symbol, string name, double shares, double price, double commission)
        {
            double basis = (shares * price) + commission;
            double balance = Double.Parse(_portfolioDataSet.Tables["account"].Rows[0].ItemArray[1].ToString()) - basis;

            bool flag = false;
            string[] positions = GetArrayOfInstruments();
            for (int index = 0; index < positions.Length; index++)
            {
                if (positions[index] == symbol)
                {
                    DataRow existingrow = _portfolioDataSet.Tables["position"].Rows[index];
                    existingrow["shares"] = double.Parse(_portfolioDataSet.Tables["position"].Rows[index].ItemArray[2].ToString()) + shares;
                    existingrow["basis"] = double.Parse(_portfolioDataSet.Tables["position"].Rows[index].ItemArray[3].ToString()) + basis;
                    flag = true;
                }
            }
            if (flag == false)
            {
                DataRow newrow = _portfolioDataSet.Tables["position"].NewRow();
                newrow["symbol"] = symbol;
                newrow["name"] = name;
                newrow["shares"] = shares;
                newrow["basis"] = basis;
                newrow["price"] = price;
                newrow["gain"] = 0;
                newrow["change"] = 0;
                newrow["date"] = DateTime.MinValue;
                newrow["volume"] = 0;
                newrow["return"] = 0;
                _portfolioDataSet.Tables["position"].Rows.Add(newrow);
            }
            DataRow newrow2 = _portfolioDataSet.Tables["transaction"].NewRow();
            newrow2["date"] = DateTime.Now;
            newrow2["position"] = "OPEN";
            newrow2["trade"] = "LONG";
            newrow2["symbol"] = symbol;
            newrow2["shares"] = shares;
            newrow2["price"] = price;
            newrow2["commission"] = commission;
            _portfolioDataSet.Tables["transaction"].Rows.Add(newrow2);
            DataRow existingrow2 = _portfolioDataSet.Tables["account"].Rows[0];
            existingrow2["current_balance"] = balance;
            _portfolioDataSet.AcceptChanges();
            Save();
            Update();
        }

        /// <summary>
        /// Remove position
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="price"></param>
        /// <param name="commission"></param>
        public void RemovePosition(string symbol, double price, double commission)
        {
            string[] positions = GetArrayOfInstruments();
            for (int index = 0; index < positions.Length; index++)
            {
                if (positions[index] == symbol)
                {
                    double shares = double.Parse(_portfolioDataSet.Tables["position"].Rows[index].ItemArray[2].ToString());
                    double basis = (shares * price) - commission;
                    double balance = Double.Parse(_portfolioDataSet.Tables["account"].Rows[0].ItemArray[1].ToString()) + basis;
                    _portfolioDataSet.Tables["position"].Rows[index].Delete();
                    DataRow newrow = _portfolioDataSet.Tables["transaction"].NewRow();
                    newrow["date"] = DateTime.Now;
                    newrow["position"] = "CLOSED";
                    newrow["trade"] = "SHORT";
                    newrow["symbol"] = symbol;
                    newrow["shares"] = shares;
                    newrow["price"] = price;
                    newrow["commission"] = commission;
                    _portfolioDataSet.Tables["transaction"].Rows.Add(newrow);
                    DataRow existingrow = _portfolioDataSet.Tables["account"].Rows[0];
                    existingrow["current_balance"] = balance;
                    _portfolioDataSet.AcceptChanges();
                    Save();
                    Update();
                }
            }
        }

        /// <summary>
        /// Get account value
        /// </summary>
        /// <returns></returns>
        public double GetAccountValue()
        {
            double cashValue = Double.Parse(_portfolioDataSet.Tables["account"].Rows[0].ItemArray[1].ToString());
            return cashValue;
        }

        /// <summary>
        /// Get total value
        /// </summary>
        /// <returns></returns>
        public double GetTotalValue()
        {
            double cashValue = Double.Parse(_portfolioDataSet.Tables["account"].Rows[0].ItemArray[1].ToString());
            double positionValue = 0;
            foreach (DataRow row in _portfolioDataSet.Tables["position"].Rows)
            {
                positionValue += (Double.Parse(row["shares"].ToString()) * Double.Parse(row["price"].ToString()));
            }
            return positionValue + cashValue;
        }

        /// <summary>
        /// Get profit and loss
        /// </summary>
        /// <returns></returns>
        public double GetPnL()
        {
            double initialValue = Double.Parse(_portfolioDataSet.Tables["account"].Rows[0].ItemArray[0].ToString());
            double cashValue = Double.Parse(_portfolioDataSet.Tables["account"].Rows[0].ItemArray[1].ToString());
            double positionValue = 0;
            foreach (DataRow row in _portfolioDataSet.Tables["position"].Rows)
            {
                positionValue += (Double.Parse(row["shares"].ToString()) * Double.Parse(row["price"].ToString()));
            }
            return (positionValue + cashValue) - initialValue;
        }

        /// <summary>
        /// Get profit and loss percentage
        /// </summary>
        /// <returns></returns>
        public double GetPnLPercentage()
        {
            double initialValue = Double.Parse(_portfolioDataSet.Tables["account"].Rows[0].ItemArray[0].ToString());
            double cashValue = Double.Parse(_portfolioDataSet.Tables["account"].Rows[0].ItemArray[1].ToString());
            double positionValue = 0;
            foreach (DataRow row in _portfolioDataSet.Tables["position"].Rows)
            {
                positionValue += (Double.Parse(row["shares"].ToString()) * Double.Parse(row["price"].ToString()));
            }
            return (((positionValue + cashValue) - initialValue) / initialValue) * 100;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDJI()
        {
            return _indexDJI;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetIXIC()
        {
            return _indexIXIC;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetGSPC()
        {
            return _indexGSPC;
        }

        /// <summary>
        /// Get array of instruments
        /// </summary>
        /// <returns></returns>
        public string[] GetArrayOfInstruments()
        {
            string[] toReturn = new string[_portfolioDataSet.Tables["position"].Rows.Count];
            int i = 0;
            foreach (DataRow row in _portfolioDataSet.Tables["position"].Rows)
            {
                toReturn[i++] = row["symbol"].ToString();
            }
            return toReturn;
        }

        /// <summary>
        /// Get symbol by row
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetSymbolByRow(int index)
        {
            return _portfolioDataSet.Tables["position"].Rows[index]["symbol"].ToString();
        }

        /// <summary>
        /// Get name by row
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetNameByRow(int index)
        {
            return _portfolioDataSet.Tables["position"].Rows[index]["name"].ToString();
        }

        /// <summary>
        /// Get DataSet
        /// </summary>
        /// <returns></returns>
        public DataSet GetDataSet()
        {
            return _portfolioDataSet;
        }

        /// <summary>
        /// Save
        /// </summary>
        public void Save()
        {
            _portfolioDataSet.WriteXml(_portfolioFile);
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            try
            {
                foreach (DataRow row in _portfolioDataSet.Tables["position"].Rows)
                {
                    string[] stockInfo = _quoteService.GetStockInfoArray(row["symbol"].ToString());
                    row["price"] = double.Parse(stockInfo[1]);
                    row["date"] = DateTime.Parse(stockInfo[2] + " " + stockInfo[3]);
                    row["change"] = double.Parse(stockInfo[4]);
                    row["volume"] = int.Parse(stockInfo[8]);
                    double basis = double.Parse(row["basis"].ToString());
                    double shares = double.Parse(row["shares"].ToString());
                    double price = double.Parse(row["price"].ToString());
                    double change = double.Parse(row["change"].ToString());
                    double gained = (shares * price) - basis;
                    row["gain"] = gained;
                    double returned = (100 / basis) * gained;
                    row["return"] = returned;
                }
                string[] stockInfoArray;
                // Update indexes
                stockInfoArray = _quoteService.GetStockInfoArray("^DJI");
                _indexDJI = stockInfoArray[1] + " " + stockInfoArray[4];
                if (double.Parse(stockInfoArray[4].ToString()) >= 0)
                    _dispositionDJI = true;
                else
                    _dispositionDJI = false;
                stockInfoArray = _quoteService.GetStockInfoArray("^IXIC");
                _indexIXIC = stockInfoArray[1] + " " + stockInfoArray[4];
                if (double.Parse(stockInfoArray[4].ToString()) >= 0)
                    _dispositionIXIC = true;
                else
                    _dispositionIXIC = false;
                stockInfoArray = _quoteService.GetStockInfoArray("^GSPC");
                _indexGSPC = stockInfoArray[1] + " " + stockInfoArray[4];
                if (double.Parse(stockInfoArray[4].ToString()) >= 0)
                    _dispositionGSPC = true;
                else
                    _dispositionGSPC = false;
                _portfolioDataSet.AcceptChanges();
                _portfolioDataSet.WriteXml(_portfolioFile);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
