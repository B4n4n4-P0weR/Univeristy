// Пример работы методов класса Queue<T>
// Туровец В.Ю.
// 22 сен 2022

using System;
using System.Collections.Generic;

namespace Queue
{
    internal class Program
    {
        private const int Length = 10;  //количество элементов 
        private const int Minimal = -7; //минимальный возможный элемент 
        private const int Maximum = 10; //максимальный возможный элемент 
        private static void Adding(Queue<int> addedQueue, int how, int howMuch)
        {
            switch (how)//выбор способа ввода
            {
                case 1: //Ручной ввод
                    Console.Clear();
                    for (int i = 1; i <= howMuch; i++)
                    {
                        Console.Write("Введите число:");
                        var readLine = Console.ReadLine();
                        var parsed = int.Parse(readLine);
                        if ((parsed > Minimal) & (parsed < Maximum) & (parsed != 0))
                        {
                            addedQueue.Enqueue(parsed);
                        }
                        else
                        {
                            Console.Write("Нет-нет-нет, другое число");
                            Adding(addedQueue, 1, 1);
                        }
                    }
                    break;
                case 2: //Генерация чисел
                    for (int i = 1; i <= howMuch; i++)
                    {
                        Random rnd = new Random();
                        int value = rnd.Next(Minimal, Maximum); //генерация случайного числа в диапазоне 
                        if (value != 0) {addedQueue.Enqueue(value);}
                        else {i = i - 1;}
                    }
                    break;
            }
        }
        
        private static int MinimumSearch(Queue<int> queueInSearch)
        {
            int mostMin = Maximum+1;
            foreach (int number in queueInSearch)
            {
                if (number < mostMin)
                {
                    mostMin = number;
                }
            }
            return mostMin;
        }
        
        private static void QueueJamming(Queue<int> jammedQueue)
        {
            foreach(int number in jammedQueue)
            {
                Console.Write(number);
                Console.Write(' ');
            }

        }

        public static void Main(string[] args)
        {
            Console.Title = "Queue";
            Console.Clear();
            
            Queue<int> mainQueue = new Queue<int>(Length); //объявление объекта класса Queue<int>
            mainQueue.Clear();

            Console.WriteLine("1 - ручной ввод");
            Console.WriteLine("2 - сгенерированные числа");
            int.TryParse(Console.ReadLine(), out var ask);
            Adding(mainQueue, ask, Length);

            bool final = false;
            do
            {
                Console.Clear();
                
                Console.WriteLine("Выберите действие: ");
                Console.WriteLine("1 - Вывод всех элементов очереди");
                Console.WriteLine("2 - Добавление элемента в конец очереди");
                Console.WriteLine("3 - Извлечение элемента из начала очереди");
                Console.WriteLine("4 - Вывод на экран элемента с головы очереди");
                Console.WriteLine("5 - Проверка на наличие элемента в очереди");
                Console.WriteLine("6 - Проверка на пустоту/наполненость");
                Console.WriteLine("7 - Поиск минимального числа и занесение в очередь");
                Console.WriteLine("8 - Удаление очереди");
                
                Console.WriteLine("0 - Выход.");
                
                int.TryParse(Console.ReadLine(), out var switcher);
                
                switch (switcher)
                {
                    case 1: //Вывод всех элементов очереди
                        Console.Clear();
                        QueueJamming(mainQueue);
                        int.TryParse(Console.ReadLine(), out var waiter); //до тех пор пока пользователь не нажмёт enter окно не закроется
                        break;
                    case 2: //Добавление элемента в конец очереди
                        Console.Clear();
                        Adding(mainQueue, 1, 1);
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 3: //Извлечение элемента из начала очереди
                        Console.Clear();
                        mainQueue.Dequeue();
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 4: //Вывод на экран элемента с головы очереди
                        Console.Clear();
                        mainQueue.Peek();
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 5: //Проверка на наличие элемента в очереди
                        Console.Clear();
                        do
                        {
                            Console.Clear();
                            Console.WriteLine("Введите элемент для поиска (для завершения введите -100)");
                            int.TryParse(Console.ReadLine(), out  var searcher);
                            
                            if (searcher == -100) {break;}
                            if (mainQueue.Contains(searcher)) {Console.WriteLine("Такой элемент есть");}
                            else {Console.WriteLine("Такого элемента нет");}

                            Console.WriteLine("Нажмите любую клавишу для продолжения");
                            Console.ReadKey();
                        } while (true);
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                        
                    case 6: //Проверка на пустоту/наполненость
                        Console.Clear();
                        if (mainQueue.Count > 0)
                        {
                            Console.WriteLine("Очередь наполнена");
                        }
                        else
                        {
                            Console.WriteLine("Очередь не наполнена");
                        }
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 7: //Поиск минимального числа и занесение в очередь
                        Console.Clear();
            
                        Console.WriteLine($"Минимальное число равно: {MinimumSearch(mainQueue)}");
                        mainQueue.Enqueue(MinimumSearch(mainQueue));
                        
                        QueueJamming(mainQueue);
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 8: //Удаление очереди
                        Console.Clear();
                        mainQueue.Clear();
                        Console.WriteLine("Очередь отчищена");
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 0: //Выход
                        Console.Clear();
                        final = true;
                        break;
                    default:
                        break;
                }

            } while (!final);
        }
    }
}