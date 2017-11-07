using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CompanyFinancialAnalysis.ViewModel;

namespace CompanyFinancialAnalysis.Models
{

    public class OrderLibrary
    {
        Program GetProgram = new Program();

        /// <summary>
        /// 將前十年公司財務包進 CompanyFinancial 
        /// </summary>
        /// <param name="stockId">股票編號(EX:3008)</param>
        public void ZvalueAnalysis(string stockId)
        {
            CompanyFinancial companyfinancial = new CompanyFinancial();

            companyfinancial.stockId = stockId;
            
            GetProgram.Main(stockId);

        }



        /// <summary>
        /// 回傳ViewModelList CompanyFinancial
        /// </summary>
        /// <param name="co_id_group">公司名稱List</param>
        /// <returns></returns>
        public List<CompanyFinancial> GetCompanyFinancialList(string co_group)
        {
            var co_id_group = new List<string>();
            if (String.IsNullOrEmpty(co_group)&&co_group!="1"&&co_group!="2")
            {
                return new List<CompanyFinancial>();
            }
            else if (co_group=="1")
            {
                co_id_group.Add("3008");
                co_id_group.Add("3406");
                co_id_group.Add("6209");
            }
            else
            {
                co_id_group.Add("2308");
                co_id_group.Add("2395");
                co_id_group.Add("6166");
            }
            
            var rst = new List<CompanyFinancial>();

            foreach (var item in co_id_group)
            {
                var addf = new CompanyFinancial();
                addf.stockId = item;
                addf.CFA_Information = GetProgram.GetCompanyTenStatDataLst(item);
                rst.Add(addf);
            }

            return rst;
            
        }


    }
}