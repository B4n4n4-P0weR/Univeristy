namespace Search
{
    internal class Disk
    {
        private static readonly string Path = " "; //путь до файла с массивом записей
        private static (List<string>, List<string>, List<int>) Texter()//делает готовые массивы дат, исполнителей и названий на основе текста из файла
        {
            var listOfAlbums = new List<string>();
            var listOfNames = new List<string>();
            var listOfyears = new List<int>();

            for (int i = 0; File.ReadLines(Path).ElementAtOrDefault(i) != null; i++)
            {
                string? readText = File.ReadLines(Path).ElementAtOrDefault(i);
                readText = readText.Replace("; ", ";"); //замена 
                string[] text = (readText.Split(';').ToArray());
                int j = 0;
                foreach (var part in text)
                {
                    j++;
                    if (j == 1) { listOfAlbums.Add(part); }
                    if (j == 2) { listOfNames.Add(part); }
                    if (j == 3) { int.TryParse(part, out var year); listOfyears.Add(year); }
                }
            }

            return (listOfAlbums, listOfNames, listOfyears);
        }

        static List<int> DiskSort()
        {

            (List<string> listOfAlbums, List<string> listOfNames, List<int> listOfyears) = Texter(); 
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

            for (int i = 0; i < listOfNames.Count; i++)
            {
                Console.WriteLine($"{i+1}. {listOfAlbums[i]}:{listOfNames[i]}:{listOfyears[i]}; ");
            }
            Console.WriteLine("");

            return listOfyears;
        }
        
        public static void DiskSearch()
        {
            
            //1 - запуск Texter -> делает готовые массивы дат, исполнителей и названий на основе текста из файла
            //2 - запуск DiskSort -> сортировка масива записей по дате 
            
            List<int> listOfyears = DiskSort();

            Console.WriteLine("Введите элемент поиска в массиве записей");
            int.TryParse(Console.ReadLine(), out var ask);
            Console.WriteLine(Search.IndexSearch(listOfyears.ToArray(), ask).index+1);
        }
    }
    internal class Search
    {
        private static int _indexComp = 0;
        public static (int index,int compare) IndexSearch(int[] arr, int key)
        {
            int m = (int)Math.Sqrt(arr.Length);
            Dictionary<int, int> maxValues = new Dictionary<int, int>();
            for (int i = m-1; i < arr.Length; i+=m)
            {
                maxValues[i]=arr[i];
            }
            if (arr.Length % m != 0)
            {
                maxValues[arr.Length-1] = arr[^1];
            }

            int startKey = 0, endKey = 0;

            foreach (var elem in maxValues)
            {
                _indexComp++;
                if (key <= elem.Value)
                {
                    endKey = elem.Key;
                    break;
                }

                startKey = elem.Key;
            }

            for (int i = startKey; i <= endKey; i++)
            {
                _indexComp++;
                if (key == arr[i]) return (i, _indexComp);
            }

            return (-1, _indexComp);
        }

        public static int InterpollComp = 0;
        
        public static int InerpollSearch(int[] inputArray, int key, int start, int end)
        {
            if (start > end) return -1;
            InterpollComp++;
            if ( (key < inputArray[start]) || (key > inputArray[end]) ) return -1;
            
            int middle = start + ((key - inputArray[start])*(end - start) / (inputArray[end] - inputArray[start]));
            InterpollComp++;
            if (inputArray[middle] == key) return middle;
            
            if (key < inputArray[middle]) return InerpollSearch(inputArray, key, start, middle-1);

            else{ return InerpollSearch(inputArray, key, middle+1, end);}
        }

        public static (int index,int compare) InerpollSearch(int[] InputArray, int key) //Нагрузка (не можем вызвать без начала и конца)
        { 
            InterpollComp = 0;
            return (InerpollSearch(InputArray, key, 0, InputArray.Length - 1), InterpollComp);
        }
    }

    
        internal class Program
        {
            private const string PathMass = " "; //путь до файла с массивом чисел

            static void Main()
            {
                do
                {
                    Console.Clear();
                    Console.WriteLine("1 - массив чисел");
                    Console.WriteLine("2 - массив записей");
                    int.TryParse(Console.ReadLine(), out var variant);
                    if (variant == 1)
                    {
                        Console.Clear();
                        
                        /*for (int i = 1; i <= 500; i++)
                        {
                            Random rnd = new Random();
                            int value = rnd.Next(-1000, 1000);
                            Console.Write($"{value}; ");
                        }*/
                        
                        
                        string readText = File.ReadLines(PathMass).ElementAtOrDefault(0);
                        readText = readText.Replace("; ", " "); //замена 
                        var numbers = readText.Split(' ').Select(int.Parse).ToArray();
                        
                        Console.WriteLine("\nОтсортированный массив чисел");
                        Array.Sort(numbers);
                        for (var i = 0; i < numbers.Length; i++){Console.Write($"{numbers[i]} ");} //Вывод отсортированного массива
                        
                        Console.WriteLine("\nВведите элемент поиска");
                        int.TryParse(Console.ReadLine(), out var ask);
                        var indexResult = Search.IndexSearch(numbers, ask);
                        var inerpollResult = Search.InerpollSearch(numbers, ask);
                        Console.WriteLine($"Индексный   индекс элемента {indexResult.index+1} кол-во сравнений{indexResult.compare}");
                        Console.WriteLine($"Интерполяц. индекс элемента {inerpollResult.index+1} кол-во сравнений{inerpollResult.compare}");
                        Console.WriteLine($"*не индексы масива, номера (всего 500)");
                
                        int.TryParse(Console.ReadLine(), out var waiter);
                    }

                    if (variant == 2)
                    {
                        Console.Clear();
                        Disk.DiskSearch();
                        int.TryParse(Console.ReadLine(), out var waiter);
                    }
                    else { continue; }
                    
                } while (true);
            }
        }
}
