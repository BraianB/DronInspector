namespace DronInspector.Services;

using DronInspector.Models;
using Npgsql;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public int GuardarSimulacion(
        int n,
        int inicioX,
        int inicioY,
        List<Movimiento> movimientos)
    {
        using var connection =
            new NpgsqlConnection(_connectionString);

        connection.Open();

        using var transaction =
            connection.BeginTransaction();

        try
        {
            string sqlMaster =
            """
        INSERT INTO tb_master_control
        (
            tamanio_terreno,
            coordenada_x,
            coordenada_y
        )
        VALUES
        (
            @n,
            @x,
            @y
        )
        RETURNING id;
        """;

            using var cmdMaster =
                new NpgsqlCommand(
                    sqlMaster,
                    connection,
                    transaction);

            cmdMaster.Parameters.AddWithValue("@n", n);
            cmdMaster.Parameters.AddWithValue("@x", inicioX);
            cmdMaster.Parameters.AddWithValue("@y", inicioY);

            int masterId =
                Convert.ToInt32(
                    cmdMaster.ExecuteScalar());

            string sqlDetalle =
            """
        INSERT INTO tb_det_log
        (
            master_id,
            paso,
            coordenada_x,
            coordenada_y
        )
        VALUES
        (
            @master,
            @paso,
            @x,
            @y
        );
        """;

            int i = 0;

            while (i < movimientos.Count)
            {
                int pasoReal =
                    movimientos[i].Paso;

                int pasoOfuscado;

                if (pasoReal % 2 == 0)
                {
                    pasoOfuscado =
                        pasoReal * 2;
                }
                else
                {
                    pasoOfuscado =
                        -pasoReal;
                }

                using var cmdDetalle =
                    new NpgsqlCommand(
                        sqlDetalle,
                        connection,
                        transaction);

                cmdDetalle.Parameters.AddWithValue(
                    "@master",
                    masterId);

                cmdDetalle.Parameters.AddWithValue(
                    "@paso",
                    pasoOfuscado);

                cmdDetalle.Parameters.AddWithValue(
                    "@x",
                    movimientos[i].X);

                cmdDetalle.Parameters.AddWithValue(
                    "@y",
                    movimientos[i].Y);

                cmdDetalle.ExecuteNonQuery();

                i++;
            }

            transaction.Commit();

            return masterId;
        }
        catch
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // Handle rollback failure if necessary
            }

            throw;
        }
    }

    public void MostrarUltimos5Movimientos(int masterId)
    {
        using var connection =
            new NpgsqlConnection(_connectionString);

        connection.Open();

        string sql =
        """
    SELECT
        id,
        paso,
        coordenada_x,
        coordenada_y
    FROM tb_det_log
    WHERE master_id = @masterId
    ORDER BY id DESC
    LIMIT 5;
    """;

        using var command =
            new NpgsqlCommand(
                sql,
                connection);

        command.Parameters.AddWithValue(
            "@masterId",
            masterId);

        using var reader =
            command.ExecuteReader();

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine($" ÚLTIMOS 5 MOVIMIENTOS - SIMULACIÓN {masterId}");
        Console.WriteLine("========================================");

        while (reader.Read())
        {
            int id =
                reader.GetInt32(0);

            int pasoGuardado =
                reader.GetInt32(1);

            int x =
                reader.GetInt32(2);

            int y =
                reader.GetInt32(3);

            int pasoReal;

            if (pasoGuardado < 0)
            {
                pasoReal =
                    pasoGuardado * -1;
            }
            else
            {
                pasoReal =
                    pasoGuardado / 2;
            }
            Console.WriteLine(
                    $"Registro: {id,-4} | Paso: {pasoReal,-3} | Posición: ({x},{y})");
        }
    }


}