namespace Hash
{
    internal class Program 
    {
        
        public static long CollapseHash(long data, long hashLength)
        {
            //Складывает все цыфры
            long hash = 0;

            while (data != 0)
            {
                hash += data % hashLength;
                data /= hashLength;
            }

            return hash % hashLength;

        }

        public static long MiddleSquaresHash(long data, long hashLength)
        {
            //Возводит в квадрат и берёт единицу 
            data = data * data;

            int resLength = (int)Math.Log10(hashLength) + (hashLength % 10 != 0 ? 1 : 0 );
            int dataLength = (int)Math.Log10(data) + (data % 10 != 0 ? 1 : 0 );
            int shift = (dataLength - resLength) / 2;
            
            string s_hash = data.ToString();

            s_hash = s_hash.Substring(shift , resLength);

            return Convert.ToInt64(s_hash);
        }

        
        //Два метода которые сумируют все символы, превращая Disk в число и после передают в методы,
        //которые ищут хэш числа (с нагрузкой внутри)
        public static long CollapseHash(Disk disk, long hashLenght)
        {
            long data = 0;

            foreach (var AlbumChar in disk.AlbumName) data += (int)AlbumChar;
            foreach (var ArtistChar in disk.ArtistName) data += (int)ArtistChar;
            data += disk.Year;

            return CollapseHash(data, hashLenght);
        }
        
        public static long MakeMiddleSquaresHash(Disk disk, long hashLenght)
        {
            long data = 0; 

            foreach (var AlbumChar in disk.AlbumName) data += (int)AlbumChar;
            foreach (var ArtistChar in disk.ArtistName) data += (int)ArtistChar;

            data += disk.Year;

            return MiddleSquaresHash(data, hashLenght);
        } 


        class HashTable
        {

            public HashTable(int hashLength)
            {
                this.hashLength = hashLength;

                table = new List<Disk>[hashLength];

                for (int i = 0; i < hashLength; i++) table[i] = new List<Disk>();

            }


            public void AddCollapse(Disk newValue)
            {
                var hash = CollapseHash(newValue , hashLength);

                table[hash].Add(newValue);
            }

            public void AddMiddle(Disk newValue)
            {
                var hash = MakeMiddleSquaresHash(newValue , hashLength);

                table[hash].Add(newValue);
            }

            public bool FindCollapse(Disk findElement)
            {
                var hash = CollapseHash(findElement , hashLength);

                return table[hash].Contains(findElement);
            }
            
            public bool FindMidle(Disk findElement)
            {
                var hash = MakeMiddleSquaresHash(findElement , hashLength);

                return table[hash].Contains(findElement);
            }

            public bool RemoveCollapse(Disk removeElement)
            {
                var hash = CollapseHash(removeElement , hashLength);

                if (!table[hash].Contains(removeElement)) return false;

                table[hash].Remove(removeElement);
                return true;
            }
            
            public bool RemoveMidle(Disk removeElement)
            {
                var hash = MakeMiddleSquaresHash(removeElement , hashLength);

                if (!table[hash].Contains(removeElement)) return false;

                table[hash].Remove(removeElement);
                return true;
            }

            public void Print()
            {
                for (int i = 0; i < hashLength; i++)
                {
                    {Console.Write($"{i+1}");
                    foreach (var elem in table[i].ToArray())
                    {
                        if (elem != null)  Console.Write($" -> {elem}");}
                    }
                    Console.WriteLine();
                }
            }

            private List<Disk>[] table;
            private int hashLength;
        }

        private static readonly string Path = ""; //Путь до файла с массивом записей
        
        public static void Main(string[] args)
        {
            //Создание хэш-таблицы
            HashTable hashTable = new HashTable(20);

            Console.WriteLine("1 - Метод срединных квадратов");
            Console.WriteLine("2 - Колапс");
            int.TryParse(Console.ReadLine(), out var type);
            if (type == 1)
            {
                var final = false;
            do
            {
                //Разделение из файла на отдельные элементы класа Disks
                for (int i = 0; File.ReadLines(Path).ElementAtOrDefault(i) != null; i++) //Из файла в hash
                {
                    string? readText = File.ReadLines(Path).ElementAtOrDefault(i);
                    string[] text = (readText.Split( '\n').ToArray());
                
                    foreach (var part in text)
                    {
                        var temp =  part.Split("; "); 
                    
                        string AlbumName, AutorName; int year;
                        AlbumName = temp[0]; AutorName = temp[1]; year = Int32.Parse(temp[2]);
                    
                        hashTable.AddMiddle(new Disk(){AlbumName = AlbumName, ArtistName = AutorName ,  Year = year});
                    
                    }
                }
                Console.Clear();
                Console.WriteLine("1 - Добавить элемент");
                Console.WriteLine("2 - Убрать элемент");
                Console.WriteLine("3 - Поиск элемента");
                Console.WriteLine("4 - Вывод таблицы");
                
                Console.WriteLine("0 - Выход.");
                
                string albumNameTemp, autorNameTemo; int yearTemp;
                int.TryParse(Console.ReadLine(), out var switcher);
                switch (switcher)
                {
                    case 1:
                        Console.Clear();
                        var added = Console.ReadLine();
                        var temp =  added.Split("; ");
                        albumNameTemp = temp[0]; autorNameTemo = temp[1]; yearTemp = Int32.Parse(temp[2]);
                        hashTable.AddMiddle(new Disk() {AlbumName = albumNameTemp, ArtistName = autorNameTemo ,  Year = yearTemp});
                        break;
                    case 2:
                        Console.Clear();
                        var removed = Console.ReadLine();
                        temp =  removed.Split("; ");
                        albumNameTemp = temp[0];
                        autorNameTemo = temp[1];
                        yearTemp = Int32.Parse(temp[2]);
                        if (hashTable.RemoveMidle(new Disk()
                                { AlbumName = albumNameTemp, ArtistName = autorNameTemo, Year = yearTemp }))
                        {
                            Console.WriteLine("Элемент удалён");
                        }
                        else
                        {
                            Console.WriteLine("Такого элемента нет");
                        }
                        
                        int.TryParse(Console.ReadLine(), out var waiter); //ожидает пока пользователь нажмёт на enter
                        break;
                    case 3:
                        Console.Clear();
                        var finded = Console.ReadLine();
                        temp =  finded.Split("; ");
                        albumNameTemp = temp[0];
                        autorNameTemo = temp[1];
                        yearTemp = Int32.Parse(temp[2]);
                        if ( hashTable.FindMidle(new Disk() { AlbumName = albumNameTemp, ArtistName = autorNameTemo, Year = yearTemp }) )
                        {
                            Console.WriteLine("Элемент присутствует");
                        }
                        else
                        {
                            Console.WriteLine("Элемент отсутствует");
                        }
                        
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                        
                    case 4:
                        Console.Clear();
                        hashTable.Print();
                        
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 0:
                        final = true;
                        break;
                    case >3:
                        break;
                }
                
            } while (!final);
            }
            else{var final = false;
            do
            {
                //Разделение из файла на отдельные элементы класа Disks
                for (int i = 0; File.ReadLines(Path).ElementAtOrDefault(i) != null; i++) //Из файла в hash
                {
                    string? readText = File.ReadLines(Path).ElementAtOrDefault(i);
                    string[] text = (readText.Split( '\n').ToArray());
                
                    foreach (var part in text)
                    {
                        var temp =  part.Split("; "); 
                    
                        string AlbumName, AutorName; int year;
                        AlbumName = temp[0]; AutorName = temp[1]; year = Int32.Parse(temp[2]);
                    
                        hashTable.AddCollapse(new Disk(){AlbumName = AlbumName, ArtistName = AutorName ,  Year = year});
                    
                    }
                }
                Console.Clear();
                Console.WriteLine("1 - Добавить элемент");
                Console.WriteLine("2 - Убрать элемент");
                Console.WriteLine("3 - Поиск элемента");
                Console.WriteLine("4 - Вывод таблицы");
                
                Console.WriteLine("0 - Выход.");
                
                string albumNameTemp, autorNameTemp; int yearTemp;
                int.TryParse(Console.ReadLine(), out var switcher);
                switch (switcher)
                {
                    case 1:
                        Console.Clear();
                        var added = Console.ReadLine();
                        var temp =  added.Split("; ");
                        albumNameTemp = temp[0]; autorNameTemp = temp[1]; yearTemp = Int32.Parse(temp[2]);
                        hashTable.AddCollapse(new Disk() {AlbumName = albumNameTemp, ArtistName = autorNameTemp ,  Year = yearTemp});
                        break;
                    case 2:
                        Console.Clear();
                        var removed = Console.ReadLine();
                        temp =  removed.Split("; ");
                        albumNameTemp = temp[0];
                        autorNameTemp = temp[1];
                        yearTemp = Int32.Parse(temp[2]);
                        if(hashTable.RemoveCollapse(new Disk()
                               { AlbumName = albumNameTemp, ArtistName = autorNameTemp, Year = yearTemp }))
                        {
                            Console.WriteLine("Элемент удалён");
                        }
                        else
                        {
                            Console.WriteLine("Такого элемента нет");
                        }
                        
                        int.TryParse(Console.ReadLine(), out var waiter); //ожидает пока пользователь нажмёт на enter
                        break;
                    case 3:
                        Console.Clear();
                        var finded = Console.ReadLine();
                        temp =  finded.Split("; ");
                        albumNameTemp = temp[0];
                        autorNameTemp = temp[1];
                        yearTemp = Int32.Parse(temp[2]);
                        if ( hashTable.FindCollapse(new Disk() { AlbumName = albumNameTemp, ArtistName = autorNameTemp, Year = yearTemp }) )
                        {
                            Console.WriteLine("Элемент присутствует");
                        }
                        else
                        {
                            Console.WriteLine("Элемент отсутствует");
                        }
                        
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                        
                    case 4:
                        Console.Clear();
                        hashTable.Print();
                        
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 0:
                        final = true;
                        break;
                    case >3:
                        break;
                }
                
            } while (!final);}
            
            
        }
    }
}