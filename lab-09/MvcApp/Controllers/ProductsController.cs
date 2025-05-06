using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;

namespace MvcApp.Controllers;

[Route("[controller]")]
public class ProductsController : Controller
{
    [HttpGet("Get")]
    public IActionResult Get()
    {
        if (HttpContext.Session.Keys.Contains("login")){
            var dane = DatabaseService.GetAll();
            return View(dane);
        }
        else 
            return RedirectToAction("Login/GetForm");
    }

    [HttpPost]
    public IActionResult AddData(string newData)
    {
        DatabaseService.AddData(newData);
        return RedirectToAction("Get");
    }

}