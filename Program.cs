using System.Diagnostics;

public class SpeedupCalculator
{
    public async Task iniciar()
    {
        int processors = Environment.ProcessorCount;
        Console.WriteLine($"Procesadores Total: {processors}");
        Console.WriteLine();

        int cargaTrabajo = 1000;

        foreach (var procesadores in new[] { 1, 4 })
        {
            await EjecutarCargas(cargaTrabajo, procesadores);
        }
    }

    private async Task EjecutarCargas(int cargaTrabajo, int procesadores)
    {
        var data = Enumerable.Range(1, cargaTrabajo).ToArray();
        var sw = new Stopwatch();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = procesadores
        };

        //Procesamiento Secuencial
        Console.WriteLine($"Procesadores a utilizar: {procesadores}");
        sw.Restart();
        var resultadoSecuencial = ProcesarDatosSecuenciales(data.ToList());
        sw.Stop();
        var tiempoSecuencial = sw.ElapsedMilliseconds;
        Console.WriteLine($"Tiempo Secuencial: {tiempoSecuencial} ms");
        Console.WriteLine();

        //Procesamiento Paralelo            
        sw.Restart();
        var resultadoParalelo = await ProcesarDatosParalelos(data.ToList(), options);
        sw.Stop();
        var tiempoParalelo = sw.ElapsedMilliseconds;
        Console.WriteLine($"Tiempo Paralelo: {tiempoParalelo} ms");

        double speedup = (double)tiempoSecuencial / tiempoParalelo;
        Console.WriteLine($"Speedup: {speedup:F2}");
        Console.WriteLine($"Eficiencia: {(speedup / procesadores):P2}");

    }

    private async Task<List<int>> ProcesarDatosParalelos(List<int> list, ParallelOptions options)
    {
        var resultado = new List<int>(list.Count);
        var lockObject = new object();

        await Task.Run(() =>
        {
            Parallel.ForEach(list, options, item =>
            {
                var result = CargaDatos(item);
                lock (lockObject)
                {
                    resultado.Add(result);
                }
            });
        });
        return resultado;
    }

    private List<int> ProcesarDatosSecuenciales(List<int> data)
    {
        var resultado = new List<int>(data.Count);
        foreach (var item in data)
        {
            resultado.Add(CargaDatos(item));
        }
        return resultado;
    }

    private int CargaDatos(int x)
    {
        Thread.Sleep(10);
        return x * x;
    }

    // Método Main
    public static async Task Main(string[] args)
    {
        SpeedupCalculator calculator = new SpeedupCalculator();
        await calculator.iniciar();
    }
}



