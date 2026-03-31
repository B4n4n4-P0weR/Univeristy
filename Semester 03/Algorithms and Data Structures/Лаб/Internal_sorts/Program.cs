// Внутренние сортировки
// Туровец В.Ю.
// 2 янв 2023

using System.Diagnostics;

namespace Internal_sorts
{
    class Disk
    { 
        // -название альбома;
        // -исполнитель;
        // -год выхода. (поле сортировки)
        
        private static string Path = "" //Путь до файла масива записей
        private static (List<string>, List<string>, List<int>) Texter()//делает готовые массивы дат, исполнителей и названий на основе текста из файла
        {
            var listOfAlbums = new List<string>();
            var listOfNames = new List<string>();
            var listOfyears = new List<int>();
            
            for (int i = 0; File.ReadLines(Path).ElementAtOrDefault(i) != null; i++)
            {
                string? readText = File.ReadLines(Path").ElementAtOrDefault(i);
                readText = readText.Replace("; ", ";"); //замена 
                string[] text = (readText.Split(';').ToArray());
                int j = 0;
                foreach (var part in text)
                {
                    j++;
                    //Console.WriteLine(j);
                    if (j == 1){listOfAlbums.Add(part);}
                    if (j == 2){listOfNames.Add(part);}
                    if (j == 3){int.TryParse(part, out var year);listOfyears.Add(year);}
                    
                }
            }
            
            return (listOfAlbums, listOfNames, listOfyears);
        }

        public static void DiskSort()
        {
            var listOfAlbums = new List<string>(); //Названия всех альбомов
            var listOfNames = new List<string>();  //Имена всех исполнителей
            var listOfyears = new List<int>();     //Года выпуска альбомов
            
            (listOfAlbums, listOfNames, listOfyears) = Texter();

            Console.WriteLine("До сортировки");
            for (int i = 0; i < listOfNames.Count; i++)
            {
                Console.Write($"{listOfAlbums[i]}:{listOfNames[i]}:{listOfyears[i]}; ");
            }
            
            listOfyears = QuickSort(listOfyears, 0, listOfyears.Count - 1);

            List<int> QuickSort(List<int> array, int minIndex, int maxIndex)
            {
                if (minIndex >= maxIndex)
                {
                    return array;
                }

                int pivotIndex = GetPivotIndex(array, minIndex, maxIndex);

                QuickSort(array, minIndex, pivotIndex - 1);

                QuickSort(array, pivotIndex + 1, maxIndex);

                return array;
            }

            int GetPivotIndex(List<int> array, int minIndex, int maxIndex)
            {
                int pivot = minIndex - 1;

                for (int i = minIndex; i <= maxIndex; i++)
                {
                    if (array[i] < array[maxIndex])
                    {
                        pivot++;
                        (listOfyears[pivot], listOfyears[i]) = (listOfyears[i], listOfyears[pivot]);       //Замена местами годов
                        (listOfNames[pivot], listOfNames[i]) = (listOfNames[i], listOfNames[pivot]);       //Замена местами названий альбовом
                        (listOfAlbums[pivot], listOfAlbums[i]) = (listOfAlbums[i], listOfAlbums[pivot]);   //Замена местами имён исполнителей
                    }
                }

                pivot++;
                //меняем значения местами
                (listOfyears[pivot], listOfyears[maxIndex]) = (listOfyears[maxIndex], listOfyears[pivot]);       //Замена местами годов
                (listOfNames[pivot], listOfNames[maxIndex]) = (listOfNames[maxIndex], listOfNames[pivot]);       //Замена местами названий альбовом
                (listOfAlbums[pivot], listOfAlbums[maxIndex]) = (listOfAlbums[maxIndex], listOfAlbums[pivot]);   //Замена местами имён исполнителей
                return pivot;
            }
            
            Console.WriteLine("\nПосле сортировки");
            
            for (int i = 0; i < listOfNames.Count; i++)
            {
                Console.Write($"{listOfAlbums[i]}:{listOfNames[i]}:{listOfyears[i]}; ");
            }
        }
        

    }
    
    class Program
    {
        private const int Minimal = -10; //минимальный возможный элемент 
        private const int Maximum = 75; //максимальный возможный элемент 

        public static void Swap(ref int e1, ref int e2) {(e1, e2) = (e2, e1);} //обмен элементов
        
        //Конвертер из строки в массив
        private static int[] Texter(int ask)//делает готовый массив чисел на основе текста из файла
        {
            
            string readText = File.ReadLines("/Users/vlad/Desktop/Сортировки [Алг 1 - 2 лаб]/Fitst_Second_labs/Internal_sorts/Input_mas.txt").ElementAtOrDefault(ask - 1);
            //Console.WriteLine(readText);
            readText = readText.Replace("; ", " "); //замена 
            var Text = readText.Split(' ').Select(int.Parse).ToArray();
            return Text;
        }

        //Сортировка простыми вставками
        static int[] EasyInserts(int[] array)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int comparisons = 0;    //кол-во сравнений
            int rearrangements = 0; //кол-во перестановок

            for (var i = 0; i < array.Length; i++) //Проходим по массиву слева направо и обрабатываем по очереди каждый элемент.
            {
                var key = array[i];
                var j = i;
                rearrangements++;
                //Слева от элемента наращиваем отсортированную часть массива,
                //справа по мере процесса потихоньку испаряется неотсортированная.
                while ((j >= 1) && (array[j - 1] > key))
                {
                    {
                        //В отсортированной части массива ищется точка вставки для очередного элемента.
                        comparisons++;
                        Swap(ref array[j - 1], ref array[j]);
                        j--;
                    }
                }

                array[j] = key;
            }
            
            watch.Stop();
            var elapsedMs = watch.ElapsedTicks;
            Console.WriteLine($"{comparisons}    | {rearrangements}   | {elapsedMs} Сортировка простыми вставками");
            for (var i = 0; i < 10; i++){Console.Write($"{array[i]} ");} 
            return array;
        }

        //Сортировка простым обменом
        static int[] Bubble(int[] array)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int comparisons = 0;    //кол-во сравнений
            int rearrangements = 0; //кол-во перестановок

            var len = array.Length;
            for (var i = 1; i < len; i++)
            {
                for (var j = 0; j < len - i; j++)
                {
                    comparisons++;
                    if (array[j] > array[j + 1])
                    {
                        Swap(ref array[j], ref array[j + 1]);
                        rearrangements++;
                    }
                }
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedTicks;
            Console.WriteLine($"\n{comparisons}   | {rearrangements}   | {elapsedMs} Сортировка простым обменом");
            for (var i = 0; i < 10; i++){Console.Write($"{array[i]} ");} 
            return array;
        }
        
        //Сортировка простым выбором
        static int[] SelectionSort(int[] array)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int comparisons = 0;    //кол-во сравнений
            int rearrangements = 0; //кол-во перестановок

            int min, temp;

            for (int i = 0; i < array.Length - 1; i++)
            {
                min = i; //устанавливаем начальное значение минимального индекса

                //находим минимальный индекс элемента
                for (int j = i + 1; j < array.Length; j++)
                {
                    comparisons++;
                    if (array[j] < array[min])
                    {
                        min = j;
                    }
                }
                rearrangements++;
                //меняем значения местами
                Swap(ref array[min], ref array[i]);
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedTicks;
            Console.WriteLine($"\n{comparisons}   | {rearrangements}   | {elapsedMs} Сортировка простым выбором");
            for (var i = 0; i < 10; i++){Console.Write($"{array[i]} ");} 
            return array;
        }
        //Сортировка бинарными вставками
        static int[] BinInsertSort(int[] array)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int comparisons = 0;    //кол-во сравнений
            int rearrangements = 0; //кол-во перестановок
            
            for (int i = 1; i < array.Length; i++) {
                int j;
                int x = array[i];
                for (j = i; j > 0 && array[j - 1] > x; j--)
                {
                    comparisons++;
                    array[j] = array[j - 1];
                }
                rearrangements++;
                array[j] = x;
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedTicks;
            Console.WriteLine($"\n{comparisons}   | {rearrangements}   | {elapsedMs/1000} Сортировка бинарными вставками");
            for (var c = 0; c < 10; c++){Console.Write($"{array[c]} ");} 
            return array;
        }
        
        //Быстрая сортировка
        static int[] QuickSort(int[] array)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int comparisons = 0;    //кол-во сравнений
            int rearrangements = 0; //кол-во перестановок

            array = QuickSort(array, 0, array.Length - 1); 
            //Для простоты метод получает на вход границы участка массива от минимального до максимального
            int[] QuickSort(int[] quickSortingArray, int leftIndex, int rightIndex) 
            {
                if (leftIndex >= rightIndex){return quickSortingArray;} //Если левая граница правее правой - возвращаем масив

                int pivotIndex = GetPivotIndex(quickSortingArray, leftIndex, rightIndex);//Стержневой индекс 

                //Делим масив на две часты (слева и справа от стержня)
                //Соритруем слева от стержня
                QuickSort(quickSortingArray, leftIndex, pivotIndex - 1);
                //Соритруем справа от стержня
                QuickSort(quickSortingArray, pivotIndex + 1, rightIndex);

                return quickSortingArray;
            }

            int GetPivotIndex(int[] pivotingArray, int leftIndex, int rightIndex)//Стержневой индекс 
            {
                int pivot = leftIndex - 1;

                for (int i = leftIndex; i <= rightIndex; i++)
                {
                    comparisons++;
                    if (pivotingArray[i] < pivotingArray[rightIndex])
                    {
                        pivot++;
                        Swap(ref pivotingArray[pivot], ref pivotingArray[i]);
                        rearrangements++;
                    }
                }

                pivot++;
                Swap(ref pivotingArray[pivot], ref pivotingArray[rightIndex]);

                return pivot;
            }


            watch.Stop();
            var elapsedMs = watch.ElapsedTicks;
            Console.WriteLine($"\n{comparisons}     | {rearrangements}   | {elapsedMs/1000} Быстрая сортировка");
            for (var i = 0; i < 10; i++){Console.Write($"{array[i]} ");} 
            return array;
        }

        //Генератор сулчайных чисел

        static void Main()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Выберите тип сортировки");
                Console.WriteLine("1 - Числа");
                Console.WriteLine("2 - Диски");
                Console.WriteLine("0 - Выход");
                int.TryParse(Console.ReadLine(), out var how); if (how == 0) {break;} if (how > 2) {continue;}

                switch (how)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("Выберите количество тестовых чисел");
                        Console.WriteLine("1 - 500");
                        Console.WriteLine("2 - 1000");
                        Console.WriteLine("3 - 5000");
                        Console.WriteLine("0 - Назад");
                        int.TryParse(Console.ReadLine(), out var ask); if (ask == 0) {continue;} if (ask > 3) {continue;}

                        int[] numbersArray = Texter(ask);//Готовый массив чисел на основе текста из файла
                        for (var i = 0; i < 10; i++){Console.Write($"{numbersArray[i]} ");} //Вывод неотсортированного массива


                        Console.Clear();
                        Console.WriteLine($"comparisons | rearrangements | time ");
                        EasyInserts(numbersArray);

                        numbersArray = Texter(ask); //Приведение масива к неотсортированному виду
                        Bubble(numbersArray); 
                        
                        numbersArray = Texter(ask); //Приведение масива к неотсортированному виду
                        SelectionSort(numbersArray);
                        
                        numbersArray = Texter(ask); //Приведение масива к неотсортированному виду
                        BinInsertSort(numbersArray); 
                        
                        numbersArray = Texter(ask); //Приведение масива к неотсортированному виду
                        QuickSort(numbersArray); 
                        break;
                    
                    case 2:
                        Console.Clear();
                        Disk.DiskSort();
                        break;
                }
                int.TryParse(Console.ReadLine(), out var waiter);
            } while (true);
        }
    }
}