using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace _03101_Proyecto2.Web.Services;

public static class ModelStateExtensions
{
    /// <summary>
    /// Copia al formulario los errores de una respuesta 400 de la API. Cada error se asocia al campo
    /// del modelo de vista con el mismo nombre (sin distinguir mayúsculas), de modo que aparece junto
    /// a ese campo; si no hay un campo equivalente, se muestra en el resumen del formulario.
    /// </summary>
    public static void AgregarErroresApi<TModelo>(
        this ModelStateDictionary modelState, IReadOnlyDictionary<string, string[]>? errores)
    {
        if (errores is null || errores.Count == 0)
        {
            modelState.AddModelError(string.Empty, "La API rechazó los datos enviados.");
            return;
        }

        foreach (var (clave, mensajes) in errores)
        {
            // Los errores de formato JSON llegan como "$.campo".
            var nombre = clave.StartsWith("$.", StringComparison.Ordinal) ? clave[2..] : clave;
            var propiedad = typeof(TModelo).GetProperty(
                nombre, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            foreach (var mensaje in mensajes)
            {
                modelState.AddModelError(propiedad?.Name ?? string.Empty, mensaje);
            }
        }
    }
}
