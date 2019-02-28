using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using OTFeed_NET;
using GenericParsing;


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
    public class DataSource
    {
        private string _pathMarkets;
        private string _pathWork;
        private string _pathLogs;
        private Hashtable _marketDateExceptions = new Hashtable();
        private InternetConnection _internetConnection = new InternetConnection();
        private Hashtable _lastDataDate = new Hashtable();
        private const string DeveloperID = "jeffrmorton";
        private const string UserID2 = "jeffrmorton";
        private const string Password2 = "testtest";
        private OTClient client = new OTClient();
        private string _index = "";
        public string _log = "";

        public DataSource()
        {
            _pathMarkets = "";
            _pathWork = "";
            client.onLogin += new OTLoginEvent(onLogin);
            client.onStatusChanged += new OTStatusChangedEvent(onStatusChange);
            client.onRestoreConnection += new OTRestoreConnectionEvent(onRestoreConnection);
            client.onError += new OTErrorEvent(onError);
            client.onMessage += new OTMessageEvent(onMessage);
            client.onRealtimeMMQuote += new OTMMQuoteEvent(onMMQuote);
            client.onRealtimeTrade += new OTTradeEvent(onTrade);
            client.onRealtimeBBO += new OTBBOEvent(onBBO);
            client.onRealtimeQuote += new OTQuoteEvent(onQuote);
            client.onBookCancel += new OTBookCancelEvent(onBookCancel);
            client.onBookChange += new OTBookChangeEvent(onBookChange);
            client.onBookDelete += new OTBookDeleteEvent(onBookDelete);
            client.onBookExecute += new OTBookExecuteEvent(onBookExecute);
            client.onBookOrder += new OTBookOrderEvent(onBookOrder);
            client.onBookPriceLevel += new OTBookPriceLevelEvent(onBookPriceLevel);
            client.onBookPurge += new OTBookPurgeEvent(onBookPurge);
            client.onBookReplace += new OTBookReplaceEvent(onBookReplace);
            client.onHistoricalMMQuote += new OTMMQuoteEvent(onMMQuote);
            client.onHistoricalTrade += new OTTradeEvent(onTrade);
            client.onHistoricalBBO += new OTBBOEvent(onBBO);
            client.onHistoricalQuote += new OTQuoteEvent(onQuote);
            client.onHistoricalOHLC += new OTOHLCEvent(onHistoricalOHLC);
            client.onListExchanges += new OTListExchangesEvent(onListExchanges);
            client.onListSymbols += new OTListSymbolsEvent(onListSymbols);
        }
        public DataSource(string pathMarkets, string pathWork)
        {
            _pathMarkets = pathMarkets;
            _pathWork = pathWork;
            client.onLogin += new OTLoginEvent(onLogin);
            client.onStatusChanged += new OTStatusChangedEvent(onStatusChange);
            client.onRestoreConnection += new OTRestoreConnectionEvent(onRestoreConnection);
            client.onError += new OTErrorEvent(onError);
            client.onMessage += new OTMessageEvent(onMessage);
            client.onRealtimeMMQuote += new OTMMQuoteEvent(onMMQuote);
            client.onRealtimeTrade += new OTTradeEvent(onTrade);
            client.onRealtimeBBO += new OTBBOEvent(onBBO);
            client.onRealtimeQuote += new OTQuoteEvent(onQuote);
            client.onBookCancel += new OTBookCancelEvent(onBookCancel);
            client.onBookChange += new OTBookChangeEvent(onBookChange);
            client.onBookDelete += new OTBookDeleteEvent(onBookDelete);
            client.onBookExecute += new OTBookExecuteEvent(onBookExecute);
            client.onBookOrder += new OTBookOrderEvent(onBookOrder);
            client.onBookPriceLevel += new OTBookPriceLevelEvent(onBookPriceLevel);
            client.onBookPurge += new OTBookPurgeEvent(onBookPurge);
            client.onBookReplace += new OTBookReplaceEvent(onBookReplace);
            client.onHistoricalMMQuote += new OTMMQuoteEvent(onMMQuote);
            client.onHistoricalTrade += new OTTradeEvent(onTrade);
            client.onHistoricalBBO += new OTBBOEvent(onBBO);
            client.onHistoricalQuote += new OTQuoteEvent(onQuote);
            client.onHistoricalOHLC += new OTOHLCEvent(onHistoricalOHLC);
            client.onListExchanges += new OTListExchangesEvent(onListExchanges);
            client.onListSymbols += new OTListSymbolsEvent(onListSymbols);
        }
        public DataSource(string pathMarkets, string pathWork, string pathLogs, string market)
        {
            _pathMarkets = pathMarkets;
            _pathWork = pathWork;
            _pathLogs = pathLogs;
            _index = market;
            client.onLogin += new OTLoginEvent(onLogin);
            client.onStatusChanged += new OTStatusChangedEvent(onStatusChange);
            client.onRestoreConnection += new OTRestoreConnectionEvent(onRestoreConnection);
            client.onError += new OTErrorEvent(onError);
            client.onMessage += new OTMessageEvent(onMessage);
            client.onRealtimeMMQuote += new OTMMQuoteEvent(onMMQuote);
            client.onRealtimeTrade += new OTTradeEvent(onTrade);
            client.onRealtimeBBO += new OTBBOEvent(onBBO);
            client.onRealtimeQuote += new OTQuoteEvent(onQuote);
            client.onBookCancel += new OTBookCancelEvent(onBookCancel);
            client.onBookChange += new OTBookChangeEvent(onBookChange);
            client.onBookDelete += new OTBookDeleteEvent(onBookDelete);
            client.onBookExecute += new OTBookExecuteEvent(onBookExecute);
            client.onBookOrder += new OTBookOrderEvent(onBookOrder);
            client.onBookPriceLevel += new OTBookPriceLevelEvent(onBookPriceLevel);
            client.onBookPurge += new OTBookPurgeEvent(onBookPurge);
            client.onBookReplace += new OTBookReplaceEvent(onBookReplace);
            client.onHistoricalMMQuote += new OTMMQuoteEvent(onMMQuote);
            client.onHistoricalTrade += new OTTradeEvent(onTrade);
            client.onHistoricalBBO += new OTBBOEvent(onBBO);
            client.onHistoricalQuote += new OTQuoteEvent(onQuote);
            client.onHistoricalOHLC += new OTOHLCEvent(onHistoricalOHLC);
            client.onListExchanges += new OTListExchangesEvent(onListExchanges);
            client.onListSymbols += new OTListSymbolsEvent(onListSymbols);
        }

        #region NASDAQ
        public void GetMarketSymbolsViaNASDAQ(string market)
        {
            string exchange = "";
            switch (market)
            {
                case "NASDAQ":
                    exchange = "Q";
                    break;
                case "NYSE":
                    exchange = "N";
                    break;
                case "AMEX":
                    exchange = "1";
                    break;
            }
            _log += DateTime.Now + " : Importing " + market + " stock symbol data.\r\n";
            _log += DateTime.Now + " : ------------------------------------------------------------------\r\n";
            StreamReader sr;
            try
            {

                string urlstr = "http://www.nasdaq.com/screening/companies-by-name.aspx?letter=0&exchange=nasdaq&render=download";
                //string urlstr = "http://www.nasdaq.com//asp/symbols.asp?exchange=" + exchange + "&start=0";
                WebRequest wreq;
                WebResponse wres;
                wreq = HttpWebRequest.Create(urlstr);
                wres = wreq.GetResponse();
                sr = new StreamReader(wres.GetResponseStream());
                FileStream fs = new FileStream(_pathWork + exchange + ".csv", FileMode.Create, FileAccess.Write);
                StreamWriter s = new StreamWriter(fs);
                s.Write(sr.ReadToEnd());
                s.Close();
                sr.Close();
            }
            catch (Exception ex)
            {
                _log += ex.ToString();
                //MessageBox.Show(ex.ToString());
            }
            StreamReader br;
            try
            {
                Regex rex = new Regex("\\,(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                sr = new System.IO.StreamReader(_pathWork + exchange + ".csv", System.Text.Encoding.Default);
                br = new System.IO.StreamReader(sr.BaseStream, sr.CurrentEncoding);
                string line = null;
                if (!Directory.Exists(_pathMarkets + market))
                    Directory.CreateDirectory(_pathMarkets + market);
                XmlTextWriter tw = new XmlTextWriter(_pathMarkets + market + "\\!INDEX.xml", null);
                tw.Formatting = Formatting.Indented;
                tw.WriteStartDocument(true);
                tw.WriteStartElement("market");
                tw.WriteAttributeString("name", exchange);
                tw.WriteAttributeString("type", "stock");
                br.ReadLine();
                //ArrayList temp = new ArrayList();
                br.ReadLine(); // header
                while ((line = br.ReadLine()) != null)
                {
                    //string[] dataArray = new string[7];
                    string[] dataArray = { line };
                    //MessageBox.Show(dataArray[0]);
                    //dataArray = rex.Split(line);

                    tw.WriteStartElement("instrument");
                    tw.WriteElementString("symbol", dataArray[0]);
                    tw.WriteElementString("name", dataArray[1]);
                    tw.WriteEndElement();
                    _log += DateTime.Now + " : " + dataArray[1] + "\r\n";
                }
                tw.WriteEndElement();
                tw.WriteEndDocument();
                tw.Flush();
                tw.Close();
            }
            catch (Exception ex)
            {
                _log += ex.ToString();
                //MessageBox.Show(ex.ToString());
            }
            _log += DateTime.Now + " : ------------------------------------------------------------------\r\n";
            _log += DateTime.Now + " : " + market + " symbol data imported.\r\n";
        }
        public void GetIndexSymbolsViaNASDAQ(string index)
        {
            _log += DateTime.Now + " : Importing " + index + " stock symbol data.\r\n";
            _log += DateTime.Now + " : ------------------------------------------------------------------\r\n";
            StreamReader sr;
            try
            {
                string urlstr = "http://www.nasdaq.com/screening/companies-by-name.aspx?letter=0&exchange=nasdaq&render=download";
                //string urlstr = "http://www.nasdaq.com//asp/symbols.asp?exchange=" + exchange + "&start=0";
                WebRequest wreq;
                WebResponse wres;
                wreq = HttpWebRequest.Create(urlstr);
                wres = wreq.GetResponse();
                sr = new StreamReader(wres.GetResponseStream());
                FileStream fs = new FileStream(_pathWork + index + ".csv", FileMode.Create, FileAccess.Write);
                StreamWriter s = new StreamWriter(fs);
                s.Write(sr.ReadToEnd());
                s.Close();
                sr.Close();
            }
            catch (Exception ex)
            {
                _log += ex.ToString();
                //MessageBox.Show(ex.ToString());
            }
            try
            {
                DataTable dt = SupportClass.ParseCSVFile(_pathWork + index + ".csv");
                if (!Directory.Exists(_pathMarkets + index))
                    Directory.CreateDirectory(_pathMarkets + index);
                XmlTextWriter tw = new XmlTextWriter(_pathMarkets + index + "\\" + "!INDEX.xml", null);
                tw.Formatting = Formatting.Indented;
                tw.WriteStartDocument(true);
                tw.WriteStartElement("index");
                tw.WriteAttributeString("symbol", index);
                tw.WriteAttributeString("name", dt.Rows[0].ItemArray[0].ToString());
                tw.WriteAttributeString("type", "stock");
                for (int x = 1; x < dt.Rows.Count - 1; x++)
                {
                    tw.WriteStartElement("instrument");
                    tw.WriteElementString("symbol", dt.Rows[x].ItemArray[1].ToString());
                    tw.WriteElementString("name", dt.Rows[x].ItemArray[0].ToString());
                    tw.WriteEndElement();
                    _log += DateTime.Now + " : " + dt.Rows[x].ItemArray[1].ToString() + " - " + dt.Rows[x].ItemArray[0].ToString() + "\r\n";
                }
                tw.WriteEndElement();
                tw.WriteEndDocument();
                tw.Flush();
                tw.Close();
            }
            catch (Exception ex)
            {
                _log += ex.ToString();
                //MessageBox.Show(ex.ToString());
            }
            _log += DateTime.Now + " : ------------------------------------------------------------------\r\n";
            _log += DateTime.Now + " : " + index + " symbol data imported.\r\n";
        }
        public void GetIndexSymbolsViaNASDAQ()
        {
            string index = _index;
            _log += DateTime.Now + " : Importing " + index + " stock symbol data.\r\n";
            _log += DateTime.Now + " : ------------------------------------------------------------------\r\n";
            StreamReader sr;
            try
            {
                string urlstr = "http://www.nasdaq.com/screening/companies-by-name.aspx?letter=0&exchange=nasdaq&render=download";
                //string urlstr = "http://www.nasdaq.com//asp/symbols.asp?exchange=" + exchange + "&start=0";
                WebRequest wreq;
                WebResponse wres;
                wreq = HttpWebRequest.Create(urlstr);
                wres = wreq.GetResponse();
                sr = new StreamReader(wres.GetResponseStream());
                FileStream fs = new FileStream(_pathWork + index + ".csv", FileMode.Create, FileAccess.Write);
                StreamWriter s = new StreamWriter(fs);
                s.Write(sr.ReadToEnd());
                s.Close();
                sr.Close();
            }
            catch (Exception ex)
            {
                _log += ex.ToString();
                //MessageBox.Show(ex.ToString());
            }
            try
            {
                GenericParserAdapter gpa = new GenericParserAdapter(_pathWork + index + ".csv");
                DataTable dt = gpa.GetDataTable();
                //DataTable dt = GenericParser.(_pathWork + index + ".csv");
                if (!Directory.Exists(_pathMarkets + index))
                    Directory.CreateDirectory(_pathMarkets + index);
                XmlTextWriter tw = new XmlTextWriter(_pathMarkets + index + "\\" + "!INDEX.xml", null);
                tw.Formatting = Formatting.Indented;
                tw.WriteStartDocument(true);
                tw.WriteStartElement("index");
                tw.WriteAttributeString("symbol", "");
                tw.WriteAttributeString("name", "");
                tw.WriteAttributeString("type", "");
                for (int x = 1; x < dt.Rows.Count - 1; x++)
                {
                    tw.WriteStartElement("instrument");
                    tw.WriteElementString("symbol", dt.Rows[x].ItemArray[0].ToString());
                    tw.WriteElementString("name", dt.Rows[x].ItemArray[1].ToString());
                    tw.WriteEndElement();
                    _log += DateTime.Now + " : " + dt.Rows[x].ItemArray[0].ToString() + " - " + dt.Rows[x].ItemArray[1].ToString() + "\r\n";
                }
                tw.WriteEndElement();
                tw.WriteEndDocument();
                tw.Flush();
                tw.Close();
            }
            catch (Exception ex)
            {
                _log += ex.ToString();
                //MessageBox.Show(ex.ToString());
            }
            _log += DateTime.Now + " : ------------------------------------------------------------------\r\n";
            _log += DateTime.Now + " : " + index + " symbol data imported.\r\n";
        }
        public void GetMarketSymbolsViaNASDAQTrader(string market)
        {
            string exchange = "";
            switch (market)
            {
                case "NASDAQ":
                    exchange = "nasdaq";
                    break;
                case "NASDAQ.FUNDS":
                    exchange = "fundsym";
                    break;
                case "NYSE":
                    exchange = "cqsSym";
                    break;
                case "AMEX":
                    exchange = "cqsSym";
                    break;
                case "OTCBB":
                    exchange = "otc";
                    break;
            }
            _log += DateTime.Now + " : Importing " + market + " stock symbol data.\r\n";
            _log += DateTime.Now + " : ------------------------------------------------------------------\r\n";
            StreamReader sr;
            try
            {
                string urlstr = "http://www.nasdaqtrader.com/dynamic/SymDir/" + exchange + ".txt";
                WebRequest wreq;
                WebResponse wres;
                wreq = HttpWebRequest.Create(urlstr);
                wres = wreq.GetResponse();
                sr = new StreamReader(wres.GetResponseStream());
                FileStream fs = new FileStream(_pathWork + exchange + ".txt", FileMode.Create, FileAccess.Write);
                StreamWriter s = new StreamWriter(fs);
                s.Write(sr.ReadToEnd());
                s.Close();
                sr.Close();
            }
            catch (Exception ex)
            {
                _log += ex.ToString();
                //MessageBox.Show(ex.ToString());
            }
            StreamReader br;
            try
            {
                Regex rex = new Regex("\\|(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                sr = new System.IO.StreamReader(_pathWork + exchange + ".txt", System.Text.Encoding.Default);
                br = new System.IO.StreamReader(sr.BaseStream, sr.CurrentEncoding);
                string line = null;
                if (!Directory.Exists(_pathMarkets + market))
                    Directory.CreateDirectory(_pathMarkets + market);
                XmlTextWriter tw = new XmlTextWriter(_pathMarkets + market + "\\!INDEX.xml", null);
                tw.Formatting = Formatting.Indented;
                tw.WriteStartDocument(true);
                tw.WriteStartElement("market");
                tw.WriteAttributeString("name", market);
                if (exchange == "fundsym")
                    tw.WriteAttributeString("type", "fund");
                else
                    tw.WriteAttributeString("type", "stock");
                br.ReadLine();
                ArrayList temp = new ArrayList();
                line = br.ReadLine(); // header
                while ((line = br.ReadLine()) != null)
                {
                    string[] dataArray = new string[7];
                    dataArray = rex.Split(line);
                    if (dataArray[1] != "")
                    {
                        if (exchange == "cqsSym")
                        {
                            if (market == "AMEX")
                                if (dataArray[2] == "AMEX")
                                {
                                    tw.WriteStartElement("instrument");
                                    tw.WriteElementString("symbol", dataArray[0]);
                                    tw.WriteElementString("name", dataArray[1]);
                                    tw.WriteEndElement();
                                    _log += DateTime.Now + " : " + dataArray[1] + "\r\n";
                                }
                            if (market == "NYSE")
                                if (dataArray[2] == "NYSE")
                                {
                                    tw.WriteStartElement("instrument");
                                    tw.WriteElementString("symbol", dataArray[0]);
                                    tw.WriteElementString("name", dataArray[1]);
                                    tw.WriteEndElement();
                                    _log += DateTime.Now + " : " + dataArray[1] + "\r\n";
                                }
                        }
                        else
                        {
                            tw.WriteStartElement("instrument");
                            tw.WriteElementString("symbol", dataArray[0]);
                            tw.WriteElementString("name", dataArray[1]);
                            tw.WriteEndElement();
                            _log += DateTime.Now + " : " + dataArray[1] + "\r\n";
                        }
                    }
                }
                tw.WriteEndElement();
                tw.WriteEndDocument();
                tw.Flush();
                tw.Close();
            }
            catch (Exception ex)
            {
                _log += ex.ToString();
                //MessageBox.Show(ex.ToString());
            }
            _log += DateTime.Now + " : ------------------------------------------------------------------\r\n";
            _log += DateTime.Now + " : " + market + " symbol data imported.\r\n";
        }
        public void GetMarketSymbolsViaNASDAQTrader()
        {
            string market = _index;
            string exchange = "";
            switch (market)
            {
                case "NASDAQ":
                    exchange = "nasdaq";
                    break;
                case "NASDAQ.FUNDS":
                    exchange = "fundsym";
                    break;
                case "NYSE":
                    exchange = "cqsSym";
                    break;
                case "AMEX":
                    exchange = "cqsSym";
                    break;
                case "OTCBB":
                    exchange = "otc";
                    break;
            }
            _log += DateTime.Now + " : Importing " + market + " stock symbol data.\r\n";
            _log += DateTime.Now + " : ------------------------------------------------------------------\r\n";
            StreamReader sr;
            try
            {
                string urlstr = "http://www.nasdaqtrader.com/dynamic/SymDir/" + exchange + ".txt";
                WebRequest wreq;
                WebResponse wres;
                wreq = HttpWebRequest.Create(urlstr);
                wres = wreq.GetResponse();
                sr = new StreamReader(wres.GetResponseStream());
                FileStream fs = new FileStream(_pathWork + exchange + ".txt", FileMode.Create, FileAccess.Write);
                StreamWriter s = new StreamWriter(fs);
                s.Write(sr.ReadToEnd());
                s.Close();
                sr.Close();
            }
            catch (Exception ex)
            {
                _log += ex.ToString();
                //MessageBox.Show(ex.ToString());
            }
            StreamReader br;
            try
            {
                Regex rex = new Regex("\\|(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                sr = new System.IO.StreamReader(_pathWork + exchange + ".txt", System.Text.Encoding.Default);
                br = new System.IO.StreamReader(sr.BaseStream, sr.CurrentEncoding);
                string line = null;
                if (!Directory.Exists(_pathMarkets + market))
                    Directory.CreateDirectory(_pathMarkets + market);
                XmlTextWriter tw = new XmlTextWriter(_pathMarkets + market + "\\!INDEX.xml", null);
                tw.Formatting = Formatting.Indented;
                tw.WriteStartDocument(true);
                tw.WriteStartElement("market");
                tw.WriteAttributeString("name", market);
                if (exchange == "fundsym")
                    tw.WriteAttributeString("type", "fund");
                else
                    tw.WriteAttributeString("type", "stock");
                br.ReadLine();
                ArrayList temp = new ArrayList();
                line = br.ReadLine(); // header
                while ((line = br.ReadLine()) != null)
                {
                    string[] dataArray = new string[7];
                    dataArray = rex.Split(line);
                    if (dataArray[1] != "")
                    {
                        if (exchange == "cqsSym")
                        {
                            if (market == "AMEX")
                                if (dataArray[2] == "AMEX")
                                {
                                    tw.WriteStartElement("instrument");
                                    tw.WriteElementString("symbol", dataArray[0]);
                                    tw.WriteElementString("name", dataArray[1]);
                                    tw.WriteEndElement();
                                    _log += DateTime.Now + " : " + dataArray[1] + "\r\n";
                                }
                            if (market == "NYSE")
                                if (dataArray[2] == "NYSE")
                                {
                                    tw.WriteStartElement("instrument");
                                    tw.WriteElementString("symbol", dataArray[0]);
                                    tw.WriteElementString("name", dataArray[1]);
                                    tw.WriteEndElement();
                                    _log += DateTime.Now + " : " + dataArray[1] + "\r\n";
                                }
                        }
                        else
                        {
                            tw.WriteStartElement("instrument");
                            tw.WriteElementString("symbol", dataArray[0]);
                            tw.WriteElementString("name", dataArray[1]);
                            tw.WriteEndElement();
                            _log += DateTime.Now + " : " + dataArray[1] + "\r\n";
                        }
                    }
                }
                tw.WriteEndElement();
                tw.WriteEndDocument();
                tw.Flush();
                tw.Close();
            }
            catch (Exception ex)
            {
                _log += ex.ToString();
                //MessageBox.Show(ex.ToString());
            }
            _log += DateTime.Now + " : ------------------------------------------------------------------\r\n";
            _log += DateTime.Now + " : " + market + " symbol data imported.\r\n";
        }
        #endregion

        #region Yahoo
        public string GetHistoricMarketDataViaYAHOO(BackgroundWorker worker, DoWorkEventArgs e, string marketname)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            _log = "";
            string directory = _pathMarkets + marketname + "\\";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            Market market = new Market();
            market.Load(_pathMarkets, marketname);
            ArrayList _exceptionList = new ArrayList();
            /*
            if (File.Exists(_pathMarkets + marketname + "\\" + "!EXCEPTIONS_SYMBOLS.txt"))
            {
                StreamReader sr = new StreamReader(_pathMarkets + marketname + "\\" + "!EXCEPTIONS_SYMBOLS.txt");
                string line = sr.ReadLine();
                while (line != null)
                {
                    _exceptionList.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
             */
            for (int i = 0; i < market._instruments.Count; i++)
            {
                //if (!_exceptionList.Contains(market._instruments[i].ToString()))
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                }
                else
                {
                    if (!File.Exists(directory + market._instruments[i].ToString() + ".csv") || new FileInfo(directory + market._instruments[i].ToString() + ".csv").Length < 45)
                    {
                        try
                        {
                            string url;
                            if (marketname == "otcbb")
                                url = "http://ichart.finance.yahoo.com/table.csv?s=" + market._instruments[i].ToString() + ".OB&d=" + DateTime.Now.Month.ToString() + "&e=" + DateTime.Now.Day.ToString() + "&f=" + DateTime.Now.Year.ToString() + "&g=d&ignore=.csv";
                            else
                                url = "http://ichart.finance.yahoo.com/table.csv?s=" + market._instruments[i].ToString() + "&d=" + DateTime.Now.Month.ToString() + "&e=" + DateTime.Now.Day.ToString() + "&f=" + DateTime.Now.Year.ToString() + "&g=d&ignore=.csv";
                            _log += DateTime.Now + " : Retrieving data from " + url;
                            WebRequest wreq = HttpWebRequest.Create(url);
                            WebResponse wres = wreq.GetResponse();
                            StreamReader sr = new StreamReader(wres.GetResponseStream());
                            FileStream fs = new FileStream(directory + market._instruments[i].ToString() + ".csv", FileMode.Create, FileAccess.Write);
                            StreamWriter s = new StreamWriter(fs);
                            s.Write(sr.ReadToEnd());
                            s.Close();
                            sr.Close();
                            if (new FileInfo(directory + market._instruments[i].ToString() + ".csv").Length > 44)
                            {
                                _log += " : imported\r\n";
                                _log += SupportClass.GetDataQualityReport(marketname, market._instruments[i].ToString(), directory);
                            }
                            else
                            {
                                _log += " : skipped\r\n";
                            }
                        }
                        catch (Exception ex)
                        {
                            _log += ex.ToString();
                            _log += " : exception" + "\r\n";
                            _log += ex + "\r\n";
                            if (_internetConnection.Active() == true)
                            {
                                if (!_exceptionList.Contains(market._instruments[i].ToString()))
                                    _exceptionList.Add(market._instruments[i].ToString());
                            }
                        }
                        try
                        {
                            string url = "http://ichart.finance.yahoo.com/table.csv?s=" + market._instruments[i].ToString() + "&d=" + DateTime.Now.Month.ToString() + "&e=" + DateTime.Now.Day.ToString() + "&f=" + DateTime.Now.Year.ToString() + "&g=v&ignore=.csv";
                            _log += DateTime.Now + " : Retrieving dividend data from " + url;
                            WebRequest wreq = HttpWebRequest.Create(url);
                            WebResponse wres = wreq.GetResponse();
                            StreamReader sr = new StreamReader(wres.GetResponseStream());
                            FileStream fs = new FileStream(directory + market._instruments[i].ToString() + "-dividends.csv", FileMode.Create, FileAccess.Write);
                            StreamWriter s = new StreamWriter(fs);
                            s.Write(sr.ReadToEnd());
                            s.Close();
                            sr.Close();
                            _log += " : imported\r\n";
                        }
                        catch
                        {
                            _log += " : exception" + "\r\n";
                        }
                        try
                        {
                            string url = string.Format("http://finance.yahoo.com/q/in?s={0}", market._instruments[i].ToString());
                            _log += DateTime.Now + " : Retrieving industry data from " + url;
                            StringBuilder sb = new StringBuilder();
                            byte[] buf = new byte[8192];
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            Stream resStream = response.GetResponseStream();
                            string tempString = null;
                            int count = 0;
                            do
                            {
                                count = resStream.Read(buf, 0, buf.Length);
                                if (count != 0)
                                {
                                    tempString = Encoding.ASCII.GetString(buf, 0, count);
                                    sb.Append(tempString);
                                }
                            }
                            while (count > 0); // any more data to read?
                            string html = sb.ToString();
                            FileStream fs = new FileStream(directory + market._instruments[i].ToString() + "-industry.txt", FileMode.Create, FileAccess.Write);
                            // create a writer and open the file
                            TextWriter tw = new StreamWriter(fs);
                            Regex pattern = new Regex("yfnc_tablehead1.>Sector:</td><td.*?html.>(?<variable>.*?)</a></td></tr>");
                            Match match = pattern.Match(html);
                            string parsed_str = match.Groups["variable"].Value;
                            tw.WriteLine(parsed_str);  // sector
                            pattern = new Regex("yfnc_tablehead1.>Industry:</td><td.*?html.>(?<variable>.*?)</a></td></tr>");
                            match = pattern.Match(html);
                            parsed_str = match.Groups["variable"].Value;
                            tw.WriteLine(parsed_str);  // industry
                            // close the stream
                            tw.Close();
                            _log += " : imported\r\n";
                        }
                        catch
                        {
                            _log += " : exception" + "\r\n";
                        }
                        try
                        {
                            string url = string.Format("http://finance.yahoo.com/q/ks?s={0}", market._instruments[i].ToString());
                            _log += DateTime.Now + " : Retrieving fundamental data from " + url;
                            StringBuilder sb = new StringBuilder();
                            byte[] buf = new byte[8192];
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            Stream resStream = response.GetResponseStream();
                            string tempString = null;
                            int count = 0;
                            do
                            {
                                count = resStream.Read(buf, 0, buf.Length);
                                if (count != 0)
                                {
                                    tempString = Encoding.ASCII.GetString(buf, 0, count);
                                    sb.Append(tempString);
                                }
                            }
                            while (count > 0); // any more data to read?
                            string html = sb.ToString();
                            FileStream fs = new FileStream(directory + market._instruments[i].ToString() + "-fundamentals.txt", FileMode.Create, FileAccess.Write);
                            // create a writer and open the file
                            TextWriter tw = new StreamWriter(fs);
                            Regex pattern = new Regex(">Market Cap (.*?):</td><td class=.yfnc_tabledata1.><span id=.yfs_j10_goog.>(?<variable>.*?)</span></td></tr><tr>");
                            Match match = pattern.Match(html);
                            string parsed_str = match.Groups["variable"].Value;
                            tw.WriteLine(parsed_str);  // market cap
                            pattern = new Regex(">Enterprise Value (.*?)<font size=.-1.><sup>3</sup></font>:</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                            match = pattern.Match(html);
                            parsed_str = match.Groups["variable"].Value;
                            tw.WriteLine(parsed_str);  // enterprise value
                            pattern = new Regex(">Trailing P/E (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                            match = pattern.Match(html);
                            parsed_str = match.Groups["variable"].Value;
                            tw.WriteLine(parsed_str);  // trailing pe
                            pattern = new Regex(">Forward P/E (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                            match = pattern.Match(html);
                            parsed_str = match.Groups["variable"].Value;
                            tw.WriteLine(parsed_str);  // forward pe
                            pattern = new Regex(">PEG Ratio (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?) x</td>");
                            match = pattern.Match(html);
                            parsed_str = match.Groups["variable"].Value;
                            tw.WriteLine(parsed_str);  // peg ratio
                            pattern = new Regex(">Price/Sales (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                            match = pattern.Match(html);
                            parsed_str = match.Groups["variable"].Value;
                            tw.WriteLine(parsed_str);  // price sales
                            pattern = new Regex(">Price/Book (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                            match = pattern.Match(html);
                            parsed_str = match.Groups["variable"].Value;
                            tw.WriteLine(parsed_str);  // price book
                            pattern = new Regex(">Enterprise Value/Revenue (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                            match = pattern.Match(html);
                            parsed_str = match.Groups["variable"].Value;
                            tw.WriteLine(parsed_str);  // enterprise value revenue
                            pattern = new Regex(">Enterprise Value/EBITDA (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                            match = pattern.Match(html);
                            parsed_str = match.Groups["variable"].Value;
                            tw.WriteLine(parsed_str);  // enterprise value ebitda
                            pattern = new Regex(">Beta:</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                            match = pattern.Match(html);
                            parsed_str = match.Groups["variable"].Value;
                            if (parsed_str != "" && parsed_str != "N/A" && parsed_str != "NaN")
                                tw.WriteLine(parsed_str);  // enterprise value beta
                            else
                                tw.WriteLine("0");
                            // close the stream
                            tw.Close();
                            _log += " : imported\r\n";
                        }
                        catch
                        {
                            _log += " : exception" + "\r\n";
                        }
                    }
                    else
                    {
                        if (File.GetLastWriteTime(directory + market._instruments[i].ToString() + ".csv") < SupportClass.GetPreviousMarketClose())
                        {
                            bool flag = false;
                            int retry = 0;
                            while (flag == false && retry < 1/*int.Parse(textBoxQuotesRetries.Text)*/)
                            {
                                if (worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                }
                                else
                                {
                                    try
                                    {
                                        string url = "http://ichart.finance.yahoo.com/table.csv?s=" + market._instruments[i].ToString() + "&d=" + DateTime.Now.Month.ToString() + "&e=" + DateTime.Now.Day.ToString() + "&f=" + DateTime.Now.Year.ToString() + "&g=d&ignore=.csv";
                                        _log += DateTime.Now + " : Retrieving data from " + url;
                                        WebRequest wreq = HttpWebRequest.Create(url);
                                        WebResponse wres = wreq.GetResponse();
                                        StreamReader sr = new StreamReader(wres.GetResponseStream());
                                        FileStream fs = new FileStream(directory + market._instruments[i].ToString() + ".xxx", FileMode.Create, FileAccess.Write);
                                        StreamWriter s = new StreamWriter(fs);
                                        s.Write(sr.ReadToEnd());
                                        s.Close();
                                        sr.Close();
                                        FileInfo oldFile = new FileInfo(directory + market._instruments[i].ToString() + ".csv");
                                        FileInfo newFile = new FileInfo(directory + market._instruments[i].ToString() + ".xxx");
                                        if (newFile.Length > oldFile.Length)
                                        {
                                            File.Delete(directory + market._instruments[i].ToString() + ".csv");
                                            File.Move(directory + market._instruments[i].ToString() + ".xxx", directory + market._instruments[i].ToString() + ".csv");
                                            _log += " : updated\r\n";
                                            _log += SupportClass.GetDataQualityReport(marketname, market._instruments[i].ToString(), directory);
                                            flag = true;
                                        }
                                        else
                                        {
                                            File.Delete(directory + market._instruments[i].ToString() + ".xxx");
                                            _log += " : skipped\r\n";
                                            System.Threading.Thread.Sleep(100);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _log += ex.ToString();
                                        _log += " : exception\r\n";
                                        _log += ex + "\r\n";
                                        if (_internetConnection.Active() == true)
                                        {
                                            if (!_exceptionList.Contains(market._instruments[i].ToString()))
                                                _exceptionList.Add(market._instruments[i].ToString());
                                        }
                                    }
                                    retry++;
                                }
                            }
                            try
                            {
                                string url = "http://ichart.finance.yahoo.com/table.csv?s=" + market._instruments[i].ToString() + "&d=" + DateTime.Now.Month.ToString() + "&e=" + DateTime.Now.Day.ToString() + "&f=" + DateTime.Now.Year.ToString() + "&g=v&ignore=.csv";
                                _log += DateTime.Now + " : Retrieving dividend data from " + url;
                                WebRequest wreq = HttpWebRequest.Create(url);
                                WebResponse wres = wreq.GetResponse();
                                StreamReader sr = new StreamReader(wres.GetResponseStream());
                                FileStream fs = new FileStream(directory + market._instruments[i].ToString() + "-dividends.csv", FileMode.Create, FileAccess.Write);
                                StreamWriter s = new StreamWriter(fs);
                                s.Write(sr.ReadToEnd());
                                s.Close();
                                sr.Close();
                                _log += " : imported\r\n";
                            }
                            catch
                            {
                                _log += " : exception" + "\r\n";
                            }
                            try
                            {
                                string url = string.Format("http://finance.yahoo.com/q/in?s={0}", market._instruments[i].ToString());
                                _log += DateTime.Now + " : Retrieving industry data from " + url;
                                StringBuilder sb = new StringBuilder();
                                byte[] buf = new byte[8192];
                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                Stream resStream = response.GetResponseStream();
                                string tempString = null;
                                int count = 0;
                                do
                                {
                                    count = resStream.Read(buf, 0, buf.Length);
                                    if (count != 0)
                                    {
                                        tempString = Encoding.ASCII.GetString(buf, 0, count);
                                        sb.Append(tempString);
                                    }
                                }
                                while (count > 0); // any more data to read?
                                string html = sb.ToString();
                                FileStream fs = new FileStream(directory + market._instruments[i].ToString() + "-industry.txt", FileMode.Create, FileAccess.Write);
                                // create a writer and open the file
                                TextWriter tw = new StreamWriter(fs);
                                Regex pattern = new Regex("yfnc_tablehead1.>Sector:</td><td.*?html.>(?<variable>.*?)</a></td></tr>");
                                Match match = pattern.Match(html);
                                string parsed_str = match.Groups["variable"].Value;
                                tw.WriteLine(parsed_str);  // sector
                                pattern = new Regex("yfnc_tablehead1.>Industry:</td><td.*?html.>(?<variable>.*?)</a></td></tr>");
                                match = pattern.Match(html);
                                parsed_str = match.Groups["variable"].Value;
                                tw.WriteLine(parsed_str);  // industry
                                // close the stream
                                tw.Close();
                                _log += " : imported\r\n";
                            }
                            catch
                            {
                                _log += " : exception" + "\r\n";
                            }
                            try
                            {
                                string url = string.Format("http://finance.yahoo.com/q/ks?s={0}", market._instruments[i].ToString());
                                _log += DateTime.Now + " : Retrieving fundamental data from " + url;
                                StringBuilder sb = new StringBuilder();
                                byte[] buf = new byte[8192];
                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                Stream resStream = response.GetResponseStream();
                                string tempString = null;
                                int count = 0;
                                do
                                {
                                    count = resStream.Read(buf, 0, buf.Length);
                                    if (count != 0)
                                    {
                                        tempString = Encoding.ASCII.GetString(buf, 0, count);
                                        sb.Append(tempString);
                                    }
                                }
                                while (count > 0); // any more data to read?
                                string html = sb.ToString();
                                FileStream fs = new FileStream(directory + market._instruments[i].ToString() + "-fundamentals.txt", FileMode.Create, FileAccess.Write);
                                // create a writer and open the file
                                TextWriter tw = new StreamWriter(fs);
                                Regex pattern = new Regex(">Market Cap (.*?):</td><td class=.yfnc_tabledata1.><span id=.yfs_j10_goog.>(?<variable>.*?)</span></td></tr><tr>");
                                Match match = pattern.Match(html);
                                string parsed_str = match.Groups["variable"].Value;
                                tw.WriteLine(parsed_str);  // market cap
                                pattern = new Regex(">Enterprise Value (.*?)<font size=.-1.><sup>3</sup></font>:</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                                match = pattern.Match(html);
                                parsed_str = match.Groups["variable"].Value;
                                tw.WriteLine(parsed_str);  // enterprise value
                                pattern = new Regex(">Trailing P/E (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                                match = pattern.Match(html);
                                parsed_str = match.Groups["variable"].Value;
                                tw.WriteLine(parsed_str);  // trailing pe
                                pattern = new Regex(">Forward P/E (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                                match = pattern.Match(html);
                                parsed_str = match.Groups["variable"].Value;
                                tw.WriteLine(parsed_str);  // forward pe
                                pattern = new Regex(">PEG Ratio (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                                match = pattern.Match(html);
                                parsed_str = match.Groups["variable"].Value;
                                tw.WriteLine(parsed_str);  // peg ratio
                                pattern = new Regex(">Price/Sales (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                                match = pattern.Match(html);
                                parsed_str = match.Groups["variable"].Value;
                                tw.WriteLine(parsed_str);  // price sales
                                pattern = new Regex(">Price/Book (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                                match = pattern.Match(html);
                                parsed_str = match.Groups["variable"].Value;
                                tw.WriteLine(parsed_str);  // price book
                                pattern = new Regex(">Enterprise Value/Revenue (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                                match = pattern.Match(html);
                                parsed_str = match.Groups["variable"].Value;
                                tw.WriteLine(parsed_str);  // enterprise value revenue
                                pattern = new Regex(">Enterprise Value/EBITDA (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                                match = pattern.Match(html);
                                parsed_str = match.Groups["variable"].Value;
                                tw.WriteLine(parsed_str);  // enterprise value ebitda
                                pattern = new Regex(">Beta:</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                                match = pattern.Match(html);
                                parsed_str = match.Groups["variable"].Value;
                                if (parsed_str != "" && parsed_str != "N/A" && parsed_str != "NaN")
                                    tw.WriteLine(parsed_str);  // enterprise value beta
                                else
                                    tw.WriteLine("0");
                                // close the stream
                                tw.Close();
                                _log += " : imported\r\n";
                            }
                            catch
                            {
                                _log += " : exception" + "\r\n";
                            }
                        }
                    }
                }
                int percentComplete = (int)((float)i / (float)market._instruments.Count * 100);
                worker.ReportProgress(percentComplete);
                //System.Windows.Forms.Application.DoEvents();
            }

            //if (checkBoxQuotesGenerateSymbolExceptions.Checked == true)
            //{
            try
            {
                FileStream fs = new FileStream(_pathMarkets + marketname + "\\" + "!EXCEPTIONS_SYMBOLS.txt", FileMode.Create, FileAccess.Write);
                StreamWriter s = new StreamWriter(fs);
                _exceptionList.Sort();
                foreach (string key in _exceptionList)
                {
                    s.Write(key + "\r\n");
                }
                s.Close();
            }
            catch (Exception ex)
            {
                _log += ex.ToString();
                //MessageBox.Show(ex.Message);
            }
            //}

            /*
            if (checkBoxQuotesGenerateDateExceptions.Checked == true)
            {
                try
                {
                    FileStream fs = new FileStream(_pathMarkets + marketname + "\\" + "!EXCEPTIONS_DATES.txt", FileMode.Append, FileAccess.Write);
                    StreamWriter s = new StreamWriter(fs);
                    ArrayList aKeys = new ArrayList(_marketDateExceptions.Keys);
                    aKeys.Sort();
                    int index = 2;
                    foreach (DateTime key in aKeys)
                    {
                        if ((int)_marketDateExceptions[key] >= index)
                        {
                            s.Write(key.ToShortDateString() + "\r\n");
                            index = (int)_marketDateExceptions[key];
                        }
                    }
                    s.Close();
                }
                catch (Exception ex)
                {
                    _log += ex.ToString());
                    //MessageBox.Show(ex.Message);
                }
                try
                {
                    FileStream fs = new FileStream(_pathMarkets + marketname + "\\" + "!DATE-RANGE.txt", FileMode.Append, FileAccess.Write);
                    StreamWriter s = new StreamWriter(fs);
                    ArrayList aKeys = new ArrayList(_lastDataDate.Keys);
                    aKeys.Sort();
                    foreach (DateTime key in aKeys)
                    {
                        s.Write(key.ToShortDateString() + "\r\n");
                    }
                    s.Close();
                }
                catch (Exception ex)
                {
                    _log += ex.ToString());
                    //MessageBox.Show(ex.Message);
                }
            }
             */
            if (File.Exists(directory + "*.xxx"))
                File.Delete(directory + "*.xxx");
            _log += DateTime.Now + " : Pass completed\r\n";
            StreamWriter sw = new StreamWriter(new FileStream(_pathLogs + "yahoo.log", FileMode.OpenOrCreate, FileAccess.Write));
            sw.Write(_log);
            sw.Close();
            return _log;
        }
        public string GetHistoricMarketDataViaYAHOO(string marketname)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            _log = "";
            string directory = _pathMarkets + marketname + "\\";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            Market market = new Market();
            market.Load(_pathMarkets, marketname);
            ArrayList _exceptionList = new ArrayList();
            /*
            if (File.Exists(_pathMarkets + marketname + "\\" + "!EXCEPTIONS_SYMBOLS.txt"))
            {
                StreamReader sr = new StreamReader(_pathMarkets + marketname + "\\" + "!EXCEPTIONS_SYMBOLS.txt");
                string line = sr.ReadLine();
                while (line != null)
                {
                    _exceptionList.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
             */
            for (int i = 0; i < market._instruments.Count; i++)
            {
                //if (!_exceptionList.Contains(market._instruments[i].ToString()))
                if (!File.Exists(directory + market._instruments[i].ToString() + ".csv") || new FileInfo(directory + market._instruments[i].ToString() + ".csv").Length < 45)
                {
                    try
                    {
                        string url;
                        if (marketname == "otcbb")
                            url = "http://ichart.finance.yahoo.com/table.csv?s=" + market._instruments[i].ToString() + ".OB&d=" + DateTime.Now.Month.ToString() + "&e=" + DateTime.Now.Day.ToString() + "&f=" + DateTime.Now.Year.ToString() + "&g=d&ignore=.csv";
                        else
                            url = "http://ichart.finance.yahoo.com/table.csv?s=" + market._instruments[i].ToString() + "&d=" + DateTime.Now.Month.ToString() + "&e=" + DateTime.Now.Day.ToString() + "&f=" + DateTime.Now.Year.ToString() + "&g=d&ignore=.csv";
                        _log += DateTime.Now + " : Retrieving data from " + url;
                        WebRequest wreq = HttpWebRequest.Create(url);
                        WebResponse wres = wreq.GetResponse();
                        StreamReader sr = new StreamReader(wres.GetResponseStream());
                        FileStream fs = new FileStream(directory + market._instruments[i].ToString() + ".csv", FileMode.Create, FileAccess.Write);
                        StreamWriter s = new StreamWriter(fs);
                        s.Write(sr.ReadToEnd());
                        s.Close();
                        sr.Close();
                        if (new FileInfo(directory + market._instruments[i].ToString() + ".csv").Length > 44)
                        {
                            _log += " : imported\r\n";
                            _log += SupportClass.GetDataQualityReport(marketname, market._instruments[i].ToString(), directory);
                        }
                        else
                        {
                            _log += " : skipped\r\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        _log += ex.ToString();
                        if (!_exceptionList.Contains(market._instruments[i].ToString()))
                            _exceptionList.Add(market._instruments[i].ToString());
                        _log += " : exception" + "\r\n";
                        _log += ex + "\r\n";
                    }
                    try
                    {
                        string url = "http://ichart.finance.yahoo.com/table.csv?s=" + market._instruments[i].ToString() + "&d=" + DateTime.Now.Month.ToString() + "&e=" + DateTime.Now.Day.ToString() + "&f=" + DateTime.Now.Year.ToString() + "&g=v&ignore=.csv";
                        _log += DateTime.Now + " : Retrieving dividend data from " + url;
                        WebRequest wreq = HttpWebRequest.Create(url);
                        WebResponse wres = wreq.GetResponse();
                        StreamReader sr = new StreamReader(wres.GetResponseStream());
                        FileStream fs = new FileStream(directory + market._instruments[i].ToString() + "-dividends.csv", FileMode.Create, FileAccess.Write);
                        StreamWriter s = new StreamWriter(fs);
                        s.Write(sr.ReadToEnd());
                        s.Close();
                        sr.Close();
                        _log += " : imported\r\n";
                    }
                    catch
                    {
                        _log += " : exception" + "\r\n";
                    }
                    try
                    {
                        string url = string.Format("http://finance.yahoo.com/q/in?s={0}", market._instruments[i].ToString());
                        _log += DateTime.Now + " : Retrieving industry data from " + url;
                        StringBuilder sb = new StringBuilder();
                        byte[] buf = new byte[8192];
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Stream resStream = response.GetResponseStream();
                        string tempString = null;
                        int count = 0;
                        do
                        {
                            count = resStream.Read(buf, 0, buf.Length);
                            if (count != 0)
                            {
                                tempString = Encoding.ASCII.GetString(buf, 0, count);
                                sb.Append(tempString);
                            }
                        }
                        while (count > 0); // any more data to read?
                        string html = sb.ToString();
                        FileStream fs = new FileStream(directory + market._instruments[i].ToString() + "-industry.txt", FileMode.Create, FileAccess.Write);
                        // create a writer and open the file
                        TextWriter tw = new StreamWriter(fs);
                        Regex pattern = new Regex("yfnc_tablehead1.>Sector:</td><td.*?html.>(?<variable>.*?)</a></td></tr>");
                        Match match = pattern.Match(html);
                        string parsed_str = match.Groups["variable"].Value;
                        tw.WriteLine(parsed_str);  // sector
                        pattern = new Regex("yfnc_tablehead1.>Industry:</td><td.*?html.>(?<variable>.*?)</a></td></tr>");
                        match = pattern.Match(html);
                        parsed_str = match.Groups["variable"].Value;
                        tw.WriteLine(parsed_str);  // industry
                        // close the stream
                        tw.Close();
                        _log += " : imported\r\n";
                    }
                    catch
                    {
                        _log += " : exception" + "\r\n";
                    }
                    try
                    {
                        string url = string.Format("http://finance.yahoo.com/q/ks?s={0}", market._instruments[i].ToString());
                        _log += DateTime.Now + " : Retrieving fundamental data from " + url;
                        StringBuilder sb = new StringBuilder();
                        byte[] buf = new byte[8192];
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Stream resStream = response.GetResponseStream();
                        string tempString = null;
                        int count = 0;
                        do
                        {
                            count = resStream.Read(buf, 0, buf.Length);
                            if (count != 0)
                            {
                                tempString = Encoding.ASCII.GetString(buf, 0, count);
                                sb.Append(tempString);
                            }
                        }
                        while (count > 0); // any more data to read?
                        string html = sb.ToString();
                        FileStream fs = new FileStream(directory + market._instruments[i].ToString() + "-fundamentals.txt", FileMode.Create, FileAccess.Write);
                        // create a writer and open the file
                        TextWriter tw = new StreamWriter(fs);
                        Regex pattern = new Regex(">Market Cap (.*?):</td><td class=.yfnc_tabledata1.><span id=.yfs_j10_goog.>(?<variable>.*?)</span></td></tr><tr>");
                        Match match = pattern.Match(html);
                        string parsed_str = match.Groups["variable"].Value;
                        tw.WriteLine(parsed_str);  // market cap
                        pattern = new Regex(">Enterprise Value (.*?)<font size=.-1.><sup>3</sup></font>:</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                        match = pattern.Match(html);
                        parsed_str = match.Groups["variable"].Value;
                        tw.WriteLine(parsed_str);  // enterprise value
                        pattern = new Regex(">Trailing P/E (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                        match = pattern.Match(html);
                        parsed_str = match.Groups["variable"].Value;
                        tw.WriteLine(parsed_str);  // trailing pe
                        pattern = new Regex(">Forward P/E (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                        match = pattern.Match(html);
                        parsed_str = match.Groups["variable"].Value;
                        tw.WriteLine(parsed_str);  // forward pe
                        pattern = new Regex(">PEG Ratio (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                        match = pattern.Match(html);
                        parsed_str = match.Groups["variable"].Value;
                        tw.WriteLine(parsed_str);  // peg ratio
                        pattern = new Regex(">Price/Sales (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                        match = pattern.Match(html);
                        parsed_str = match.Groups["variable"].Value;
                        tw.WriteLine(parsed_str);  // price sales
                        pattern = new Regex(">Price/Book (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                        match = pattern.Match(html);
                        parsed_str = match.Groups["variable"].Value;
                        tw.WriteLine(parsed_str);  // price book
                        pattern = new Regex(">Enterprise Value/Revenue (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                        match = pattern.Match(html);
                        parsed_str = match.Groups["variable"].Value;
                        tw.WriteLine(parsed_str);  // enterprise value revenue
                        pattern = new Regex(">Enterprise Value/EBITDA (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                        match = pattern.Match(html);
                        parsed_str = match.Groups["variable"].Value;
                        tw.WriteLine(parsed_str);  // enterprise value ebitda
                        pattern = new Regex(">Beta:</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                        match = pattern.Match(html);
                        parsed_str = match.Groups["variable"].Value;
                        if (parsed_str != "" && parsed_str != "N/A" && parsed_str != "NaN")
                            tw.WriteLine(parsed_str);  // enterprise value beta
                        else
                            tw.WriteLine("0");
                        // close the stream
                        tw.Close();
                        _log += " : imported\r\n";
                    }
                    catch
                    {
                        _log += " : exception" + "\r\n";
                    }
                }
                else
                {
                    if (File.GetLastWriteTime(directory + market._instruments[i].ToString() + ".csv") < SupportClass.GetPreviousMarketClose())
                    {
                        bool flag = false;
                        int retry = 0;
                        while (flag == false && retry < 1/*int.Parse(textBoxQuotesRetries.Text)*/)
                        {
                            try
                            {
                                string url = "http://ichart.finance.yahoo.com/table.csv?s=" + market._instruments[i].ToString() + "&d=" + DateTime.Now.Month.ToString() + "&e=" + DateTime.Now.Day.ToString() + "&f=" + DateTime.Now.Year.ToString() + "&g=d&ignore=.csv";
                                _log += DateTime.Now + " : Retrieving data from " + url;
                                WebRequest wreq = HttpWebRequest.Create(url);
                                wreq.Timeout = 1000000;
                                WebResponse wres = wreq.GetResponse();
                                StreamReader sr = new StreamReader(wres.GetResponseStream());
                                FileStream fs = new FileStream(directory + market._instruments[i].ToString() + ".xxx", FileMode.Create, FileAccess.Write);
                                StreamWriter s = new StreamWriter(fs);
                                s.Write(sr.ReadToEnd());
                                s.Close();
                                sr.Close();
                                FileInfo oldFile = new FileInfo(directory + market._instruments[i].ToString() + ".csv");
                                FileInfo newFile = new FileInfo(directory + market._instruments[i].ToString() + ".xxx");
                                if (newFile.Length > oldFile.Length)
                                {
                                    File.Delete(directory + market._instruments[i].ToString() + ".csv");
                                    File.Move(directory + market._instruments[i].ToString() + ".xxx", directory + market._instruments[i].ToString() + ".csv");
                                    _log += " : updated\r\n";
                                    _log += SupportClass.GetDataQualityReport(marketname, market._instruments[i].ToString(), directory);
                                    flag = true;
                                }
                                else
                                {
                                    File.Delete(directory + market._instruments[i].ToString() + ".xxx");
                                    _log += " : skipped\r\n";
                                    System.Threading.Thread.Sleep(100);
                                }
                            }
                            catch (Exception ex)
                            {
                                _log += ex.ToString();
                                if (!_exceptionList.Contains(market._instruments[i].ToString()))
                                    _exceptionList.Add(market._instruments[i].ToString());
                                _log += " : exception\r\n";
                                _log += ex + "\r\n";
                            }
                            retry++;
                        }
                    }
                }
                try
                {
                    string url = "http://ichart.finance.yahoo.com/table.csv?s=" + market._instruments[i].ToString() + "&d=" + DateTime.Now.Month.ToString() + "&e=" + DateTime.Now.Day.ToString() + "&f=" + DateTime.Now.Year.ToString() + "&g=v&ignore=.csv";
                    _log += DateTime.Now + " : Retrieving dividend data from " + url;
                    WebRequest wreq = HttpWebRequest.Create(url);
                    WebResponse wres = wreq.GetResponse();
                    StreamReader sr = new StreamReader(wres.GetResponseStream());
                    FileStream fs = new FileStream(directory + market._instruments[i].ToString() + "-dividends.csv", FileMode.Create, FileAccess.Write);
                    StreamWriter s = new StreamWriter(fs);
                    s.Write(sr.ReadToEnd());
                    s.Close();
                    sr.Close();
                    _log += " : imported\r\n";
                }
                catch
                {
                    _log += " : exception" + "\r\n";
                }
                try
                {
                    string url = string.Format("http://finance.yahoo.com/q/in?s={0}", market._instruments[i].ToString());
                    _log += DateTime.Now + " : Retrieving industry data from " + url;
                    StringBuilder sb = new StringBuilder();
                    byte[] buf = new byte[8192];
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream resStream = response.GetResponseStream();
                    string tempString = null;
                    int count = 0;
                    do
                    {
                        count = resStream.Read(buf, 0, buf.Length);
                        if (count != 0)
                        {
                            tempString = Encoding.ASCII.GetString(buf, 0, count);
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0); // any more data to read?
                    string html = sb.ToString();
                    FileStream fs = new FileStream(directory + market._instruments[i].ToString() + "-industry.txt", FileMode.Create, FileAccess.Write);
                    // create a writer and open the file
                    TextWriter tw = new StreamWriter(fs);
                    Regex pattern = new Regex("yfnc_tablehead1.>Sector:</td><td.*?html.>(?<variable>.*?)</a></td></tr>");
                    Match match = pattern.Match(html);
                    string parsed_str = match.Groups["variable"].Value;
                    tw.WriteLine(parsed_str);  // sector
                    pattern = new Regex("yfnc_tablehead1.>Industry:</td><td.*?html.>(?<variable>.*?)</a></td></tr>");
                    match = pattern.Match(html);
                    parsed_str = match.Groups["variable"].Value;
                    tw.WriteLine(parsed_str);  // industry
                    // close the stream
                    tw.Close();
                    _log += " : imported\r\n";
                }
                catch
                {
                    _log += " : exception" + "\r\n";
                }
                try
                {
                    string url = string.Format("http://finance.yahoo.com/q/ks?s={0}", market._instruments[i].ToString());
                    _log += DateTime.Now + " : Retrieving fundamental data from " + url;
                    StringBuilder sb = new StringBuilder();
                    byte[] buf = new byte[8192];
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream resStream = response.GetResponseStream();
                    string tempString = null;
                    int count = 0;
                    do
                    {
                        count = resStream.Read(buf, 0, buf.Length);
                        if (count != 0)
                        {
                            tempString = Encoding.ASCII.GetString(buf, 0, count);
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0); // any more data to read?
                    string html = sb.ToString();
                    FileStream fs = new FileStream(directory + market._instruments[i].ToString() + "-fundamentals.txt", FileMode.Create, FileAccess.Write);
                    // create a writer and open the file
                    TextWriter tw = new StreamWriter(fs);
                    Regex pattern = new Regex("Market Cap (intraday)(.*?)<span id=.yfs_j10_goog.>(?<variable>.*?)</span>");
                    Match match = pattern.Match(html);
                    string parsed_str = match.Groups["variable"].Value;
                    tw.WriteLine(parsed_str);  // market cap
                    pattern = new Regex(">Enterprise Value (.*?)<font size=.-1.><sup>3</sup></font>:</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                    match = pattern.Match(html);
                    parsed_str = match.Groups["variable"].Value;
                    tw.WriteLine(parsed_str);  // enterprise value
                    pattern = new Regex(">Trailing P/E (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                    match = pattern.Match(html);
                    parsed_str = match.Groups["variable"].Value;
                    tw.WriteLine(parsed_str);  // trailing pe
                    pattern = new Regex(">Forward P/E (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                    match = pattern.Match(html);
                    parsed_str = match.Groups["variable"].Value;
                    tw.WriteLine(parsed_str);  // forward pe
                    pattern = new Regex(">PEG Ratio <small>(5 yr expected)</small>:</th><td class=.yfnc_tabledata1.>(?<variable>.*?) x</td>");
                    match = pattern.Match(html);
                    parsed_str = match.Groups["variable"].Value;
                    tw.WriteLine(parsed_str);  // peg ratio
                    pattern = new Regex(">Price/Sales (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                    match = pattern.Match(html);
                    parsed_str = match.Groups["variable"].Value;
                    tw.WriteLine(parsed_str);  // price sales
                    pattern = new Regex(">Price/Book (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                    match = pattern.Match(html);
                    parsed_str = match.Groups["variable"].Value;
                    tw.WriteLine(parsed_str);  // price book
                    pattern = new Regex(">Enterprise Value/Revenue (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                    match = pattern.Match(html);
                    parsed_str = match.Groups["variable"].Value;
                    tw.WriteLine(parsed_str);  // enterprise value revenue
                    pattern = new Regex(">Enterprise Value/EBITDA (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                    match = pattern.Match(html);
                    parsed_str = match.Groups["variable"].Value;
                    tw.WriteLine(parsed_str);  // enterprise value ebitda
                    pattern = new Regex(">Beta:</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
                    match = pattern.Match(html);
                    parsed_str = match.Groups["variable"].Value;
                    if (parsed_str != "" && parsed_str != "N/A" && parsed_str != "NaN")
                        tw.WriteLine(parsed_str);  // enterprise value beta
                    else
                        tw.WriteLine("0");
                    // close the stream
                    tw.Close();
                    _log += " : imported\r\n";
                }
                catch
                {
                    _log += " : exception" + "\r\n";
                }
                int percentComplete = (int)((float)i / (float)market._instruments.Count * 100);
                //this.progressBarQuotes.Value = percentComplete;
                //System.Windows.Forms.Application.DoEvents();
            }

            //if (checkBoxQuotesGenerateSymbolExceptions.Checked == true)
            //{
            try
            {
                FileStream fs = new FileStream(_pathMarkets + marketname + "\\" + "!EXCEPTIONS_SYMBOLS.txt", FileMode.Create, FileAccess.Write);
                StreamWriter s = new StreamWriter(fs);
                _exceptionList.Sort();
                foreach (string key in _exceptionList)
                {
                    s.Write(key + "\r\n");
                }
                s.Close();
            }
            catch (Exception ex)
            {
                _log += ex.ToString();
                //MessageBox.Show(ex.Message);
            }
            //}

            /*
            if (checkBoxQuotesGenerateDateExceptions.Checked == true)
            {
                try
                {
                    FileStream fs = new FileStream(_pathMarkets + marketname + "\\" + "!EXCEPTIONS_DATES.txt", FileMode.Append, FileAccess.Write);
                    StreamWriter s = new StreamWriter(fs);
                    ArrayList aKeys = new ArrayList(_marketDateExceptions.Keys);
                    aKeys.Sort();
                    int index = 2;
                    foreach (DateTime key in aKeys)
                    {
                        if ((int)_marketDateExceptions[key] >= index)
                        {
                            s.Write(key.ToShortDateString() + "\r\n");
                            index = (int)_marketDateExceptions[key];
                        }
                    }
                    s.Close();
                }
                catch (Exception ex)
                {
                    _log += ex.ToString());
                    //MessageBox.Show(ex.Message);
                }
                try
                {
                    FileStream fs = new FileStream(_pathMarkets + marketname + "\\" + "!DATE-RANGE.txt", FileMode.Append, FileAccess.Write);
                    StreamWriter s = new StreamWriter(fs);
                    ArrayList aKeys = new ArrayList(_lastDataDate.Keys);
                    aKeys.Sort();
                    foreach (DateTime key in aKeys)
                    {
                        s.Write(key.ToShortDateString() + "\r\n");
                    }
                    s.Close();
                }
                catch (Exception ex)
                {
                    _log += ex.ToString());
                    //MessageBox.Show(ex.Message);
                }
            }
             */
            if (File.Exists(directory + "*.xxx"))
                File.Delete(directory + "*.xxx");
            _log += DateTime.Now + " : Pass completed\r\n";
            StreamWriter sw = new StreamWriter(new FileStream(_pathLogs + "yahoo.log", FileMode.OpenOrCreate, FileAccess.Write));
            sw.Write(_log);
            sw.Close();
            return _log;
        }
        private void ParseIndustry(string symbol)
        {
            string url = string.Format("http://finance.yahoo.com/q/in?s={0}", symbol);
            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
            string tempString = null;
            int count = 0;
            do
            {
                count = resStream.Read(buf, 0, buf.Length);
                if (count != 0)
                {
                    tempString = Encoding.ASCII.GetString(buf, 0, count);
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?
            string html = sb.ToString();
            Regex pattern = new Regex("yfnc_tablehead1.>Industry:</td><td.*?html.>(?<variable>.*?)</a></td></tr>");
            Match match = pattern.Match(html);
            string parsed_str = match.Groups["variable"].Value;
            _log += "Industry: " + parsed_str;
            pattern = new Regex("yfnc_tablehead1.>Sector:</td><td.*?html.>(?<variable>.*?)</a></td></tr>");
            match = pattern.Match(html);
            parsed_str = match.Groups["variable"].Value;
            _log += "Sector: " + parsed_str;
        }
        private void ParseFundamentals(string symbol)
        {
            string url = string.Format("http://finance.yahoo.com/q/ks?s={0}", symbol);
            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
            string tempString = null;
            int count = 0;
            do
            {
                count = resStream.Read(buf, 0, buf.Length);
                if (count != 0)
                {
                    tempString = Encoding.ASCII.GetString(buf, 0, count);
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?
            string html = sb.ToString();
            Regex pattern = new Regex(">Market Cap (.*?):</td><td class=.yfnc_tabledata1.><span id=.yfs_j10_goog.>(?<variable>.*?)</span></td></tr><tr>");
            Match match = pattern.Match(html);
            string parsed_str = match.Groups["variable"].Value;
            _log += "Market Cap: " + parsed_str;
            pattern = new Regex(">Enterprise Value (.*?)<font size=.-1.><sup>3</sup></font>:</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
            match = pattern.Match(html);
            parsed_str = match.Groups["variable"].Value;
            _log += "Enterprise Value: " + parsed_str;
            pattern = new Regex(">Trailing P/E (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
            match = pattern.Match(html);
            parsed_str = match.Groups["variable"].Value;
            _log += "Trailing P/E: " + parsed_str;
            pattern = new Regex(">Forward P/E (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
            match = pattern.Match(html);
            parsed_str = match.Groups["variable"].Value;
            _log += "Forward P/E: " + parsed_str;
            pattern = new Regex(">PEG Ratio (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
            match = pattern.Match(html);
            parsed_str = match.Groups["variable"].Value;
            _log += "PEG Ratio: " + parsed_str;
            pattern = new Regex(">Price/Sales (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
            match = pattern.Match(html);
            parsed_str = match.Groups["variable"].Value;
            _log += "Price/Sales: " + parsed_str;
            pattern = new Regex(">Price/Book (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
            match = pattern.Match(html);
            parsed_str = match.Groups["variable"].Value;
            _log += "Price/Book: " + parsed_str;
            pattern = new Regex(">Enterprise Value/Revenue (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
            match = pattern.Match(html);
            parsed_str = match.Groups["variable"].Value;
            _log += "Enterprise Value/Revenue: " + parsed_str;
            pattern = new Regex(">Enterprise Value/EBITDA (.*?):</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
            match = pattern.Match(html);
            parsed_str = match.Groups["variable"].Value;
            _log += "Enterprise Value/EBITDA: " + parsed_str;
            pattern = new Regex(">Beta:</td><td class=.yfnc_tabledata1.>(?<variable>.*?)</td>");
            match = pattern.Match(html);
            parsed_str = match.Groups["variable"].Value;
            _log += "Beta: " + parsed_str;

        }
        private void MarketSQLImport()
        {
            SqlConnection sqlConn = new SqlConnection("Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\MarketBot.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True");
            sqlConn.Open();
            string CreateTableSQL = "CREATE TABLE " + "market_NASDAQ" + " (INSTRUMENT text NOT NULL, DATE datetime NOT NULL, \"OPEN\" money NOT NULL, HIGH money NOT NULL, LOW money NOT NULL, \"CLOSE\" money NOT NULL, VOLUME int NOT NULL, ADJUSTED money NOT NULL)";
            SqlCommand cmdTable = new SqlCommand(CreateTableSQL, sqlConn);
            cmdTable.ExecuteNonQuery();
            Market market = new Market();
            market.Load(Directory.GetCurrentDirectory() + "\\markets\\", "nasdaq");
            for (int i = 0; i < market._instruments.Count; i++)
            {
                string instrument = market._instruments[i].ToString();
                System.IO.StreamReader file_reader = null;
                System.IO.StreamReader buffered_reader = null;
                if (File.Exists(Directory.GetCurrentDirectory() + "\\markets\\nasdaq\\" + instrument + ".csv"))
                {
                    _log += DateTime.Now + " : Importing " + instrument + "\r\n";
                    try
                    {
                        file_reader = new System.IO.StreamReader(Directory.GetCurrentDirectory() + "\\markets\\nasdaq\\" + instrument + ".csv", System.Text.Encoding.Default);
                        buffered_reader = new System.IO.StreamReader(file_reader.BaseStream, file_reader.CurrentEncoding);
                        string line = null;
                        buffered_reader.ReadLine();
                        System.Collections.ArrayList _temp = new System.Collections.ArrayList();
                        SqlCommand cmdRow = new SqlCommand();
                        cmdRow.Connection = sqlConn;
                        while ((line = buffered_reader.ReadLine()) != null)
                        {
                            string[] dataArray = new string[7];
                            Regex rex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                            dataArray = rex.Split(line);
                            DateTime d = DateTime.Parse(dataArray[0]);
                            cmdRow.CommandText = "INSERT INTO " + "market_NASDAQ" + " VALUES('" + instrument + "','" + d.Month.ToString() + "-" + d.Day.ToString() + "-" + d.Year.ToString() + "'," + dataArray[1] + "," + dataArray[2] + "," + dataArray[3] + "," + dataArray[4] + "," + dataArray[5] + "," + dataArray[6] + ")";
                            cmdRow.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        _log += ex.ToString();
                        //MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        buffered_reader.Close();
                        file_reader.Close();
                    }
                }
            }
            sqlConn.Close();
        }
        #endregion

        #region OpenTick
        // NOTE: You must run VS.NET 2005 if you wish to use GZipStream.
        // Please use the standard GetData and GetSymbols functions if 
        // you do not have VS.NET 2005.		
        public byte[] Decompress(byte[] body)
        {
            MemoryStream input = new MemoryStream(body);
            input.Position = 0;
            GZipStream gzip = new GZipStream(input, CompressionMode.Decompress, true);
            MemoryStream output = new MemoryStream();
            byte[] buff = new byte[64];
            int read = -1;
            read = gzip.Read(buff, 0, buff.Length);
            while (read > 0)
            {
                output.Write(buff, 0, read);
                read = gzip.Read(buff, 0, buff.Length);
            }
            gzip.Close();
            return output.ToArray();
        }
        public void Demo()
        {
            _log += "Run client...\r\n";
            //_log += "Run client...");

            _log += "Add hosts...\r\n";
            //_log += "Add hosts...");

            if (client.addHost("feed1.opentick.com", 10010))
            {
                string user = "";
                string pwd = "";

                //while (user == "")
                //{
                //Console.Write("Please, enter your login: ");
                //user = Console.ReadLine();
                //}

                //Console.Write("Please, enter your password: ");
                //pwd = Console.ReadLine();

                user = UserID2;
                pwd = Password2;

                client.login(user, pwd);
            }

            //Console.ReadKey();
            //client.logout();

        }
        private void onLogin()
        {
            _log += "I see, you was logged in at " + DateTime.Now.TimeOfDay + "\r\n";
            //_log += "I see, you was logged in at " + DateTime.Now.TimeOfDay);
            /*_log += "Requesting market depth.");
            _log += "ReqstId=" + client.requestMarketDepth(new OTDataEntity("ec", "#ES")));
            _log += "ReqstId=" + client.requestMarketDepth(new OTDataEntity("is", "EAGL")));
            _log += "ReqstId=" + client.requestMarketDepth(new OTDataEntity("Q", "AAPL")));/**/
            /*_log += "Requesting tick streams.");
            _log += "ReqstId=" + client.requestTickStream(new OTDataEntity("Q", "$COMPX")));            
            _log += "ReqstId=" + client.requestTickStream(new OTDataEntity("", "GOOG")));
            _log += "ReqstId=" + client.requestTickStream(new OTDataEntity("@", "DELL")));/**/
            /*_log += "Requesting extended tick streams.");
            _log += "ReqstId=" + client.requestTickStream(new OTDataEntity("Q", "IBM"), OTTickType.Level1));
            _log += "ReqstId=" + client.requestTickStream(new OTDataEntity("Q", "CSCO"), OTTickType.Level1));
            _log += "ReqstId=" + client.requestTickStream(new OTDataEntity("Q", "YHOO"), OTTickType.Level1));           
            _log += "ReqstId=" + client.requestTickStream(new OTDataEntity("", "KO"), OTTickType.Level2));/**/
            /*_log += "Requesting book streams.");          
            _log += "ReqstId=" + client.requestBookStream(new OTDataEntity("is", "EASI")));
            _log += "ReqstId=" + client.requestBookStream(new OTDataEntity("is", "ENTG")));/**/
            _log += "ReqstId=" + client.requestListExchanges() + "\r\n";
            //_log += "ReqstId=" + client.requestListExchanges());
            //_log += "ReqstId=" + client.requestListSymbols("N"));
            /*_log += "Requesting Historical data.");
            _log += "ReqstId=" + client.requestHistTicks(new OTDataEntity("HT", "/6EH5"),
                                                                 new DateTime(2004, 11, 25, 9, 0, 0, 0, DateTimeKind.Utc),
                                                                 new DateTime(2004, 11, 25, 10, 0, 0, 0, DateTimeKind.Utc),
                                                                 OTTickType.Trade));
            _log += "ReqstId=" + client.requestHistData(new OTDataEntity("Q", "MSFT"),
                                                                  new DateTime(2006, 3, 17, 14, 30, 0, 0, DateTimeKind.Utc),
                                                                  new DateTime(2006, 3, 17, 15, 30, 0, 0, DateTimeKind.Utc),
                                                                  OTHistoricalType.OhlcMinutely, 1));/**/
        }

        private void onStatusChange(Int32 status)
        {
            _log += "New status is : " + status;
        }

        private void onTrade(OTTrade trade)
        {
            string flags = trade.isUpdateLast ? "U" : ".";
            flags += trade.isOpen ? "O" : ".";
            flags += trade.isHigh ? "H" : ".";
            flags += trade.isLow ? "L" : ".";
            flags += trade.isClose ? "C" : ".";

            OTDataEntity dataEntity = client.getEntityById(trade.RequestId);
            _log += "t:" + dataEntity.Exchange + "/" + dataEntity.Symbol +
                              " " + trade.ExchangeTime.ToLocalTime() +
                              " size=" + trade.Size +
                              " price=" + trade.Price +
                              " volume=" + trade.Volume +
                              " flags=" + flags +
                              " indicator=" + trade.Indicator +
                              " serial=" + trade.SequenceNumber
                              + " tickIndicator=" + trade.TickIndicator
                              + " exchange=" + trade.Exchange
                              + " now is " + DateTime.Now.TimeOfDay;
        }
        private void onBBO(OTBBO bbo)
        {
            OTDataEntity dataEntity = client.getEntityById(bbo.RequestId);
            Console.Write("b:" + dataEntity.Exchange + "/" + dataEntity.Symbol +
                              " " + bbo.ExchangeTime.ToLocalTime() +
                              " price=" + bbo.Price +
                              " size=" + bbo.Size +
                              " side=" + bbo.Side +
                              " exchange=" + bbo.Exchange +
                              " now is " + DateTime.Now.TimeOfDay/**/
                                                                   );
            if (bbo.SymbolCode != null)
            {
                _log += " symcode=" + bbo.SymbolCode;
            }
            else
            {
                _log += "";
            }
        }
        private void onMMQuote(OTMMQuote mmquote)
        {
            OTDataEntity dataEntity = client.getEntityById(mmquote.RequestId);
            Console.Write("m:" + dataEntity.Exchange + "/" + dataEntity.Symbol +
                          " " + mmquote.ExchangeTime.ToLocalTime() +
                          " askPrice=" + mmquote.AskPrice +
                          " askSize=" + mmquote.AskSize +
                          " bidPrice=" + mmquote.BidPrice +
                          " bidSize=" + mmquote.BidSize +
                          " MMID=" + mmquote.MMID +
                          " Ind=" + mmquote.Indicator +
                          " exchange=" + mmquote.Exchange +
                          " now is " + DateTime.Now.TimeOfDay/**/
                                                                   );
            if (mmquote.SymbolCode != null)
            {
                _log += " symcode=" + mmquote.SymbolCode;
            }
            else
            {
                _log += "";
            }
        }
        private void onQuote(OTQuote quote)
        {
            OTDataEntity dataEntity = client.getEntityById(quote.RequestId);
            Console.Write("q:" + dataEntity.Exchange + "/" + dataEntity.Symbol +
                          " " + quote.ExchangeTime.ToLocalTime() +
                          " askPrice=" + quote.AskPrice +
                          " askSize=" + quote.AskSize +
                          " askExchange=" + quote.AskExchange +
                          " bidPrice=" + quote.BidPrice +
                          " bidSize=" + quote.BidSize +
                          " Ind=" + quote.Indicator +
                          " TInd=" + quote.TickIndicator +
                          " exchange=" + quote.Exchange +
                          " BidExchange=" + quote.BidExchange +
                          " now is " + DateTime.Now.TimeOfDay/**/
                                                                   );
            if (quote.SymbolCode != null && quote.SymbolCode != "")
            {
                _log += " symcode=" + quote.SymbolCode;
            }
            else
            {
                _log += "";
            }
        }
        private void onError(OTError error)
        {
            _log += "error: reqId=" + error.RequestId +
                              " type=" + error.Type +
                              " code=" + error.Code +
                              " description: " + error.Description +
                              " now is " + DateTime.Now.TimeOfDay;
        }
        private void onMessage(OTMessage message)
        {
            _log += "message: reqId=" + message.RequestId +
                               " code=" + message.Code +
                               " description: " + message.Description +
                              " now is " + DateTime.Now.TimeOfDay;
            _log += "";
        }
        private void onRestoreConnection()
        {
            _log += "Restore connection";
        }
        private void onEquityInit(OTEquityInit equityInit)
        {
            _log += "EqInit: reqId=" + equityInit.RequestId +
                              " cname=" + equityInit.CompanyName +
                              " prevDate=" + equityInit.PrevCloseDate +
                              " prevPrice=" + equityInit.PrevClosePrice +
                              " annualHD=" + equityInit.AnnualHighDate +
                              " annualHP=" + equityInit.AnnualHighPrice +
                              " annualLD=" + equityInit.AnnualLowDate +
                              " annualLP=" + equityInit.AnnualLowPrice +
                              " EarningsDate=" + equityInit.EarningsDate +
                              " EarningsPrice=" + equityInit.EarningsPrice +
                              " AverageVolume=" + equityInit.AverageVolume +
                              " TotalShares=" + equityInit.TotalShares +
                              " InstrumentType=" + equityInit.InstrumentType +
                              " ISIN=" + equityInit.ISIN +
                              " CUSIP=" + equityInit.CUSIP +
                              " IsSmallCap=" + equityInit.IsSmallCap +
                              " IsUPC11830=" + equityInit.IsUPC11830 +
                              " IsTestlssue=" + equityInit.IsTestlssue;
        }
        private void onHistoricalOHLC(OTOHLC ohlc)
        {
            _log += "OHLC: reqId=" + ohlc.RequestId +
                               " time=" + ohlc.Timestamp.ToLocalTime() +
                               " open=" + ohlc.OpenPrice +
                               " high=" + ohlc.HighPrice +
                               " low=" + ohlc.LowPrice +
                               " close=" + ohlc.ClosePrice +
                               " volume=" + ohlc.Volume +
                              " now is " + DateTime.Now.TimeOfDay;
        }
        private void onTodaysOHL(int requestId, double openPrice, double highPrice, double lowPrice)
        {
            _log += "TodaysOHL: reqId=" + requestId +
                              " open=" + openPrice +
                              " high=" + highPrice +
                              " low=" + lowPrice;
        }
        private void onListSymbols(OTSymbol[] symbols)
        {
            _log += "Symbols count=" + symbols.Length;
            for (int i = 0; i < symbols.Length; i++)
            {
                _log += "listSymbols: reqId=" + symbols[i].RequestId +
                                  " cur=" + symbols[i].Currency +
                                  " code=" + symbols[i].Code +
                                  " InstrType=" + symbols[i].Type +
                                  " Company=" + symbols[i].Company +
                                  " now is " + DateTime.Now.TimeOfDay;
            }
        }
        private void onListExchanges(OTExchange[] exchanges)
        {
            DataTable dt = new DataTable("exchange");
            dt.Columns.Add("code", typeof(string));
            dt.Columns.Add("available", typeof(bool));
            dt.Columns.Add("title", typeof(string));
            dt.Columns.Add("description", typeof(string));
            for (int i = 0; i < exchanges.Length; i++)
            {
                DataRow dr = dt.NewRow();
                dr["code"] = exchanges[i].Code;
                dr["available"] = exchanges[i].Available;
                dr["title"] = exchanges[i].Title;
                dr["description"] = exchanges[i].Description;
                dt.Rows.Add(dr);
                //textBox1.AppendText("\r\nlistExchanges: reqId=" + exchanges[i].RequestId + " SubscriptionURL=" + exchanges[i].SubscriptionURL + " code=" + exchanges[i].Code + " avail=" + exchanges[i].Available + " title=" + exchanges[i].Title + " description=" + exchanges[i].Description + "\r\n");
            }
            //File.Delete(file);
            FileStream fout = new FileStream(_pathWork + "exchanges.xml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            dt.WriteXml(fout);
            fout.Close();
            _log += "Exported exchange list\r\n";
        }
        private void onBookReplace(OTBookReplace book)
        {
            _log += "bookReplace: reqId=" + book.RequestId +
                               " time=" + book.Timestamp.TimeOfDay +
                               " orderRef" + book.OrderRef +
                               " price=" + book.Price +
                               " size=" + book.Size +
                               " side=" + book.Side;
        }
        private void onBookPurge(OTBookPurge book)
        {
            _log += "bookPurge: reqId=" + book.RequestId +
                               " time=" + book.Timestamp.TimeOfDay +
                               " exchangeNameRoot=" + book.ExchangeNameRoot;
        }
        private void onBookPriceLevel(OTBookPriceLevel book)
        {
            _log += "bookPriceLevel: reqId=" + book.RequestId +
                               " time=" + book.Timestamp.TimeOfDay +
                               " price=" + book.Price +
                               " size=" + book.Size +
                               " side=" + book.Side +
                               " levelid=" + book.LevelId;
        }
        private void onBookOrder(OTBookOrder book)
        {
            _log += "bookOrder: reqId=" + book.RequestId +
                               " time=" + book.Timestamp.TimeOfDay +
                               " orderRef" + book.OrderRef +
                               " price=" + book.Price +
                               " size=" + book.Size +
                               " side=" + book.Side +
                               " display=" + book.Display;
        }
        private void onBookExecute(OTBookExecute book)
        {
            _log += "bookExecute: reqId=" + book.RequestId +
                               " time=" + book.Timestamp.TimeOfDay +
                               " orderRef" + book.OrderRef +
                               " size=" + book.Size +
                               " matchNumber=" + book.MatchNumber;
        }
        private void onBookDelete(OTBookDelete book)
        {
            _log += "bookDelete: reqId=" + book.RequestId +
                               " time=" + book.Timestamp.TimeOfDay +
                               " orderRef" + book.OrderRef +
                               " side=" + book.Side +
                               " deleteType=" + book.DeleteType;
        }
        private void onBookChange(OTBookChange book)
        {
            _log += "bookChange: reqId=" + book.RequestId +
                               " time=" + book.Timestamp.TimeOfDay +
                               " orderRef" + book.OrderRef +
                               " price=" + book.Price +
                               " size=" + book.Size;
        }
        private void onBookCancel(OTBookCancel book)
        {
            _log += "bookCancel: reqId=" + book.RequestId +
                               " time=" + book.Timestamp.TimeOfDay +
                               " orderRef" + book.OrderRef +
                               " size=" + book.Size;
        }
        #endregion
    }
}
