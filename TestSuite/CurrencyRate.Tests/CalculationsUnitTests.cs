using CurrencyRates.Services;
using CurrencyRates.Data;
using Xunit;

namespace CurrencyRate.Tests;

public class CaluculationsUnitTests
{
    private readonly RateCalculations _rateCalculations = new();
        
    // The CalculateRates function returns the data given in parameter
    [Fact]
    public void CalculateRates_AUD()
    {
        string selectedCurrency = "AUD";
        List<decimal> data = new List<decimal> {1.11m, 2.22m, 3.33m};

        List<decimal> result = _rateCalculations.CalculateRates(selectedCurrency, data);

        Assert.Same(data, result); // checks if the parameter value is used
    }
    
    [Fact]
    public void CalculateRates_Other_Currencies_Test()
    {
        // function uses CurrencyNames property
        CurrencyRatesContext.CurrencyNames = new List<string> { "USD", "CNY", "JPY" };
        
        string selectedCurrency = "USD";
        List<decimal> data = new List<decimal> {1.11m, 2.22m, 3.33m};

        List<decimal> result = _rateCalculations.CalculateRates(selectedCurrency, data);

        Assert.Equal(1.0m/1.11m, result[0]); // checks if it turned into aud rate
        Assert.Equal(2.22m/1.11m, result[1]);
        Assert.Equal(3.33m/1.11m, result[2]);
    }

    [Fact]
    public void CalculateChange_Other_Currencies_Test()
    {
        List<List<decimal>> data = new List<List<decimal>>
        {
            new List<decimal> {1.11m, 1.22m, 1.33m}, 
            new List<decimal> {2.11m, 2.22m, 2.33m}
        };
        
        List<decimal> result = _rateCalculations.CalculateChange(data);
        
        Assert.Equal( Math.Round(((1.11m-2.11m)/2.11m) * 100m, 2), result[0] );
        Assert.Equal( Math.Round(((1.22m-2.22m)/2.22m) * 100m, 2), result[1] );
        Assert.Equal( Math.Round(((1.33m-2.33m)/2.33m) * 100m, 2), result[2] );
    }
    
    //...
}
