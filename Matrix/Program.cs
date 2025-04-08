using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Matrix;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Matrix Multiplication Benchmark");
        Console.WriteLine("--------------------------------");

        // Parametry testów
        int[] matrixSizes = { 100, 200, 300 };//zestaw rozmiarów macierzy do testów
        int[] threadCounts = { 1, 2, 4, 8, 16 };//liczba watków do testów
        int iterations = 5;//liczba powtórzeń testów 

        Console.WriteLine("Przykładowe macierze i wynik mnożenia:");
        var exampleMultiplier = new Multiplier(3, 2); 
        exampleMultiplier.MultiplySequential();
        exampleMultiplier.PrintMatrices();
        foreach (int size in matrixSizes)//pętla po różnych rozmiarach macierzy
        {
            Console.WriteLine($"\nMatrix size: {size}x{size}");
            Console.WriteLine("Threads | Sequential (ms) | Parallel (ms) | Speedup");
            Console.WriteLine("--------|-----------------|---------------|--------");

            // Test sekwencyjny (1 wątek)- jako punkt odniesienia
            var sequentialTime = TestMultiplication(size, 1, iterations, false);
                
            foreach (int threads in threadCounts)//testy równoległe dla róznych liczby wątków
            {
                var parallelTime = TestMultiplication(size, threads, iterations, true);
                double speedup = (double)sequentialTime / parallelTime;//obliczamy przyspieszenie
                Console.WriteLine($"{threads,7} | {sequentialTime,15} | {parallelTime,13} | {speedup,6:F2}x");
            }
        }
    }
//Metoda testująca mnożenie macierzy z uśrednieniem czasu wykonania
    static long TestMultiplication(int size, int threads, int iterations, bool parallel)
    {
        long totalTime = 0;
            
        for (int i = 0; i < iterations; i++)
        {
            var multiplier = new Multiplier(size, threads);
            totalTime += parallel ? multiplier.MultiplyParallel() : multiplier.MultiplySequential();
        }
            
        return totalTime / iterations;// średni czas z wielu powtórzeń
    }
}
