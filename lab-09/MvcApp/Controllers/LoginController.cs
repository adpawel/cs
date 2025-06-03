using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;

namespace MvcApp.Controllers;

[Route("[controller]")]
public class LoginController : Controller
{
    [HttpGet("GetForm")]
    public IActionResult GetForm()
    {
        return View();
    }

    [HttpPost("GetForm")]
    public IActionResult GetForm(IFormCollection form)
    {
        string login = form["login"].ToString();
        string password = form["password"].ToString();
        string hashedPassword = DatabaseService.MD5Hash(password);

        if (DatabaseService.IsValidUser(login, hashedPassword))
        {
            HttpContext.Session.SetString("login", login);
            return RedirectToAction("Successfull");
        }
        else
        {
            return View();
        }
    }

    [HttpGet("Successfull")]
    public IActionResult Successfull()
    {
        return View();
    }

    public IActionResult Logout(){
        if (HttpContext.Session.Keys.Contains("login"))
            HttpContext.Session.Remove("login");

        return RedirectToAction("GetForm");
    }
}
