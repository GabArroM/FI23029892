using Microsoft.AspNetCore.Mvc;
using MVC.Models;

namespace MVC.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(TheModel model)
    {
        ViewBag.Valid = ModelState.IsValid;

        if (ViewBag.Valid)
        {
            // Cambio aplicado: se eliminan los espacios (' ') a nivel de Character usando LINQ
            //Ayuda con ChatGPT esta parte.
            var charArray = model.Phrase!
                                 .Where(c => !char.IsWhiteSpace(c)) 
                                 .ToList();

            foreach (var c in charArray)
            {
                if (!model.Counts!.ContainsKey(c))
                {
                    model.Counts[c] = 0;
                }
                model.Counts[c]++;
                model.Lower += c.ToString().ToLower();
                model.Upper += c.ToString().ToUpper();
            }
        }

        return View(model);
    }
}
