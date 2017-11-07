using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CompanyFinancialAnalysis.Models;

namespace CompanyFinancialAnalysis.ViewModel
{
    public class CompanyFinancial
    {
        //近十年公司財務資訊
        public List<Company> CFA_Information { get; set; }

        //股票代號
        public string stockId { get; set; }
    }
}