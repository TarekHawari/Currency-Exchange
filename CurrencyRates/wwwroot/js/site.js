// Used to get graph image from api and give it to GraphImage div to generate
function loadGraph(dropDownCurrency, clickedCurrency) 
{
    try 
    {
        document.getElementById("GraphImage").src =
            `/api/graph/${dropDownCurrency}/${clickedCurrency}`;
    }
    catch(e)
    {
        Console.log("error")
    }
}