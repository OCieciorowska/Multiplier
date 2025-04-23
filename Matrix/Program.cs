using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Matrix;

class Program
{
    static void Main(string[] args)
    {  
        Console.WriteLine("=== Matrix Multiplication Benchmark ===\n");

        // Parametry testów
        int[] matrixSizes = { 100, 200, 300 }; // Rozmiary macierzy
        int[] threadCounts = { 1, 2, 4, 8, 16 }; // Liczba wątków do testowania
        int iterations = 5; // Liczba powtórzeń dla uśrednienia

        //  ścieżki do folderu i pliku z wynikami
        string projectDir = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
        string folderPath = Path.Combine(projectDir, "wyniki");
        string csvPath = Path.Combine(folderPath, "wyniki.csv");


        // Utwórz folder jeśli nie istnieje
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Jeśli plik istnieje — usuń go
        if (File.Exists(csvPath))
        {
            File.Delete(csvPath);
        }

        // Napisz nagłówek
        File.WriteAllText(csvPath, "MatrixSize,Threads,Method,AverageTimeMicroseconds,Speedup\n");

        foreach (int size in matrixSizes) // Dla każdego rozmiaru macierzy
        {
            Console.WriteLine($"\n--- MATRIX SIZE: {size} x {size} ---");
        
            foreach (int threads in threadCounts) // Dla każdej liczby wątków
            {
                Console.WriteLine($"\n--- THREAD COUNT: {threads} ---");

                // SEKWENCYJNY
                long sequentialTime = TestMultiplication(size, 1, iterations, false);
                Console.WriteLine($"[SEKWENCYJNY]     Średni czas: {sequentialTime} µs");
                AppendToCsv(csvPath, size, threads, "Sequential", sequentialTime, 1.0);


                // PARALLEL.FOR
                long parallelTime = TestMultiplication(size, threads, iterations, true);
                double speedupParallel = (double)sequentialTime / parallelTime;
                Console.WriteLine($"[PARALLEL.FOR]    Średni czas: {parallelTime} µs");
                Console.WriteLine($"[SPEEDUP]         Parallel.For: {speedupParallel:F2}x");
                AppendToCsv(csvPath, size, threads, "Parallel.For", parallelTime, speedupParallel);

                // NISKOPOZIOMOWY THREAD
                long threadTime = TestMultiplication(size, threads, iterations, true, true);
                double speedupThread = (double)sequentialTime / threadTime;
                Console.WriteLine($"[THREAD]          Średni czas: {threadTime} µs");
                Console.WriteLine($"[SPEEDUP]         Thread: {speedupThread:F2}x");
                AppendToCsv(csvPath, size, threads, "Thread", threadTime, speedupThread);

                
            }
        }
        Console.WriteLine("\n=== Wszystko zapisane w 'wyniki.csv'! ===");
    }


//Metoda testująca mnożenie macierzy z uśrednieniem czasu wykonania
    static long TestMultiplication(int size, int threads, int iterations, bool parallel, bool lowLevel = false)
    {
        long totalTime = 0;//sumaryczny czas

        for (int i = 0; i < iterations; i++) //powtarzaj testy
        {
            var multiplier = new Multiplier(size, threads);

            if (lowLevel) //jesli używamy niskopoziomowego wątku
                totalTime += multiplier.MultiplyWithThreads();
            else//w przeciwnym razie 
                totalTime += parallel ? multiplier.MultiplyParallel() : multiplier.MultiplySequential();
        }

        return totalTime / iterations; //zwróc średni czas
    }
    // Zapis do CSV
    static void AppendToCsv(string path, int size, int threads, string method, double avgTime, double speedup)
    {
        string line = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3:F2},{4:F2}", size, threads, method, avgTime, speedup);
        File.AppendAllText(path, line + "\n");
    }


}
