using System.Text.Json;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data;

public static class DbContextExtensions
{
    public static void SeedCustomers(this Dsw2025TpiContext context, string jsonFilePath)
    {
        if (context.Customers.Any()) return;
        if (!File.Exists(jsonFilePath)) return;

        try
        {
            var json = File.ReadAllText(jsonFilePath);
            var customers = JsonSerializer.Deserialize<List<Customer>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            if (customers != null && customers.Count > 0)
            {
                context.Customers.AddRange(customers);
                context.SaveChanges();
                // Opcional: Console.WriteLine($"{customers.Count} customers seeded.");
            }
        }
        catch (Exception ex)
        {
            // Acá simplemente evitás que falle la app por el seed; podés loguear si querés.
            // Ejemplo:
            Console.WriteLine($"Error al seedear clientes: {ex.Message}");
        }
    }
}
