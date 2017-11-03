using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace GetCompanyInfoTest
{
    class Program
    {
        static void Main(string[] args)
        {
            HtmlWeb web;
            HtmlDocument doc;
            HtmlNodeCollection nodes;
            string xp_title = string.Empty;

            web = new HtmlWeb();
            string url = "http://mops.twse.com.tw/server-java/t164sb01?step=1&CO_ID=2330&SYEAR=2015&SSEASON=4&REPORT_ID=C/";
            string url2 = @"http://mops.twse.com.tw/server-java/t164sb01";
            //string url3 = "http://mops.twse.com.tw/server-java/t164sb01?step=1&CO_ID=2330&SYEAR=2015&SSEASON=4&REPORT_ID=C";
            var url4 = "http://html-agility-pack.net/";
            var url5 = "http://mops.twse.com.tw/";
            var url6 = "http://mops.twse.com.tw/mops/web/ajax_t163sb15?TYPEK=all&step=1&firstin=1&off=1&queryName=co_id&t05st29_c_ifrs=N&t05st30_c_ifrs=N&isnew=false&co_id=6166&year=106";
            var url7 = "http://www.twse.com.tw/exchangeReport/STOCK_DAY?response=.csv&date=20171101&stockNo=6166";
            doc = web.Load(url6);

            //xp_title = "/tr/td";
            //nodes = doc.DocumentNode.SelectNodes(xp_title);
            //nodes = doc.DocumentNode.SelectNodes("//th/td");

            //doc = web.Load("http://www.google.com/search?hl=en&q=xpath&oq=XPath");

            xp_title = @"//h3[@class=""r""]";
            //var xp_title2 = @"//div[@class=""f kv_SWb""]";
            xp_title = @"html";


            nodes = doc.DocumentNode.SelectNodes("/html/head/title");

            //抓出財報的項目名稱
            var nodes2 = doc.DocumentNode.SelectNodes("//tr/th[@class!=\"tblHead\"]");

            //抓出第二季的資料
            var nodes3 = doc.DocumentNode.SelectNodes("//td[@class!=\"reportCont\"][2]");

            var numbersAndWords = nodes2.Zip(nodes3, (w, n) => new { Word = w, Number = n });
            foreach (var nw in numbersAndWords)
            {
                Console.WriteLine(nw.Word.InnerText + ":" + nw.Number.InnerText);
            }

            Console.WriteLine("node2 count: " + nodes2.Count);

            Console.WriteLine("node3 count: " + nodes3.Count);

            //GetJsonData();

            //Console.Read();

            while (true)
            {
                Console.WriteLine("輸入股票代號及季別:XXXXyyyyQX");

                string input = Console.ReadLine();

                var stockId = input.Substring(0, 4);

                var year = input.Substring(4).Substring(0, 4);

                var season = input.Substring(4).Substring(4);

                if (input == "")
                {
                    break;
                }
                else if(Convert.ToInt32(year) >= 2013)
                {
                    GetCompanyFinanceStat(input.Substring(0, 4), input.Substring(4));
                }
                else if(Convert.ToInt32(year) < 2013)
                {
                    GetCompanyFinanceStatBeforeIfrs(input.Substring(0, 4), input.Substring(4));
                }


                //GetCompanyFinanceStat(input.Substring(0, 4), input.Substring(4));

                //Console.WriteLine(string.Format("stock id:{0} season:{1} 股價:", input.Substring(0,4), input.Substring(4)) + GetStockPrice(input.Substring(0, 4), input.Substring(4)));
            }
        }

        /// <summary>
        /// 取得ifrs前的財務報表
        /// </summary>
        /// <param name="stockId">股票代號(Ex:6616)</param>
        /// <param name="date">年季別(Ex:2017Q4)</param>
        private static void GetCompanyFinanceStatBeforeIfrs(string stockId, string date)
        {
            try
            {
                Company company = new Company();

                //資產負債表
                string apiBSUrlIfrs = "http://mops.twse.com.tw/mops/web/ajax_t05st29?TYPEK=all&step=1&firstin=1&off=1&queryName=co_id&isnew=false";

                //損益表
                string apiISUrlIfrs = "http://mops.twse.com.tw/mops/web/ajax_t05st30_c?TYPEK=all&step=1&firstin=1&off=1&queryName=co_id&t05st29_c_ifrs=N&t05st31_c_ifrs=N&isnew=false";

                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc;
                //HtmlNodeCollection nodes;

                apiBSUrlIfrs += "&co_id=" + stockId + "&year=" + (Convert.ToInt32(date.Substring(0, 4)) - 1911).ToString();

                apiISUrlIfrs += "&co_id=" + stockId + "&year=" + (Convert.ToInt32(date.Substring(0, 4)) - 1911).ToString();

                //資產負債表
                doc = web.Load(apiBSUrlIfrs);

                //項目名稱
                var nodeHead = doc.DocumentNode.SelectNodes("//tr[@class!=\"reportCont\"]/td[1]");
                //取得第四季資料
                string dataPath = "//tr[@class!=\"reportCont\"]/td[5]";
                //季別資料
                var nodeData = doc.DocumentNode.SelectNodes(dataPath);

                Dictionary<string, string> BSData = new Dictionary<string, string>();

                var numbersAndWords = nodeHead.Zip(nodeData, (w, n) => new { Word = w, Number = n });
                foreach (var nw in numbersAndWords)
                {
                    BSData.Add(nw.Word.InnerText, nw.Number.InnerText);

                    //Console.WriteLine(nw.Word.InnerText + ":" + nw.Number.InnerText);
                }

                //損益表
                doc = web.Load(apiISUrlIfrs);

                //項目名稱
                nodeHead = doc.DocumentNode.SelectNodes("//tr[@class!=\"reportCont\"]/td[1]");
                //path設定為日期所指定的季別
                dataPath = "//tr[@class!=\"reportCont\"]/td[5]";
                //季別資料
                nodeData = doc.DocumentNode.SelectNodes(dataPath);


                Dictionary<string, string> ISData = new Dictionary<string, string>();

                numbersAndWords = nodeHead.Zip(nodeData, (w, n) => new { Word = w, Number = n });
                foreach (var nw in numbersAndWords)
                {
                    ISData.Add(nw.Word.InnerText, nw.Number.InnerText);
                    //Console.WriteLine(nw.Word.InnerText + ":" + nw.Number.InnerText);
                }

                //var aaa = Convert.ToInt32(BSData["流動資產"]);

                company.Date = date;

                company.WorkingCapital = StringToInt(BSData["流動資產"]) - StringToInt(BSData["流動負債"]);

                company.RetainedEarning = StringToInt(BSData["保留盈餘"]);

                company.EBIT = StringToInt(ISData["營業收入"]) - StringToInt(ISData["營業成本"]) - StringToInt(ISData["營業費用"]);

                company.TotalAsset = StringToInt(BSData["資產總計"]);

                company.TotalLiability = StringToInt(BSData["負債總計"]);

                company.GrossSales = StringToInt(ISData["營業收入"]);

                company.MarketValue = StringToInt(BSData["股本"]) / 10 * Convert.ToDouble(GetStockPrice(stockId, date));

                company.CompanyStock = StringToInt(BSData["股本"]);

                company.StockPrice = Convert.ToDouble(GetStockPrice(stockId, date));

                company.ZValue = GetZValue(company.WorkingCapital, company.RetainedEarning, company.EBIT, company.MarketValue, company.GrossSales, company.TotalAsset, company.TotalLiability);

                Console.WriteLine(company);
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal static WebRequest CreateWebRequest()
        {
            WebRequest myWebRequest = WebRequest.Create("http://mops.twse.com.tw/server-java/t164sb01?step=1&CO_ID=2349&SYEAR=2016&SSEASON=4&REPORT_ID=C");
            //string[] strByPass = new string[] { "192.1.1.1", "192.2.2.2" };
            myWebRequest.Timeout = 10000;
            //myWebRequest.Credentials = new NetworkCredential("Name", "PWD", "Domain Name");
            //myWebRequest.Proxy = new WebProxy(new Uri("http://proxy.hinet.net:8080"));
            //myWebRequest.Proxy.Credentials = new NetworkCredential("Name", "PWD", "Domain Name");
            return myWebRequest;
        }

        internal static void GetJsonData()
        {
            using (WebClient wc = new WebClient())
            {
                var jsonStr = wc.DownloadString("http://www.twse.com.tw/exchangeReport/STOCK_DAY?response=.csv&date=20171001&stockNo=6166");

                //JObject json = XObject.Parse(jsonStr);

                JObject o = JObject.Parse(jsonStr);

                //抓股價資料裡最後一筆的收盤價
                Console.WriteLine(o["data"].Last()[6]);

                //dynamic json = JsonConvert.DeserializeObject(jsonStr);

                //Console.WriteLine(json["data"]);


            }
        }
        /// <summary>
        /// 季的股價資訊
        /// </summary>
        /// <param name="stockId">股票代號</param>
        /// <param name="date">年季別yyyyQx(x:1~4)</param>
        /// <returns></returns>
        internal static string GetStockPrice(string stockId, string date)
        {
            using (WebClient wc = new WebClient())
            {
                string url = "http://www.twse.com.tw/exchangeReport/STOCK_DAY?response=.csv";

                url += "&date=" + date.Substring(0, 4) + ConvertSeasonToDate(date.Substring(4));

                url += "&stockNo=" + stockId.ToString();

                var jsonStr = wc.DownloadString(url);

                JObject stockDataO = JObject.Parse(jsonStr);

                return stockDataO["data"].Last()[6].ToString();
            }
        }

        internal static void GetCompanyFinanceStat(string stockId, string date)
        {
            Company company = new Company();

            //資產負債表
            string apiBSUrlIfrs = "http://mops.twse.com.tw/mops/web/ajax_t163sb16?TYPEK=all&step=1&firstin=1&off=1&queryName=co_id&t05st29_c_ifrs=N&t05st30_c_ifrs=N&isnew=false";

            //損益表
            string apiISUrlIfrs = "http://mops.twse.com.tw/mops/web/ajax_t163sb15?TYPEK=all&step=1&firstin=1&off=1&queryName=co_id&t05st29_c_ifrs=N&t05st30_c_ifrs=N&isnew=false";

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc;
            //HtmlNodeCollection nodes;

            apiBSUrlIfrs += "&co_id=" + stockId + "&year=" + (Convert.ToInt32(date.Substring(0, 4)) - 1911).ToString();

            apiISUrlIfrs += "&co_id=" + stockId + "&year=" + (Convert.ToInt32(date.Substring(0, 4)) - 1911).ToString();

            //資產負債表
            doc = web.Load(apiBSUrlIfrs);

            //項目名稱
            var nodeHead = doc.DocumentNode.SelectNodes("//tr/th[@class!=\"tblHead\"]");
            //path設定為日期所指定的季別
            string dataPath = "//td[@class!=\"reportCont\"][" + date.Substring(date.Length - 1) + "]";
            //季別資料
            var nodeData = doc.DocumentNode.SelectNodes(dataPath);

            Dictionary<string, string> BSData = new Dictionary<string, string>();

            var numbersAndWords = nodeHead.Zip(nodeData, (w, n) => new { Word = w, Number = n });
            foreach (var nw in numbersAndWords)
            {
                BSData.Add(nw.Word.InnerText, nw.Number.InnerText);

                //Console.WriteLine(nw.Word.InnerText + ":" + nw.Number.InnerText);
            }

            //損益表
            doc = web.Load(apiISUrlIfrs);

            //項目名稱
            nodeHead = doc.DocumentNode.SelectNodes("//tr/th[@class!=\"tblHead\"]");
            //path設定為日期所指定的季別
            dataPath = "//td[@class!=\"reportCont\"][" + date.Substring(date.Length - 1) + "]";
            //季別資料
            nodeData = doc.DocumentNode.SelectNodes(dataPath);


            Dictionary<string, string> ISData = new Dictionary<string, string>();

            numbersAndWords = nodeHead.Zip(nodeData, (w, n) => new { Word = w, Number = n });
            foreach (var nw in numbersAndWords)
            {
                ISData.Add(nw.Word.InnerText, nw.Number.InnerText);
                //Console.WriteLine(nw.Word.InnerText + ":" + nw.Number.InnerText);
            }

            //var aaa = Convert.ToInt32(BSData["流動資產"]);

            company.Date = date;

            company.WorkingCapital = StringToInt(BSData["流動資產"]) - StringToInt(BSData["流動負債"]);

            company.RetainedEarning = StringToInt(BSData["保留盈餘"]);

            company.EBIT = StringToInt(ISData["營業收入"]) - StringToInt(ISData["營業成本"]) - StringToInt(ISData["營業費用"]);

            company.TotalAsset = StringToInt(BSData["資產總計"]);

            company.TotalLiability = StringToInt(BSData["負債總計"]);

            company.GrossSales = StringToInt(ISData["營業收入"]);

            company.MarketValue = StringToInt(BSData["股本"]) / 10 * Convert.ToDouble(GetStockPrice(stockId, date));

            company.CompanyStock = StringToInt(BSData["股本"]);

            company.StockPrice = Convert.ToDouble(GetStockPrice(stockId, date));

            company.ZValue = GetZValue(company.WorkingCapital, company.RetainedEarning, company.EBIT, company.MarketValue, company.GrossSales, company.TotalAsset, company.TotalLiability);

            Console.WriteLine(company);

            //Console.WriteLine(string.Format("stock id:{0} season:{1} 股價:", stockId, date) + GetStockPrice(stockId, date));
        }

        /// <summary>
        /// 計算 z value
        /// </summary>
        /// <param name="wc">營運資金</param>
        /// <param name="re">保留盈餘</param>
        /// <param name="ebit">營業利益</param>
        /// <param name="mv">市值</param>
        /// <param name="gs">營業收入</param>
        /// <param name="ta">總資產</param>
        /// <param name="tl">總負債</param>
        /// <returns></returns>
        internal static double GetZValue(double wc, double re, double ebit, double mv, double gs, double ta, double tl)
        {
            return 1.2 * wc / ta + 1.4 * re / ta + 3.3 * ebit / ta + 0.6 * mv / tl + gs / ta;
        }

        /// <summary>
        /// 將季別轉換成月份
        /// </summary>
        /// <param name="season"></param>
        /// <returns></returns>
        internal static string ConvertSeasonToDate(string season)
        {
            switch (season)
            {
                case "Q1":
                    return "0301";
                case "Q2":
                    return "0601";
                case "Q3":
                    return "0901";
                case "Q4":
                    return "1201";
                default:
                    return "not valid season";
            }
        }

        internal static int StringToInt(string num)
        {
            string strNum="";
            foreach( var n in num.Split(","))
            {
                strNum += n;
            }
            return int.Parse(strNum);
        }
    }
}
