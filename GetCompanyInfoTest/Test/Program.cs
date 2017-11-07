using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                while (true)
                {
                    /*
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
                    */

                    Console.WriteLine("輸入股票代號");

                    var stockId = Console.ReadLine();

                    if (CheckValidStockId(stockId))
                    {
                        ShowCompLstZValue(GetCompanyTenStatDataLst(stockId));
                    }
                    else
                    {
                        if (stockId == "")
                        {
                            break;
                        }

                        Console.WriteLine();

                        Console.WriteLine("請輸入正確的股票代號");
                    }


                }

                Console.Read();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 取得ifrs前的財務報表
        /// </summary>
        /// <param name="stockId">股票代號(Ex:6616)</param>
        /// <param name="date">年季別(Ex:2017Q4)</param>
        private static Company GetCompanyFinanceStatBeforeIfrs(string stockId, string date)
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

                company.Ticker = stockId;

                company.Date = date;

                company.WorkingCapital = StringToInt(BSData["流動資產"]) - StringToInt(BSData["流動負債"]);

                company.RetainedEarning = StringToInt(BSData["保留盈餘"]);

                company.EBIT = StringToInt(ISData["營業收入"]) - StringToInt(ISData["營業成本"]) - StringToInt(ISData["營業費用"]);

                //有些是 資產總額 or 資產總計
                //company.TotalAsset = StringToInt(BSData["資產總計"]);
                if (BSData.ContainsKey("資產總額"))
                {
                    company.TotalAsset = StringToInt(BSData["資產總額"]);
                }
                else if (BSData.ContainsKey("資產總計"))
                {
                    company.TotalAsset = StringToInt(BSData["資產總計"]);
                }
                else
                {
                    company.TotalAsset = StringToInt("0");
                }

                //有些是 負債總額 or 負債總計
                //company.TotalLiability = StringToInt(BSData["負債總計"]);
                if (BSData.ContainsKey("負債總額"))
                {
                    company.TotalLiability = StringToInt(BSData["負債總額"]);
                }
                else if (BSData.ContainsKey("負債總計"))
                {
                    company.TotalLiability = StringToInt(BSData["負債總計"]);
                }
                else
                {
                    company.TotalLiability = StringToInt("0");
                }

                company.GrossSales = StringToInt(ISData["營業收入"]);

                company.MarketValue = StringToInt(BSData["股本"]) / 10 * Convert.ToDouble(GetStockPrice(stockId, date));

                company.CompanyStock = StringToInt(BSData["股本"]);

                company.StockPrice = Convert.ToDouble(GetStockPrice(stockId, date));

                company.ZValue = GetZValue(company.WorkingCapital, company.RetainedEarning, company.EBIT, company.MarketValue, company.GrossSales, company.TotalAsset, company.TotalLiability);

                return company;
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

        /// <summary>
        /// 季的股價資訊
        /// </summary>
        /// <param name="stockId">股票代號</param>
        /// <param name="date">年季別yyyyQx(x:1~4)</param>
        /// <returns></returns>
        internal static string GetStockPrice(string stockId, string date)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    //不要在短時間請求網頁
                    Thread.Sleep(1);

                    string url = "http://www.twse.com.tw/exchangeReport/STOCK_DAY?response=.csv";

                    url += "&date=" + date.Substring(0, 4) + ConvertSeasonToDate(date.Substring(4));

                    url += "&stockNo=" + stockId.ToString();

                    wc.Encoding = Encoding.UTF8;

                    var jsonStr = wc.DownloadString(url);

                    JObject stockDataO = JObject.Parse(jsonStr);

                    //沒有撈到資料(網站更新時會沒有資料)
                    if (stockDataO["stat"].ToString() != "OK")
                    {
                        return "0";
                    }

                    return stockDataO["data"].Last()[6].ToString();
                }
            }
            catch (Exception)
            {
                return "0";

                throw;
            }
        }

        internal static Company GetCompanyFinanceStat(string stockId, string date)

        {
            try
            {
                Company company = new Company();

                //資產負債表
                string apiBSUrlIfrs = "http://mops.twse.com.tw/mops/web/ajax_t163sb16?TYPEK=all&step=1&firstin=1&off=1&queryName=co_id&t05st29_c_ifrs=N&t05st30_c_ifrs=N&isnew=false";

                //損益表
                string apiISUrlIfrs = "http://mops.twse.com.tw/mops/web/ajax_t163sb15?TYPEK=all&step=1&firstin=1&off=1&queryName=co_id&t05st29_c_ifrs=N&t05st30_c_ifrs=N&isnew=false";

                HtmlWeb web = new HtmlWeb();
                WebClient webC = new WebClient();
                //webC.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1";
                //webC.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                HtmlDocument doc = new HtmlDocument();
                //HtmlNodeCollection nodes;

                apiBSUrlIfrs += "&co_id=" + stockId + "&year=" + (Convert.ToInt32(date.Substring(0, 4)) - 1911).ToString();

                apiISUrlIfrs += "&co_id=" + stockId + "&year=" + (Convert.ToInt32(date.Substring(0, 4)) - 1911).ToString();

                //資產負債表
                doc = web.Load(apiBSUrlIfrs);

                //doc.Load(webC.OpenRead(apiBSUrlIfrs));


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

                company.Ticker = stockId;

                company.Date = date;

                company.WorkingCapital = StringToInt(BSData["流動資產"]) - StringToInt(BSData["流動負債"]);

                company.RetainedEarning = StringToInt(BSData["保留盈餘"]);

                company.EBIT = StringToInt(ISData["營業收入"]) - StringToInt(ISData["營業成本"]) - StringToInt(ISData["營業費用"]);

                //有些是 資產總額 or 資產總計
                //company.TotalAsset = StringToInt(BSData["資產總計"]);
                if (BSData.ContainsKey("資產總額"))
                {
                    company.TotalAsset = StringToInt(BSData["資產總額"]);
                }
                else if (BSData.ContainsKey("資產總計"))
                {
                    company.TotalAsset = StringToInt(BSData["資產總計"]);
                }
                else
                {
                    company.TotalAsset = StringToInt("0");
                }

                //有些是 負債總額 or 負債總計
                //company.TotalLiability = StringToInt(BSData["負債總計"]);
                if (BSData.ContainsKey("負債總額"))
                {
                    company.TotalLiability = StringToInt(BSData["負債總額"]);
                }
                else if (BSData.ContainsKey("負債總計"))
                {
                    company.TotalLiability = StringToInt(BSData["負債總計"]);
                }
                else
                {
                    company.TotalLiability = StringToInt("0");
                }

                company.GrossSales = StringToInt(ISData["營業收入"]);

                company.MarketValue = StringToInt(BSData["股本"]) / 10 * Convert.ToDouble(GetStockPrice(stockId, date));

                company.CompanyStock = StringToInt(BSData["股本"]);

                company.StockPrice = Convert.ToDouble(GetStockPrice(stockId, date));

                company.ZValue = GetZValue(company.WorkingCapital, company.RetainedEarning, company.EBIT, company.MarketValue, company.GrossSales, company.TotalAsset, company.TotalLiability);

                //Console.WriteLine(company);
                return company;
                //Console.WriteLine(string.Format("stock id:{0} season:{1} 股價:", stockId, date) + GetStockPrice(stockId, date));
            }
            catch (Exception)
            {

                throw;
            }
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
            try
            {
                return 1.2 * wc / ta + 1.4 * re / ta + 3.3 * ebit / ta + 0.6 * mv / tl + gs / ta;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 將季別轉換成月份
        /// </summary>
        /// <param name="season"></param>
        /// <returns></returns>
        internal static string ConvertSeasonToDate(string season)
        {
            try
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
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// #,###,### to #######
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        internal static int StringToInt(string num)
        {
            try
            {
                if (num == "")
                {
                    return 0;
                }

                string strNum = "";
                foreach (var n in num.Split(','))
                {
                    strNum += n;
                }
                return int.Parse(strNum);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 回傳十年間的公司財報資料
        /// </summary>
        /// <param name="stockId">股市代號(Ex:6166)</param>
        /// <returns>回傳公司十年的Z值</returns>
        internal static List<Company> GetCompanyTenStatDataLst(string stockId)
        {
            try
            {

                var season = LastStatDate();

                Company temp = new Company();

                List<Company> compTenYrDataLst = new List<Company>();

                temp = GetCompanyFinanceStat(stockId, season);

                compTenYrDataLst.Add(temp);

                for (int i = 1; i < 9; i++)
                {
                    if ((Convert.ToInt32(season.Substring(0, 4)) - i) < 2013)
                    {
                        temp = GetCompanyFinanceStatBeforeIfrs(stockId, (Convert.ToInt32(season.Substring(0, 4)) - i).ToString() + "Q4");
                    }
                    else
                    {
                        temp = GetCompanyFinanceStat(stockId, (Convert.ToInt32(season.Substring(0, 4)) - i).ToString() + "Q4");
                    }

                    compTenYrDataLst.Add(temp);
                }

                return compTenYrDataLst;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 回傳最新的財報資料年季別
        /// </summary>
        /// <returns>Ex:2017Q4</returns>
        internal static string LastStatDate()
        {
            try
            {
                //第一季財報出來月份
                DateTime fstSeasonStatDt = Convert.ToDateTime(DateTime.Now.Year.ToString() + "/5/15");

                DateTime secSeasonStatDt = Convert.ToDateTime(DateTime.Now.Year.ToString() + "/8/14");

                DateTime thdSeasonStatDt = Convert.ToDateTime(DateTime.Now.Year.ToString() + "/11/14");

                DateTime lstSeasonStatDt = Convert.ToDateTime(DateTime.Now.Year.ToString() + "/3/31");

                if (DateTime.Compare(DateTime.Now, lstSeasonStatDt) <= 0)
                {
                    return DateTime.Now.AddYears(-1).Year.ToString() + "Q3";
                }
                else if (DateTime.Compare(DateTime.Now, lstSeasonStatDt) > 0 && DateTime.Compare(DateTime.Now, fstSeasonStatDt) <= 0)
                {
                    return DateTime.Now.AddYears(-1).Year.ToString() + "Q4";
                }
                else if (DateTime.Compare(DateTime.Now, fstSeasonStatDt) > 0 && DateTime.Compare(DateTime.Now, secSeasonStatDt) <= 0)
                {
                    return DateTime.Now.Year.ToString() + "Q1";
                }
                else if (DateTime.Compare(DateTime.Now, secSeasonStatDt) > 0 && DateTime.Compare(DateTime.Now, thdSeasonStatDt) <= 0)
                {
                    return DateTime.Now.Year.ToString() + "Q2";
                }
                else
                {
                    return DateTime.Now.Year.ToString() + "Q3";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 查看是否有此檔股票代號
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        internal static bool CheckValidStockId(string stockId)
        {
            try
            {
                if (stockId == "")
                {
                    return false;
                }

                string url = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=2";

                WebClient webC = new WebClient();

                HtmlWeb web = new HtmlWeb();

                HtmlDocument doc = new HtmlDocument();

                //doc = web.Load(url);

                doc.Load(webC.OpenRead(url), Encoding.GetEncoding("big5"));
                //doc.Load(web.OpenRead(url));

                //WebBrowser htmlbrowser = new WebBrowser();

                //var node = doc.DocumentNode.SelectNodes("//tbody/tr[position()>2]/td[1]");
                //doc.ParsedText = EncodToBig5(doc.ParsedText);

                var nodes = doc.DocumentNode.SelectNodes("//tr[position()>2]/td[1]");

                if (nodes == null)
                {
                    return false;
                }

                var stockCnt = nodes.ToList().Where(s => s.InnerText.Split('　').First().Length == 4)
                                            .Where(s => s.InnerText.Split('　').First() == stockId)
                                            .Count();

                //var ticker = nodes.ToList().Where(s => s.InnerText.Split('　').First().Length == 4)
                //                                    .Where(s => s.InnerText.Split('　').First() == stockId)
                //                                    .First();

                var stockZhName = nodes.ToList().Where(s => s.InnerText.Split('　').First().Length == 4)
                                            .Where(s => s.InnerText.Split('　').First() == stockId)
                                            .Select(s => s.InnerText.Split('　').Last());

                if (stockCnt != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 顯示zvalue
        /// </summary>
        /// <param name="cmpLst"></param>
        internal static void ShowCompLstZValue(List<Company> cmpLst)
        {
            try
            {
                Console.WriteLine("股票代號" + cmpLst.First().Ticker);

                foreach (var c in cmpLst)
                {
                    Console.Write(string.Format("{0} ", c.Date));
                }
                Console.WriteLine();

                foreach (var c in cmpLst)
                {
                    Console.Write(string.Format("{0} ", Math.Round(c.ZValue, 4)));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        static string EncodToBig5(string source)
        {
            //string source = "⊃;nÅé&frac14;Ò⊃;Õ¤¤ªº¤@¯ë©Ê¿ù»~¡G&frac14;Ð·Ç GUI (⊃;Ï§Î¤Æ¥Î¤á¤¶­±)¡C";
            byte[] unknow = Encoding.GetEncoding(28591).GetBytes(source);
            string Big5 = Encoding.GetEncoding(950).GetString(unknow);
            return Big5;
            Console.WriteLine(Big5);
            Console.ReadLine();
        }
    }
}
