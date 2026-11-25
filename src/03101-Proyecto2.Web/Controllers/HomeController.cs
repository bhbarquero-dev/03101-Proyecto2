using System.Diagnostics;
using _03101_Proyecto2.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _03101_Proyecto2.Web.Controllers;

public sealed class HomeController : Controller
{
    /// <summary>Página de inicio después del login. Aquí se mostrará el dashboard (HU-06).</summary>
    public IActionResult Index() => View();

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
