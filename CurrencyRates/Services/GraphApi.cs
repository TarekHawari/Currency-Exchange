namespace CurrencyRates.Services;
using Data;
using ScottPlot;

public class GraphApi
{
    public async Task<IResult> GraphCurrencyRateTrends(string dropDownCurrency, string clickedCurrency)
    {
        CurrencyRatesContext currencyRatesContext = new();
        // receives multiple lists to be handled
        var (dates, dropDownCurrencyValues, clickedCurrencyValues) =
            await currencyRatesContext.FetchDataForOneMonth(dropDownCurrency, clickedCurrency);
        
        List<decimal> calculatedRates = CalculateRateForGraph(dropDownCurrencyValues, clickedCurrencyValues);
        
        // handles datatypes for ScottPlott graph
        DateTime[] parsedDates = dates.Select(DateTime.Parse).ToArray();
        double[] parsedRates = calculatedRates.Select(rate => (double)rate).ToArray();
        
        Plot plot = new();
        plot.Title($"{dropDownCurrency} / {clickedCurrency}");
        plot.Add.Scatter(parsedDates, parsedRates);
        plot.Axes.DateTimeTicksBottom();
        byte[] imageBytes = plot.GetImageBytes(500, 450);
        
        return Results.File(imageBytes, "image/png");
    }
    
    private List<decimal> CalculateRateForGraph(List<decimal> dropDownCurrencyValues, List<decimal> clickedCurrencyValues)
    {
            
        List<decimal> modifiedData = new();
            
        for (int i = 0; i < dropDownCurrencyValues.Count; i++)
        {
            /*// skip the value of the selected currency type by checking index
            if (i == indexOfCurrencyType)
            {
                modifiedData.Add(1 / selectedCurrencyRate); // to get AUD rate
                continue;
            }*/
    
            // divides currencies to get rate because db originally contains AUD rates
            modifiedData.Add(clickedCurrencyValues[i] / dropDownCurrencyValues[i]); 
        }
        return modifiedData;
    }
}