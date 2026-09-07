using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CurrencyRates.Data;
using CurrencyRates.Services;

namespace Interview_Challenge.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public string Currency {get; set;} = "AUD";
    
    // sets the default date to be 3 days behind because of database
    [BindProperty]
    public DateOnly Date {get; set;} = DateOnly.FromDateTime(DateTime.Today.AddDays(-4)); 
    
    // used in cshtml
    public List<decimal> CalculatedRates = new();
    public List<decimal> CalculatedChange = new();
    public List<string> Sources = new();

    public async Task OnGetAsync()
    {
        try
        {
            await CurrencyDataRetriever();
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
        }
    }
    
    public async Task<IActionResult> OnPostCurrencyDetails() 
    {
        try
        {
            // security to prevent possible malicious injections and check if selected date is a relevant currency.
            if ((Currency != "AUD" && !CurrencyRatesContext.CurrencyNames.Contains(Currency)) 
                || Date >= DateOnly.FromDateTime(DateTime.Today.AddDays(-3)))
                return Page();
            
            await CurrencyDataRetriever();
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
        }
        
        return Page();
    }

    private async Task CurrencyDataRetriever()
    {
        CurrencyRatesContext currencyRatesContext = new();
        await currencyRatesContext.FetchCurrencyNames();
        
        List<decimal> allData = await currencyRatesContext.FetchCurrencyData(Date);
        RateCalculations rateCalculations = new();
        CalculatedRates = rateCalculations.CalculateRates(Currency, allData);
        
        Sources = await currencyRatesContext.FetchSources();
        
        List<List<decimal>> fetchTwoDays = await currencyRatesContext.FetchTwoDays(Date); 
        CalculatedChange = rateCalculations.CalculateChange(Currency, fetchTwoDays);
    }
}
