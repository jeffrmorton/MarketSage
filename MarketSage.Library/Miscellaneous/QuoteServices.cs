using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections;
using System.IO;
using System.Net;

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
    /// Yahoo! Finance Quote Services object
    /// </summary>
    public class QuoteServices
    {
        private string _directoryLogs;
        private string _fileErrorLog = "errors.log";

        /// <summary>
        /// QuoteServices constructor
        /// </summary>
        /// <param name="directoryLogs">string</param>
        public QuoteServices(string directoryLogs)
        {
            _directoryLogs = directoryLogs;
        }

        protected string[] GetNumbers(string symbol)
        {
            string ret = "";
            try
            {
                // The Path to the Yahoo Quotes Service
                string fullpath = @"http://quote.yahoo.com/d/quotes.csv?s=" + symbol + "&f=sl1d1t1c1ohgvj1pp2owern&e=.csv";
                // Create a HttpWebRequest object on the Yahoo url
                HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(fullpath);

                // Get a HttpWebResponse object from the Yahoo url
                HttpWebResponse webresp = (HttpWebResponse)webreq.GetResponse();

                // Create a StreamReader object and pass the Yahoo Server stream as a parameter
                StreamReader strm = new StreamReader(webresp.GetResponseStream(), Encoding.ASCII);

                // Read a single line from the stream (from the server) 
                // We read only a single line, since the Yahoo server returns all the
                // information needed by us in just one line.
                ret = strm.ReadLine();

                // Close the stream to the server and free the resources.
                strm.Close();

                char[] splitter = { ',' };

                string[] returnArray = ret.Split(splitter);
                returnArray[2] = returnArray[2].Replace("\"", "");  // fix the date
                returnArray[3] = returnArray[3].Replace("\"", "");  // fix the time 
                //Return the Quote or Exception
                return returnArray;
            }
            catch (Exception ex)
            {
                PostToLog(DateTime.Now + " : " + ex + "\r\n", _directoryLogs + _fileErrorLog);
                return null;
            }
        }

        public string[] GetStockInfoArray(string symbol)
        {
            return GetNumbers(symbol);
        }

        public string GetPrice(string symbol)
        {
            string[] values = GetNumbers(symbol);
            return values[1];
        }

        public string GetQuoteDate(string symbol)
        {
            string[] values = GetNumbers(symbol);
            return values[2];
        }

        public string GetQuote(string symbol)
        {
            string stockURL, page, retval;
            try
            {
                stockURL = GetURL(symbol);
                page = GetPageContent(stockURL);
                retval = ParsePage(page);
            }
            catch (ArgumentOutOfRangeException)
            {
                retval = "Invalid symbol";
            }
            catch (Exception)
            {
                retval = "Unknown error";
            }

            return retval;
        }
        /*
        public string[] GetQuote(string symbol)
        {
            string url;  //stores url of yahoo quote engine
            string buffer;
            string[] bufferList;
            WebRequest webRequest;
            WebResponse webResponse;

            url = "http://quote.yahoo.com/d/quotes.csv?s=" + symbol + "&d=t&f=sl1d1t1c1ohgvj1pp2wern";

            webRequest = HttpWebRequest.Create(url);
            webResponse = webRequest.GetResponse();
            using (StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
            {
                buffer = sr.ReadToEnd();
                sr.Close();
            }

            buffer = buffer.Replace("\"", "");
            bufferList = buffer.Split(new char[] { ',' });
            return bufferList;
        }
         */

        public DataSet GetQuotes(string symbols)
        {
            char[] splitter = { ' ' };
            string[] __Symbols = symbols.Trim().Split(splitter);
            Int32 i, ticks;

            ticks = __Symbols.Length;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable("StockData");
            DataColumn dc;

            dc = dt.Columns.Add("_Symbol", System.Type.GetType("string"));
            dc = dt.Columns.Add("Price", System.Type.GetType("string"));

            for (i = 0; i < ticks; i++)
            {
                DataRow dr = dt.NewRow();
                dr["_Symbol"] = __Symbols[i].ToUpper();
                dr["Price"] = GetQuote(__Symbols[i]);
                dt.Rows.Add(dr);
            }

            ds.Tables.Add(dt);
            return ds;
        }

        private string GetURL(string symbol)
        {
            StringBuilder url = new StringBuilder();

            url.Append("http://finance.yahoo.com/q/ecn?s=");
            url.Append(symbol);

            return url.ToString();
        }

        private string ParsePage(string page)
        {
            Int32 i;

            i = page.IndexOf("Last Trade:");
            page = page.Substring(i);

            i = page.IndexOf("<b>");
            page = page.Substring(i);

            i = page.IndexOf("</b>");
            page = page.Substring(0, i);

            page = Regex.Replace(page, "<b>", "");
            return page;
        }

        public string GetPageContent(string url)
        {
            WebRequest wreq;
            WebResponse wres;
            StreamReader sr;
            String content;

            wreq = HttpWebRequest.Create(url);
            wres = wreq.GetResponse();
            sr = new StreamReader(wres.GetResponseStream());
            content = sr.ReadToEnd();
            sr.Close();

            return content;
        }

        /// <summary>
        /// Writes text to file
        /// </summary>
        /// <param name="textstring">Text to write</param>
        /// <param name="filename">Name of file</param>
        private void PostToLog(string textstring, string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
            StreamWriter s = new StreamWriter(fs);
            s.Write(textstring);
            s.Close();
        }
    }
}
