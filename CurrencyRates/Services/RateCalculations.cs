using CurrencyRates.Data;

namespace CurrencyRates.Services
{
    public class RateCalculations
    {
        public List<decimal> CalculateRates(string selectedCurrency, List<decimal> data)
        {
            if (selectedCurrency == "AUD")
                return data;
            
            List<decimal> modifiedData = new();
            int indexOfCurrencyType = CurrencyRatesContext.CurrencyNames.IndexOf(selectedCurrency);
            decimal selectedCurrencyRate = data[indexOfCurrencyType]; // currency rate of the selected currency
            
            for (int i = 0; i < data.Count; i++)
            {
                // skip the value of the selected currency type by checking index
                if (i == indexOfCurrencyType)
                {
                    modifiedData.Add(1 / selectedCurrencyRate); // to get AUD rate
                    continue;
                }

                modifiedData.Add(data[i] / selectedCurrencyRate); // divides the rate in the db with the selected rate
            }
            return modifiedData;
        }

        public List<decimal> CalculateChange(string currency, List<List<decimal>> data)
        {
            List<decimal> change = new();
            
            // get the actual rates, not the AUD rates
            List<decimal> recentDay = CalculateRates(currency, data[0]);
            List<decimal> previousDay = CalculateRates(currency, data[1]);
            
            for (int i = 0; i < recentDay.Count; i++)
            {
                decimal result = ((recentDay[i] - previousDay[i]) / previousDay[i]) * 100;
                decimal roundedResult= Math.Round(result, 2);
                change.Add(roundedResult);
            }
            
            return change;
        }
    }
}

