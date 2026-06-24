using DronInspector.Models;
using DronInspector.Services;
using Microsoft.Extensions.Configuration;



namespace DronInspector;


class Program
{
    static void Main(string[] args)
    {
        var configuration =
        new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(
            "appsettings.json",
            optional: false,
            reloadOnChange: false)
        .Build();

        string connectionString =
        configuration.GetConnectionString(
            "PostgresConnection")
        ?? throw new Exception(
            "No se encontró la cadena de conexión");

        Console.Write("Ingrese N: ");

        int n =
        int.Parse(
        Console.ReadLine()!);

        while (n < 1)
        {
            Console.WriteLine(
                "N debe ser mayor o igual a 1");

            n =
                int.Parse(
                    Console.ReadLine()!);
        }

        Console.Write("Inicio X: ");

        int x =
            int.Parse(
                Console.ReadLine()!);

        Console.Write("Inicio Y: ");

        int y =
            int.Parse(
                Console.ReadLine()!);
        while (
    x < 0 ||
    x >= n ||
    y < 0 ||
    y >= n)
        {
            Console.WriteLine(
                "Coordenadas inválidas");

            Console.Write("Inicio X: ");

            x =
                int.Parse(
                    Console.ReadLine()!);

            Console.Write("Inicio Y: ");

            y =
                int.Parse(
                    Console.ReadLine()!);
        }
        var solver =
            new DroneSolver();

        bool exito =
            solver.ResolverRecorrido(
                n,
                x,
                y,
                out int[,] tablero,
                out var movimientos);

        if (exito)
        {
            Console.WriteLine();
            Console.WriteLine("RECORRIDO:");

            solver.MostrarTablero(
                tablero,
                n);
        }
        else
        {
            Console.WriteLine(
                "No existe solución.");

            return;
        }

        var database =
            new DatabaseService(
                connectionString);

        int simulacionId =
            database.GuardarSimulacion(
                n,
                x,
                y,
                movimientos);

        
        Console.WriteLine();
        Console.WriteLine(
            $"Simulación guardada con ID {simulacionId}");


        database.MostrarUltimos5Movimientos(
    simulacionId);



    }




}
