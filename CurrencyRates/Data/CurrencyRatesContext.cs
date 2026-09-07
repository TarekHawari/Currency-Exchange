using Microsoft.Data.SqlClient;

namespace CurrencyRates.Data
{
    public class CurrencyRatesContext
    {
        private readonly string _connectionString = Environment.GetEnvironmentVariable("AZURE_CONNECTION_STRING") 
                                                   ?? throw new Exception("AZURE_CONNECTION_STRING not found");

        public static List<string> CurrencyNames = new();

        public async Task FetchCurrencyNames()
        {
            if (CurrencyNames.Count > 0)
                return;
        
            List<string> data = new();

            SqlDataReader sqlDataReader = await PrepareQuery(
                "select column_name from information_schema.columns where " +
                "table_name = 'rates_aud' and column_name not in " +
                "('id', 'dates') order by ordinal_position");

            // gets columns names
            try
            {
                while (await sqlDataReader.ReadAsync())
                    data.Add(sqlDataReader.GetString(0));
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }

            await sqlDataReader.CloseAsync();

            CurrencyNames = data;
        }

        public async Task<List<decimal>> FetchCurrencyData(DateOnly date)
        {
            List<decimal> rates = new();
            
            // made to fit the sql date dataset
            string parsedDate = date.ToString("yyyy-MM-dd");
            
            SqlDataReader sqlDataReader = await PrepareQuery($"SELECT * FROM rates_aud WHERE Dates = '{parsedDate}'");

            try
            {
                if (await sqlDataReader.ReadAsync())
                    foreach (string currencyName in CurrencyNames)
                        rates.Add((decimal)sqlDataReader[currencyName]);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
            
            await sqlDataReader.CloseAsync();
            
            return rates;
        }

        public async Task<(List<string>, List<decimal>, List<decimal>)> FetchDataForOneMonth(string dropDownCurrency, string clickedCurrency)
        {
            List<string> dates = new();
            List<decimal> dropDownCurrencyValues = new();
            List<decimal> clickedCurrencyValues = new();

            string query;
            if (clickedCurrency == "AUD")
                query = $"SELECT TOP 30 Dates, {dropDownCurrency} FROM rates_aud ORDER BY id DESC;";
            else if (dropDownCurrency == "AUD")
                query = $"SELECT TOP 30 Dates, {clickedCurrency} FROM rates_aud ORDER BY id DESC;";
            else
                query = $"SELECT TOP 30 Dates, {dropDownCurrency}, {clickedCurrency} FROM rates_aud ORDER BY id DESC;";
            
            SqlDataReader sqlDataReader = await PrepareQuery(query);
            
            try
            {
                while (await sqlDataReader.ReadAsync())
                {
                    DateTime dbDate = sqlDataReader.GetDateTime(0);
                    dates.Add(dbDate.ToString("yyyy-MM-dd"));

                    if (dropDownCurrency == "AUD")
                    {
                        dropDownCurrencyValues.Add(1.0m);
                        clickedCurrencyValues.Add((decimal)sqlDataReader[clickedCurrency]);

                        continue;
                    }
                    dropDownCurrencyValues.Add((decimal)sqlDataReader[dropDownCurrency]);
                    
                    if (clickedCurrency == "AUD")
                        clickedCurrencyValues.Add(1.0m);
                    else
                        clickedCurrencyValues.Add((decimal)sqlDataReader[clickedCurrency]);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
            
            await sqlDataReader.CloseAsync();
            
            dates.Reverse(); // switches the order to make most recent date the last
            return (dates, dropDownCurrencyValues, clickedCurrencyValues); 
        }
        
        public async Task<List<List<decimal>>> FetchTwoDays(DateOnly chosenDate)
        {
            List<List<decimal>> values = new();
            
            string parsedChosenDate = chosenDate.ToString("yyyy-MM-dd");
            string dayBeforeChosenDate = chosenDate.AddDays(-1).ToString("yyyy-MM-dd");

            SqlDataReader sqlDataReader = await PrepareQuery($"SELECT * FROM rates_aud WHERE dates BETWEEN '{dayBeforeChosenDate}' " +
                                                             $"AND '{parsedChosenDate}' ORDER BY dates DESC;");

            try
            {
                while (await sqlDataReader.ReadAsync())
                {
                    List<decimal> temp = new();
                    foreach (string currencyName in CurrencyNames)
                        temp.Add((decimal)sqlDataReader[currencyName]);

                    values.Add(temp);
                }

                await sqlDataReader.CloseAsync();
            } 
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            } 
            
            await sqlDataReader.CloseAsync();
            
            return values; 
        }

        public async Task<List<string>> FetchSources()
        {
            List<string> sources = new();
            
            SqlDataReader sqlDataReader = await PrepareQuery("SELECT source FROM sources");

            try
            {
                while (await sqlDataReader.ReadAsync())
                    sources.Add(sqlDataReader.GetString(0));
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
            
            await sqlDataReader.CloseAsync();
            
            return sources;
        }
        
        private async Task<SqlDataReader> PrepareQuery(string query)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            SqlCommand command = connection.CreateCommand();
            command.CommandText = query;
            
            SqlDataReader sqlDataReader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);

            return sqlDataReader;
        }
    }
}
