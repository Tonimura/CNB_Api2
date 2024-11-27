using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Globalization;

namespace CNB_Api.Models
{
    public class Data_Exchange
    {
        public DateTime Date { get; set; }
        public string Provider { get; set; }
        public int Count { get; set; }
        public List<Data_ExchangeList> DataExchangeList { get; set; }



        public Data_Exchange()
        {
            //this.Date = DateTime.Now;
            //this.Provider = "CNB";
            //this.Count = 0;
            //var data = ReadData("https://www.cnb.cz/cs/financni-trhy/devizovy-trh/kurzy-devizoveho-trhu/kurzy-devizoveho-trhu/denni_kurz.txt");
        }
        public Data_Exchange ReadData(string url)
        {
            bool anyfault = false;
            var result = new HttpClient().GetStringAsync(url).Result;
            if (!String.IsNullOrEmpty(result))
            {
                List<string> lines = result.Split(
                    new string[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                    ).ToList();
                if (lines.Count > 0)
                {
                    List<string> firstLine = lines[0].Split( " #", StringSplitOptions.None).ToList();
                    if (firstLine.Count == 2)
                    {
                        //var dateTime = Convert.ToDateTime(firstLine[0].Replace(" ", ""), "ddMMyyyy");
                        var dateTime = DateTime.ParseExact(firstLine[0], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        this.Date = dateTime;
                        if (Int32.TryParse(firstLine[1], out int value))
                        {
                            this.Count = value;
                        }
                        else
                        {
                            anyfault = true;
                            ToLog("neni korektni cítač");
                        }
                    }
                }
                if (lines.Count > 1 && !anyfault)
                {
                    //List<string> secondLine = lines[0].Split(" #", StringSplitOptions.None).ToList();
                    if (!String.Equals(lines[1], "země|měna|množství|kód|kurz"))
                    {
                        anyfault = true;
                        ToLog("neni kompatibilní hlavička");
                    }
                    if (lines.Count > 2 && !anyfault)
                    {
                        List<Data_ExchangeList> dataExchangeList = new List<Data_ExchangeList>();
                        for (int i = 2; i < lines.Count - 1; i++)
                        {
                            if (!String.IsNullOrEmpty(lines[i]))
                            {
                                List<string> line = lines[i].Split("|", StringSplitOptions.None).ToList();
                                Data_ExchangeList data_ExchangeList = new Data_ExchangeList();
                                data_ExchangeList.Country = line[0];
                                data_ExchangeList.DenominationName = line[1];
                                if (Int32.TryParse(line[2], out int valuecount))
                                {
                                    data_ExchangeList.Count = valuecount;
                                }
                                //data_ExchangeList.Count = line[2];
                                data_ExchangeList.DenominationCode = line[3];
                                if (float.TryParse(line[4].Replace(",", "."), out float valuecoin))
                                {
                                    data_ExchangeList.Coin = float.Parse(line[4].Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
                                }
                                dataExchangeList.Add(data_ExchangeList);
                                
                            }
                        }
                        this.DataExchangeList = dataExchangeList;
                    }
                }

                
            }
            return this;
         
            //return new Data_Exchange();


        }

        private void ToLog(string logtext)
        {

        }
    }
}
