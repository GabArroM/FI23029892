using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Practica2.Models;

namespace Practica2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(new Binario());
    }



    [HttpPost]
    public IActionResult BinarioResultado(Binario binarioModel)
    {
        
        if (!ModelState.IsValid)
        {
            
            return View("Index", binarioModel);
        }

      
        binarioModel.A = binarioModel.A!.PadLeft(8, '0');
        binarioModel.B = binarioModel.B!.PadLeft(8, '0');

        
        char[] andResult = new char[8];
        char[] orResult = new char[8];
        char[] xorResult = new char[8];

        for (int i = 0; i < 8; i++)
        {
            char bitA = binarioModel.A[i];
            char bitB = binarioModel.B[i];

            andResult[i] = (bitA == '1' && bitB == '1') ? '1' : '0';
            orResult[i] = (bitA == '1' || bitB == '1') ? '1' : '0';
            xorResult[i] = (bitA != bitB) ? '1' : '0';
        }

        ViewData["AND"] = new string(andResult);
        ViewData["OR"] = new string(orResult);
        ViewData["XOR"] = new string(xorResult);


        int a = Convert.ToInt32(binarioModel.A, 2);
        int b = Convert.ToInt32(binarioModel.B, 2);

        ViewData["SUMA"] = Convert.ToString(a + b, 2).PadLeft(8, '0');
        ViewData["MULT"] = Convert.ToString(a * b, 2).PadLeft(8, '0');

        return View("Index", binarioModel);
    }





    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
