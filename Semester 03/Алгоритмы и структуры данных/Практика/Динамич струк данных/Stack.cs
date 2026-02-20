// Пример работы методов класса Stack<T>
// 
// 

using System;
using System.Collections.Generic;

namespace Stack
{
    internal class Program
    {

        private static int LetterCounter(string stackLine) // счётчик букв
        {
            int lCount = 0; //переменная-счётчик букв
            stackLine = stackLine.ToLower(); //приведение строки к нижнему регистру
            char[] letters = new char[]  //массив английских и русских букв 
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                
                'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л',
                'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш',
                'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я'
            };
            
            foreach (var stackChar in stackLine) //для каждой буквы в строке стека
            {
                foreach (var letter in letters) //для каждой буквы в массиве букв
                {
                    if (letter == stackChar) { lCount++; } //если буква в строке стека = букве в массиве букв прибавить к переменной-счётчику букв
                }
            }
            return lCount;
        }

        private static Stack<string> Reversing(Stack<string> incomeStack) // переворачивание стека
        {
            //в стек все элементы попадают с головы и если всунуть элементы уже всунутые с головы они окажутся всунутыми снизу
            Stack<string> reversedStack = new Stack<string>(incomeStack);
            return reversedStack;
        }
        
        private static void Adding(Stack<string> addedStack, int howMuch) // добавление в стек
        {
            Console.Clear();
            for (int i = 0; i < howMuch; i++) 
            {
                Console.Write("Введите строку:");
                var _ = Console.ReadLine();
                if (_ != null) addedStack.Push(_);
            }
        }

        private static void Outputting(Stack<string> outputStack, bool last = false) // вывод стека
        {
            if (last)
            {
                outputStack = Reversing(outputStack); //переворот для красоты отображения
                Console.WriteLine($"Последний занесённый элемент {outputStack.Peek()}");
                outputStack = Reversing(outputStack);
            }
            
            else
            {
                int i = 0; //переменная-счётчик
                Console.WriteLine($"Кол-во элементов в стеке равно {outputStack.Count}");
                foreach (var stacker in outputStack)
                {
                    i++;
                    Console.WriteLine($"{i}. {stacker}");
                }
            }
        }
        
        
        public static void Main(string[] args)
        {
            Console.Title = "Stack";
            Console.Clear();
            
            const int length = 7; // изначальная длина стека

            Stack<string> mainStack = new Stack<string>(length); // создание стека 

            Adding(mainStack, length);
            
            mainStack = Reversing(mainStack);

            var final = false; //переменная отвечающая за окончание <do{} while()>
            do
            {
                Console.Clear();

                Console.WriteLine("Выберите действие: ");
                Console.WriteLine("1 - Вывод всех элементов стека");
                Console.WriteLine("2 - Давление элемента в стек");
                Console.WriteLine("3 - Извлечение элемент из вершины стека");
                Console.WriteLine("4 - Подсчёт количества гласных в строке");
                Console.WriteLine("5 - Вывод на экран последнего занесённого элемента");
                Console.WriteLine("6 - Проверка на наличие элемента в стеке");
                Console.WriteLine("7 - Проверка на пустоту/наполненность");
                Console.WriteLine("8 - Удаление слов из стека, пока не встретится слово, содержащие чётное кол-во гласных");
                Console.WriteLine("9 - Удаление стека");
                
                Console.WriteLine("0 - Выход");

                int.TryParse(Console.ReadLine(), out var switcher);
                switch (switcher)
                {
                    case 1: //Вывод всех элементов стека
                        Console.Clear();
                        Outputting(mainStack);
                        int.TryParse(Console.ReadLine(), out var waiter); //до тех пор пока пользователь не нажмёт enter окно не закроется
                        break;
                    case 2: //Давление элемента в стек
                        Console.Clear();
                        mainStack = Reversing(mainStack);
                        Adding(mainStack, 1);
                        mainStack = Reversing(mainStack);
                        break;
                    case 3: //Извлечение элемент из вершины стека
                        Console.Clear();
                        string popped = mainStack.Pop();
                        Console.WriteLine($"Из вершины стека извлечён элемент {popped}");
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 4: //Подсчёт количества букв в строке
                        Console.Clear();
                        foreach (string stackLine in mainStack)
                        {
                            int LetterCount = LetterCounter(stackLine);
                            Console.WriteLine($"{stackLine}: {LetterCount}");
                        }
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 5: //Вывод на экран последний занесённый элемент
                        Console.Clear();
                        Outputting(mainStack, true);
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 6: //Проверка на наличие элемента в стеке
                        Console.Clear();
                        Console.WriteLine ("Введите строку, которую нужно найти");
                        string desiredItem = Console.ReadLine(); // искомая строка
                        Console.WriteLine(desiredItem != null && mainStack.Contains(desiredItem) ? "Элемент присутствует в стеке" : "Элемент отсутствует в стеке");
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 7: //Проверка на пустоту/наполненность
                        Console.Clear();
                        if (mainStack.Count > 0)
                        {
                            Console.WriteLine("Стек наполнен");
                        }
                        else
                        {
                            Console.WriteLine("Стек не наполнен");
                        }
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 8: //Удаление слов из стека, пока не встретится слово, содержащие чётное кол-во букв
                        Console.Clear();
                        
                        var redundantArray = new int[mainStack.Count()]; //создание массива с посчитанным кол-вом глассных
                        var _ = 0; //переменная-счётчик
                        foreach (string stackLine in mainStack) 
                        {
                            int letterCount = LetterCounter(stackLine); //количество  в строке <stackLine>
                            redundantArray[_] = letterCount; //запись количества гласных в массив
                            _++;
                        }
                        
                        //в массиве записано кол-во  для каждой строки стека
                        
                        foreach (var countedLetters in redundantArray)
                        {
                            if (countedLetters != 0 && countedLetters % 2 != 0)//если количество букв в строке не равно нулю и не чётно
                            {
                                break;
                            }
                            else
                            {
                                string noVowelPopped = mainStack.Pop();
                                Console.WriteLine($"Из вершины стека извлечён элемент {noVowelPopped}");
                            }
                        }

                        Outputting(mainStack);
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 9: //Удаление стека
                        Console.Clear();
                        mainStack.Clear();
                        Console.WriteLine("Стек очищен");
                        Outputting(mainStack);
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