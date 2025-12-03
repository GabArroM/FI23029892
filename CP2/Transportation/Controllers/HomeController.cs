using Microsoft.AspNetCore.Mvc;
using Transportation.Interfaces;
using Transportation.Models;

namespace Transportation.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index([FromServices] IEnumerable<IAirplanes> airplanes)//ChatGPT
    {
        using var db = new CarsContext();
        var customer = db.Customers.First((c) => c.LastName == "Mouse");
        var ownership = db.CustomerOwnerships.First((o) => o.CustomerId == customer.CustomerId);
        var vin = db.CarVins.First((v) => v.Vin == ownership.Vin);
        var model = db.Models.First((m) => m.ModelId == vin.Vin);
        var brand = db.Brands.First((b) => b.BrandId == model.BrandId);
        ViewData["BrandModel"] = $"{brand.BrandName} - {model.ModelName}";

        var dealer = db.Dealers.First(d => d.DealerId == ownership.DealerId); 
        ViewData["Dealer"] = $"{dealer.DealerName} - {dealer.DealerAddress}"; //ChatGPT


        var airbus = airplanes.First(a => a.GetBrand == "Airbus");
        var boeing = airplanes.First(a => a.GetBrand == "Boeing");

        ViewData["Airbus"] = $"{airbus.GetBrand}: {string.Join(" - ", airbus.GetModels)}";
        ViewData["Boeing"] = $"{boeing.GetBrand}: {string.Join(" - ", boeing.GetModels)}";

        return View();
    }
}
