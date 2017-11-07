using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompanyFinancialAnalysis.Models;
using CompanyFinancialAnalysis.ViewModel;

namespace CompanyFinancialAnalysis.Controllers
{
    public class CFAController : Controller
    {
        OrderLibrary order = new OrderLibrary();

        // GET: Companyfinancial
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string stockId)
        {
            order.ZvalueAnalysis(stockId);

            return View();
        }

        public ActionResult Analysis(string group)
        {
            List<CompanyFinancial> rst =  order.GetCompanyFinancialList(group);

            return View(rst);
        }


    }
}