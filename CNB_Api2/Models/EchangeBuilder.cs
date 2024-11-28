using CNB_Api.Models;
using System;
using System.Globalization;

namespace CNB_Api2.Models
{
    public class EchangeBuilder
    {
        string urlCNB = "https://www.cnb.cz/cs/financni-trhy/devizovy-trh/kurzy-devizoveho-trhu/kurzy-devizoveho-trhu/denni_kurz.txt";
        
        private Data_Exchange? _data_exchange;
        public Data_Exchange? data_exchange
        {
            get {
                // provede update jen pokud je to nutné v předem definovaný čas
                if (_data_exchange?.Date.DayOfYear < DateTime.Now.DayOfYear && TimeOnly.FromDateTime(DateTime.Now) > new TimeOnly(14, 31, 00))
                { 
                    UpdateData();
                }
                return _data_exchange; }
        }

        public EchangeBuilder()
        {
            _data_exchange = new Data_Exchange();
            _data_exchange = ReadDataCNB(urlCNB);
        }
        public void UpdateData()
        {
            //_data_exchange = ReadDataCNB(urlCNB);
            // pokud se nepovede update hodnot tak si neprepise ty puvodni (logika muze byt jina, ja jsem si zvolil tuto)
            var data = ReadDataCNB(urlCNB);
            if (data != null) { _data_exchange = data; }
               
        }
        public Data_Exchange? ReadDataCNB(string url)
        {
            bool anyfault = false;
            Data_Exchange dataechange = new Data_Exchange();
            dataechange.Provider = "CNB";
            var result = new HttpClient().GetStringAsync(url).Result;
            if (!String.IsNullOrEmpty(result))
            {
                List<string> lines = result.Split(
                    new string[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                    ).ToList();
                // radek 1
                if (lines.Count > 0)
                {
                    List<string> firstLine = lines[0].Split(" #", StringSplitOptions.None).ToList();
                    if (firstLine.Count == 2)
                    {
                        if (DateTime.TryParseExact(firstLine[0], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                        { 
                            dataechange.Date = DateTime.ParseExact(firstLine[0], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture); ;
                        }
                        else
                        {
                            anyfault = true;
                            ToLog("v hlavičce neni korektni datum");
                        }
                        var dateTime = DateTime.ParseExact(firstLine[0], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        dataechange.Date = dateTime;
                        if (Int32.TryParse(firstLine[1], out int value))
                        {
                            dataechange.Count = value;
                        }
                        else
                        {
                            anyfault = true;
                            ToLog("v hlavičce neni korektni cítač");
                        }
                    }
                }
                List<Data_ExchangeList> dataExchangeList = new List<Data_ExchangeList>();
                if (lines.Count > 1 && !anyfault)
                {
                    //radek 2
                    if (!String.Equals(lines[1], "země|měna|množství|kód|kurz"))
                    {
                        anyfault = true;
                        ToLog("neni kompatibilní hlavička dat");
                    }
                    //radky ostatni
                    if (lines.Count > 2 && !anyfault)
                    {
                        
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
                    }
                }
                dataechange.DataExchangeList = dataExchangeList;
            }
            if(anyfault)
            {
                return null;
            }
            else
            {
                return dataechange;
            }
            

            //return new Data_Exchange();


        }
        private void ToLog(string logtext)
        {

        }
    }
}
