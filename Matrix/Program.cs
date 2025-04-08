using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Matrix;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Matrix Multiplication Benchmark ===\n");

        // Parametry testów
        int[] matrixSizes = { 100, 200, 300 };
        int[] threadCounts = { 1, 2, 4, 8, 16 };
        int iterations = 5;

        foreach (int size in matrixSizes)
        {
            Console.WriteLine($"\n--- MATRIX SIZE: {size} x {size} ---");
        
            foreach (int threads in threadCounts)
            {
                Console.WriteLine($"\n--- THREAD COUNT: {threads} ---");

                // SEKWENCYJNY
                long sequentialTime = TestMultiplication(size, 1, iterations, false);
                Console.WriteLine($"[SEKWENCYJNY]     Średni czas: {sequentialTime} ms");

                // PARALLEL.FOR
                long parallelTime = TestMultiplication(size, threads, iterations, true);
                Console.WriteLine($"[PARALLEL.FOR]    Średni czas: {parallelTime} ms");

                // NISKOPOZIOMOWY THREAD
                long threadTime = TestMultiplication(size, threads, iterations, true, true);
                Console.WriteLine($"[THREAD]          Średni czas: {threadTime} ms");

                // PRZYSPIESZENIA
                double speedupParallel = (double)sequentialTime / parallelTime;
                double speedupThread = (double)sequentialTime / threadTime;

                Console.WriteLine($"[SPEEDUP]         Parallel.For: {speedupParallel:F2}x | Thread: {speedupThread:F2}x");
            }
        }
    }


//Metoda testująca mnożenie macierzy z uśrednieniem czasu wykonania
    static long TestMultiplication(int size, int threads, int iterations, bool parallel, bool lowLevel = false)
    {
        long totalTime = 0;

        for (int i = 0; i < iterations; i++)
        {
            var multiplier = new Multiplier(size, threads);

            if (lowLevel)
                totalTime += multiplier.MultiplyWithThreads();
            else
                totalTime += parallel ? multiplier.MultiplyParallel() : multiplier.MultiplySequential();
        }

        return totalTime / iterations;
    }

}
