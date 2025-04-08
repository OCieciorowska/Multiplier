using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Matrix;

public class Multiplier
{
    private readonly int _size; //rozmiar macierzy nxn
        private readonly int _maxThreads;//max liczba watkow
        private readonly int[,] _matrixA;
        private readonly int[,] _matrixB;
        private readonly int[,] _resultMatrix;//macierz AxB
//konstruktor inicjalizuje macierze i ustawia rozmiar i liczbe wątków
        public Multiplier(int size, int maxThreads)
        {
            _size = size;
            _maxThreads = maxThreads;
            _matrixA = new int[size, size];
            _matrixB = new int[size, size];
            _resultMatrix = new int[size, size];
            
            InitializeMatrices();//losowe liczby w macierzach a i b
        }
// Wypełnia macierze A i B losowymi liczbami z przedziału [1, 9]
        private void InitializeMatrices()
        {
            var random = new Random();
            
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    _matrixA[i, j] = random.Next(1, 10);
                    _matrixB[i, j] = random.Next(1, 10);
                }
            }
        }
//mnożenie sekwencyjne macierzy, zwraca czas wykonywania w ms
        public long MultiplySequential()
        {
            var watch = Stopwatch.StartNew();//start pomiaru czasu
            
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    _resultMatrix[i, j] = 0;
                    for (int k = 0; k < _size; k++)
                    {
                        _resultMatrix[i, j] += _matrixA[i, k] * _matrixB[k, j];
                    }
                }
            }
            
            watch.Stop();//koniec pomiaru czasu
            return watch.ElapsedMilliseconds;
        }
//równoległe mnożenie macierzy z wykorzystaniem wilu wątków
        public long MultiplyParallel()
        {
            var watch = Stopwatch.StartNew();//pomiar czasu
            var options = new ParallelOptions { MaxDegreeOfParallelism = _maxThreads };//ustawiam limit wątków
// Parallel.For umożliwia równoległe wykonywanie operacji mnożenia wierszy
            Parallel.For(0, _size, options, i =>
            {
                for (int j = 0; j < _size; j++)
                {
                    _resultMatrix[i, j] = 0;
                    for (int k = 0; k < _size; k++)
                    {
                        _resultMatrix[i, j] += _matrixA[i, k] * _matrixB[k, j];
                    }
                }
            });

            watch.Stop();//koniec pomiaru czasu
            return watch.ElapsedMilliseconds;
        }
//wyświetlanie macierzy w konsoli
        public void PrintMatrices()
        {
            Console.WriteLine("Matrix A:");
            PrintMatrix(_matrixA);
            
            Console.WriteLine("\nMatrix B:");
            PrintMatrix(_matrixB);
            
            Console.WriteLine("\nResult Matrix:");
            PrintMatrix(_resultMatrix);
        }

        private void PrintMatrix(int[,] matrix)
        {
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
       //zadanie2 
        public long MultiplyWithThreads()
        {
            var watch = Stopwatch.StartNew();
            Thread[] threads = new Thread[_maxThreads];

            int rowsPerThread = _size / _maxThreads;
            int remainder = _size % _maxThreads;

            int currentRow = 0;

            for (int i = 0; i < _maxThreads; i++)
            {
                int start = currentRow;
                int count = rowsPerThread + (i < remainder ? 1 : 0);
                int end = start + count;

                threads[i] = new Thread(() => MultiplyRange(start, end));
                threads[i].Start();

                currentRow = end;
            }

            foreach (var thread in threads)
                thread.Join();

            watch.Stop();
            return watch.ElapsedMilliseconds;
        }

// Pomocnicza metoda – mnoży podany zakres wierszy
        private void MultiplyRange(int startRow, int endRow)
        {
            for (int i = startRow; i < endRow; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    _resultMatrix[i, j] = 0;
                    for (int k = 0; k < _size; k++)
                    {
                        _resultMatrix[i, j] += _matrixA[i, k] * _matrixB[k, j];
                    }
                }
            }
        }

}