namespace DronInspector.Services;

using DronInspector.Models;


public class DroneSolver
{

    private List<Movimiento> movimientos = new();

    private readonly int[] movX =
 {
    -2,-2,
     2, 2,
    -1,-1,
     1, 1
};

    private readonly int[] movY =
    {
    -1, 1,
    -1, 1,
    -2, 2,
    -2, 2
};

    private bool EsValido(
        int x,
        int y,
        int n,
        int[,] tablero)
    {
        return
            x >= 0 &&
            x < n &&
            y >= 0 &&
            y < n &&
            tablero[x, y] == -1;
    }

    private void DFSAlcanzables(
        int x,
        int y,
        int n,
        bool[,] visitado)
    {
        visitado[x, y] = true;

        int i = 0;

        while (i < 8)
        {
            int nx = x + movX[i];
            int ny = y + movY[i];

            if (nx >= 0 &&
               nx < n &&
               ny >= 0 &&
               ny < n &&
               !visitado[nx, ny])
            {
                DFSAlcanzables(nx, ny, n, visitado);
            }

            i++;
        }
    }

    private int CalcularGrado(
        int x,
        int y,
        int n,
        int[,] tablero)
    {
        int grado = 0;

        int i = 0;

        while (i < 8)
        {
            int nx = x + movX[i];
            int ny = y + movY[i];

            if (EsValido(nx, ny, n, tablero))
                grado++;

            i++;
        }

        return grado;
    }

    private class Candidato
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Grado { get; set; }
    }

    private int ContarAlcanzables(
        int inicioX,
        int inicioY,
        int n)
    {
        bool[,] visitado = new bool[n, n];

        DFSAlcanzables(
            inicioX,
            inicioY,
            n,
            visitado);

        int contador = 0;

        int fila = 0;

        while (fila < n)
        {
            int columna = 0;

            while (columna < n)
            {
                if (visitado[fila, columna])
                    contador++;

                columna++;
            }

            fila++;
        }

        return contador;
    }

    private bool Resolver(
        int x,
        int y,
        int paso,
        int objetivo,
        int n,
        int[,] tablero)
    {
        if (paso == objetivo - 1)
            return true;

        var candidatos = new List<Candidato>();

        int i = 0;

        while (i < 8)
        {
            int nx = x + movX[i];
            int ny = y + movY[i];

            if (EsValido(nx, ny, n, tablero))
            {
                candidatos.Add(new Candidato
                {
                    X = nx,
                    Y = ny,
                    Grado = CalcularGrado(nx, ny, n, tablero)
                });
            }

            i++;
        }

        candidatos =
            candidatos
            .OrderBy(c => c.Grado)
            .ToList();

        foreach (var c in candidatos)
        {
            tablero[c.X, c.Y] = paso + 1;

            if (
                Resolver(
                    c.X,
                    c.Y,
                    paso + 1,
                    objetivo,
                    n,
                    tablero))
            {
                return true;
            }

            tablero[c.X, c.Y] = -1;
        }

        return false;
    }

    public bool ResolverRecorrido(
        int n,
        int inicioX,
        int inicioY,
        out int[,] tablero,
        out List<Movimiento> secuencia)
    {
        tablero = new int[n, n];

        int fila = 0;

        while (fila < n)
        {
            int columna = 0;

            while (columna < n)
            {
                tablero[fila, columna] = -1;
                columna++;
            }

            fila++;
        }

        movimientos.Clear();

        tablero[inicioX, inicioY] = 0;

        int alcanzables =
            ContarAlcanzables(
                inicioX,
                inicioY,
                n);

        bool exito =
            Resolver(
                inicioX,
                inicioY,
                0,
                alcanzables,
                n,
                tablero);

        if (exito)
        {
            GenerarSecuencia(
                tablero,
                n);
        }

        secuencia = movimientos;

        return exito;
    }

    private void GenerarSecuencia(
        int[,] tablero,
        int n)
    {
        int totalPasos = 0;

        int fila = 0;

        while (fila < n)
        {
            int columna = 0;

            while (columna < n)
            {
                if (tablero[fila, columna] >= 0)
                    totalPasos++;

                columna++;
            }

            fila++;
        }

        int pasoActual = 0;

        while (pasoActual < totalPasos)
        {
            fila = 0;

            while (fila < n)
            {
                int columna = 0;

                while (columna < n)
                {
                    if (tablero[fila, columna] == pasoActual)
                    {
                        movimientos.Add(
                            new Movimiento
                            {
                                Paso = pasoActual,
                                X = fila,
                                Y = columna
                            });
                    }

                    columna++;
                }

                fila++;
            }

            pasoActual++;
        }
    }

    public void MostrarTablero(
        int[,] tablero,
        int n)
    {
        int fila = 0;

        while (fila < n)
        {
            int columna = 0;

            while (columna < n)
            {
                if (tablero[fila, columna] == -1)
                {
                    Console.Write(".\t");
                }
                else
                {
                    Console.Write(
                        tablero[fila, columna] + "\t");
                }

                columna++;
            }

            Console.WriteLine();

            fila++;
        }
    }

}

