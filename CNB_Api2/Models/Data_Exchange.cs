using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Globalization;

namespace CNB_Api.Models
{
    public class Data_Exchange
    {
        public DateTime Date { get; set; }
        public string Provider { get; set; }
        public int? Count { get; set; }
        public List<Data_ExchangeList> DataExchangeList { get; set; }



        //public Data_Exchange()
        //{
        //    //this.Date = DateTime.Now;
        //    //this.Provider = "CNB";
        //    //this.Count = 0;
        //    //var data = ReadData("https://www.cnb.cz/cs/financni-trhy/devizovy-trh/kurzy-devizoveho-trhu/kurzy-devizoveho-trhu/denni_kurz.txt");
        //}
    }
}
