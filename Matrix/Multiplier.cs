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
            _size = size; // Ustaw rozmiar macierzy
            _maxThreads = maxThreads;  // Ustaw maksymalną liczbę wątków
            _matrixA = new int[size, size]; // Inicjalizuj macierz A
            _matrixB = new int[size, size]; // Inicjalizuj macierz B
            _resultMatrix = new int[size, size]; // Inicjalizuj macierz wynikową
            
            InitializeMatrices();//losowe liczby w macierzach a i b
        }
// Wypełnia macierze A i B losowymi liczbami z przedziału [1, 9]
        private void InitializeMatrices()
        {
            var random = new Random(); // Generator liczb losowych
            
            for (int i = 0; i < _size; i++) // Iteruj po wierszach
            {
                for (int j = 0; j < _size; j++) // Iteruj po kolumnach
                {
                    _matrixA[i, j] = random.Next(1, 10); // Losowa liczba dla A
                    _matrixB[i, j] = random.Next(1, 10); // Losowa liczba dla B
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
                    _resultMatrix[i, j] = 0; // Wyzeruj komórkę wyniku
                    for (int k = 0; k < _size; k++) // Przemnóż odpowiednie elementy
                    {
                        _resultMatrix[i, j] += _matrixA[i, k] * _matrixB[k, j];
                    }
                }
            }
            
            watch.Stop();//koniec pomiaru czasu
            //return watch.ElapsedMilliseconds;
            return (long)watch.Elapsed.TotalMicroseconds;
        }
//równoległe mnożenie macierzy 
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
            //return watch.ElapsedMilliseconds;
            return (long)watch.Elapsed.TotalMicroseconds;
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
       // Mnożenie macierzy z wykorzystaniem Thread (niskopoziomowe)
        public long MultiplyWithThreads()
        {
            var watch = Stopwatch.StartNew();
            Thread[] threads = new Thread[_maxThreads]; // Tablica wątków

            int rowsPerThread = _size / _maxThreads;  // Liczba wierszy na wątek
            int remainder = _size % _maxThreads; // Reszta (jeśli rozmiar się nie dzieli idealnie)

            int currentRow = 0; //aktualny wiersz startowy

            for (int i = 0; i < _maxThreads; i++) // Tworzenie i uruchamianie wątków
            
            {
                int start = currentRow;//startowy wiersz
                int count = rowsPerThread + (i < remainder ? 1 : 0);//liczba wierszy w tym wątku
                int end = start + count;//wiersz końcowy (nie włącznie)

                threads[i] = new Thread(() => MultiplyRange(start, end));//nowy wątek
                threads[i].Start(); //start wątku

                currentRow = end; //przesuń na kolejny start
            }

            foreach (var thread in threads)//czekaj aż wszystkie wątki zakończą
                thread.Join();

            watch.Stop(); //koniec pomiaru 
            //return watch.ElapsedMilliseconds; //Zwróć czas
            return (long)watch.Elapsed.TotalMicroseconds;
        }

// Pomocnicza metoda – mnoży podany zakres wierszy
        private void MultiplyRange(int startRow, int endRow) //od startRow do endRow
        {
            for (int i = startRow; i < endRow; i++) //Wiersze do przetworzenia 
            {
                for (int j = 0; j < _size; j++)//kolumny
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