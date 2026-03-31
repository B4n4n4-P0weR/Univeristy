// 
// Туровец В.Ю.
// 2 янв 2022


using System.Collections;

namespace Round_Queue
{
    internal class Program
    {
        private const int MainLength = 6;  //количество человек состава

        private const string Path = "/Users/vlad/Desktop/Кольцевая очередь [Алг 1.3 прак]/Third_practice/Round_Queue/Input.txt";
        
        public static (RoundQueue, RoundQueue) Add(RoundQueue AddingMainQueue, RoundQueue AddingBackQueue, int ask)
        {
            if (ask == 1)
            {
                Console.WriteLine("Введите имена игроков основного состава");
                for (int i = 0; i < 5; i++) {AddingBackQueue.Enqueue(Console.ReadLine());}
                
                Console.WriteLine("Введите имена игроков запасного состава");
                for (int i = 0; i < 5; i++) {AddingMainQueue.Enqueue(Console.ReadLine());}
            }
            else
            {
                if (ask == 2)
                {
                    string? readText = File.ReadLines(Path).ElementAtOrDefault(0);
                    readText = readText.Replace(", ", ",");   //замена 
                    string[] text = (readText.Split(',').ToArray()); //разделение
                
                    foreach (var part in text)
                    {
                        AddingMainQueue.Enqueue(part);
                    }
                    readText = File.ReadLines(Path).ElementAtOrDefault(1);
                    readText = readText.Replace(", ", ",");   //замена 
                    text = (readText.Split(',').ToArray()); //разделение
                
                    foreach (var part in text)
                    {
                        AddingBackQueue.Enqueue(part);
                    }
                }
                if (ask == 3)
                {
                    string? readText = File.ReadLines(Path).ElementAtOrDefault(2);
                    readText = readText.Replace(", ", ",");   //замена 
                    string[] text = (readText.Split(',').ToArray()); //разделение
                
                    foreach (var part in text)
                    {
                        AddingMainQueue.Enqueue(part);
                    }
                    readText = File.ReadLines(Path).ElementAtOrDefault(3);
                    readText = readText.Replace(", ", ",");   //замена 
                    text = (readText.Split(',').ToArray()); //разделение
                
                    foreach (var part in text)
                    {
                        AddingBackQueue.Enqueue(part);
                    }
                }

                
            }
            return (AddingMainQueue, AddingBackQueue);
        }

        public static (RoundQueue, RoundQueue) Swaper(RoundQueue SwapingMainQueue, RoundQueue SwapingBackQueue, int replacementCount, int recalculationCount)
        {
            int recalculated = recalculationCount; //каждый какой-то элемент

            int j = 0;
            for (int i = 0; i < replacementCount; i++)
            {
                if (j > 5){j %= 5;}
                
                if (recalculated > 5) { recalculated %= 5; } //если элемент больше количества игроков 
                if (recalculated == 0) { recalculated = 5; } //если элемент больше 5, но кратен 10
                Console.WriteLine($"Число пересчётов {recalculated}");


                string swaper = SwapingMainQueue.Element(recalculated-1);
                SwapingMainQueue.Swap(recalculated-1, SwapingBackQueue.Element(j));
                SwapingBackQueue.Swap(j, swaper);
                
                recalculated += recalculated;
                j++;
            }
            
            return (SwapingMainQueue, SwapingBackQueue);
        }


        public static void Main(string[] args)
        {
            do
            {
                Console.Clear();
                //Создать два кольцевых списка
                RoundQueue mainQueue = new RoundQueue();
                RoundQueue backQueue = new RoundQueue();

                //Записать фамилии баскетболистов основного и запасного состава
                Console.WriteLine("1 - ручной ввод");
                Console.WriteLine("2 - Chicago Bulls");
                Console.WriteLine("3 - Los-Angeles Lakers");
                int.TryParse(Console.ReadLine(), out var ask);
                if ((ask > 3) || (ask == 0)){ continue;}
                (mainQueue,backQueue) =Add(mainQueue, backQueue, ask);

                //Вывети фамилии баскетболистов основного и запасного состава
                mainQueue.Display();
                backQueue.Display();
            
                if ((backQueue.IsEmpty()) || (mainQueue.IsEmpty())){ continue;}
                
                //Считать K и T
                Console.Write("Введите число K (количество замен)");
                int.TryParse(Console.ReadLine(), out var replacementCount);//количество замен (число K)
                Console.Write("Введите число T (количество пересчётов)");
                int.TryParse(Console.ReadLine(), out var recalculationCount);//число пересчёта (число Т)

                //Произвести K замен с числом для пересчётов T (для каждого T эементка)
                Swaper(mainQueue, backQueue, replacementCount, recalculationCount);
                int.TryParse(Console.ReadLine(), out var waiter);


                //Вывести списки составов
                mainQueue.Display();
                backQueue.Display();
                int.TryParse(Console.ReadLine(), out waiter);
            } while (true);
        }
    }
}