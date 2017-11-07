using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;

namespace CompanyFinancialAnalysis.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var co_id = "3008";
            //var year = "102";
            //var season = "4";
            //var url = "http://mops.twse.com.tw/mops/web/ajax_t164sb03?TYPEK=all&step=1&firstin=1&off=1&queryName=co_id&t05st29_c_ifrs=N&t05st30_c_ifrs=N&isnew=false&co_id=" + co_id + "&year=" + year + "&season=" + season;
            //var web = new HtmlWeb();
            //var doc = web.Load(url);

            //var title = doc.DocumentNode.SelectNodes("//table[@class='hasBorder']/tr/td[1]").ToList();
            //var value = doc.DocumentNode.SelectNodes("//table[@class='hasBorder']/tr/td[2]").ToList();
            ////List<String> valxpath = testxpath.Select(x => x.InnerText).ToList();

            //BalanceSheet statement = new BalanceSheet();
            //statement.co_id = co_id;
            //statement.year = year;
            //statement.season = season;
            //statement.keyAndVal = (from x in title
            //                       join y in value on title.IndexOf(x) equals value.IndexOf(y)
            //                       select new KeyAndValue
            //                       {
            //                           account_item = x.InnerHtml,
            //                           value = y.InnerHtml
            //                       }).ToList();

            ////--------------------------------------------------------------------------------------------

            //var url2 = "http://mops.twse.com.tw/mops/web/ajax_t164sb04?TYPEK=all&step=1&firstin=1&off=1&queryName=co_id&t05st29_c_ifrs=N&t05st30_c_ifrs=N&isnew=false&co_id=" + co_id + "&year=" + year + "&season=" + season;
            //var web2 = new HtmlWeb();
            //var doc2 = web.Load(url2);

            //var title2 = doc2.DocumentNode.SelectNodes("//table[@class='hasBorder']/tr/td[1]").ToList();
            //var value2 = doc2.DocumentNode.SelectNodes("//table[@class='hasBorder']/tr/td[2]").ToList();
            ////List<String> valxpath = testxpath.Select(x => x.InnerText).ToList();

            //IncomeStatement statement2 = new IncomeStatement();
            //statement2.co_id = co_id;
            //statement2.year = year;
            //statement2.season = season;
            //statement2.keyAndVal = (from x in title2
            //                        join y in value2 on title2.IndexOf(x) equals value2.IndexOf(y)
            //                        select new KeyAndValue
            //                        {
            //                            account_item = x.InnerHtml,
            //                            value = y.InnerHtml
            //                        }).ToList();


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    public class BalanceSheet
    {
        public String co_id { get; set; }
        public String year { get; set; }
        public String season { get; set; }
        //public List<String> account_item { get; set; }
        //public List<String> value { get; set; }
        public List<KeyAndValue> keyAndVal { get; set; }
    }



    public class KeyAndValue
    {
        public String account_item { get; set; }
        public String value { get; set; }
    }


    public class IncomeStatement
    {
        public String co_id { get; set; }
        public String year { get; set; }
        public String season { get; set; }
        //public List<String> account_item { get; set; }
        //public List<String> value { get; set; }
        public List<KeyAndValue> keyAndVal { get; set; }


    }


}