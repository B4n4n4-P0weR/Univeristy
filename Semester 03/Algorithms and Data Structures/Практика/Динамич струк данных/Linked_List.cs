// Пример работы методов класса LinkedList<T>
// Туровец В.Ю.
// 22 сен 2022

using System;
using System.Collections.Generic;

namespace Linked_List
{
    internal class Program
    {
        public const int length = 16;   //количество элементов 
        public const int minimal = -14; //минимальный возможный элемент 
        public const int maximum = 33;  //максимальный возможный элемент 

        private static void Adding(LinkedList<int> addedLinkedList, int howMuch = 1, int byHand = 1) //Добавление элемента
        {
            while (true)
            {
                switch (byHand) //выбор способа ввода
                {
                    case 1: //ручной ввод
                        Console.Clear();
                        LinkedListNode<int>? foundInList; //элемент после/до которого будет вставлено число
                        Console.WriteLine("Выберите условие для добавления");
                        Console.WriteLine("1 - добавить узел со значением в начало списка;");
                        Console.WriteLine("2 - добавить узел со значением в конец списка;");
                        Console.WriteLine("3 - добавить узел со значением после определённого элемента;");
                        Console.WriteLine("4 - добавить узел со значением перед определённым элементом;");
                        int.TryParse(Console.ReadLine(), out var outputSwitcher); //как добавлять
                        Console.Clear();
                        Console.Write("Введите число:");
                        int.TryParse(Console.ReadLine(), out var addingNumber); //что добавлять
                        if ((addingNumber > minimal) & (addingNumber < maximum) & (addingNumber != 0))
                        {
                            switch (outputSwitcher)
                            {
                                case 1: //добавить узел со значением в начало списка
                                    addedLinkedList.AddFirst(addingNumber);
                                    break;
                                case 2: //добавить узел со значением в конец списка
                                    addedLinkedList.AddLast(addingNumber);
                                    break;
                                case 3: //добавить узел со значением после определённого элемента
                                    Console.Clear();
                                    Console.Write("Введите элемент, после которого будет вставлено число:");
                                    int.TryParse(Console.ReadLine(), out var searchingNumber); //элемент, после которого будет вставлено число
                                    foundInList = addedLinkedList.Find(searchingNumber);
                                    if (foundInList != null) //проверка на не пустоту
                                    {
                                        addedLinkedList.AddAfter(foundInList, addingNumber);
                                    }
                                    else
                                        Console.WriteLine("Элемент не найден!");

                                    break;
                                case 4: //добавить узел со значением перед определённым элементом
                                    Console.Clear();
                                    Console.Write("Введите элемент, перед которым будет вставлено число:"); //элемент, перед которым будет вставлено число
                                    int.TryParse(Console.ReadLine(), out searchingNumber);
                                    foundInList = addedLinkedList.Find(searchingNumber);
                                    if (foundInList != null) //проверка на не пустоту
                                    {
                                        addedLinkedList.AddBefore(foundInList, addingNumber);
                                    }
                                    else
                                        Console.WriteLine("Элемент не найден!");

                                    break;
                            }
                        }
                        else
                        {
                            Console.Write("Нет-нет-нет, другое число\n");
                            howMuch = 1;
                            byHand = 1;
                            continue;
                        }

                        break;
                    case 2: //генерируемый ввод
                        for (var i = 1; i <= howMuch; i++)
                        {
                            var rnd = new Random();
                            addingNumber = rnd.Next(minimal, maximum); //генерация случайного числа в диапазоне 
                            if (addingNumber != 0)
                            {
                                addedLinkedList.AddFirst(addingNumber);
                            }
                            else
                            {
                                i = i - 1;
                            }
                        }

                        break;
                }

                break;
            }
        }

        private static void Deleting(LinkedList<int> editableList) //Удаление
        {
            Console.Clear();
                        Console.WriteLine("Выберите условие для удаления");
                        Console.WriteLine("1 - удалить узел с указанным значением;");
                        Console.WriteLine("2 - удалить первый узел;");
                        Console.WriteLine("3 - удалить последний узел;");
                        Console.WriteLine("4 - удалить все узлы;");
                        int.TryParse(Console.ReadLine(), out var switcher); //как удалять
                        
                        switch (switcher)
                        {
                            case 1: //удалить узел с указанным значением
                                Console.Clear();
                                Console.Write("Введите элемент, который будет удалён: ");
                                int.TryParse(Console.ReadLine(), out var searchingNumber);
                                LinkedListNode<int>? foundInList = editableList.Find(searchingNumber); //Поиск элемента в списке
                                if (foundInList != null) editableList.Remove(foundInList); //Проверка на наличие элемента в списке и удаление
                                else Console.WriteLine("Данный элемент отсутствует ");
                                break;

                            case 2: //удалить первый узел
                                Console.Clear();
                                if (editableList.Count > 0) //Если в листе есть элементы
                                {
                                    editableList.RemoveFirst();
                                    Console.WriteLine("Узел в начале удалён");
                                }
                                else Console.WriteLine("Список пуст");
                                break;
                            case 3: //удалить последний узел;
                                Console.Clear();
                                if (editableList.Count > 0) //Если в листе есть элементы
                                {
                                    editableList.RemoveLast();
                                    Console.WriteLine("Узел в конце удалён");
                                }
                                else Console.WriteLine("Список пуст");
                                break;
                            case 4: //удалить все узлы
                                Console.Clear();
                                if (editableList.Count > 0) //Если в листе есть элементы
                                {
                                    editableList.Clear();
                                    Console.WriteLine("Все узлы удалены");
                                }
                                else Console.WriteLine("Список пуст");
                                break;
                                
                        }
        }

        private static void MaximumSearch(LinkedList<int> listInSearch) //Вставка элемента после максимального
        {
            int mostMax = minimal - 1;
            foreach (int number in listInSearch)
            {
                if (number > mostMax)
                {
                    mostMax = number;
                }
            }
            
            LinkedListNode<int>? foundInList = listInSearch.Find(mostMax);
            if (foundInList != null)
            {
                Console.Write("Введите число:");
                int.TryParse(Console.ReadLine(), out var addingNumber);
                listInSearch.AddAfter(foundInList, addingNumber);
            }
        }
        
        public static void Main(string[] args)
        {
            Console.Title = "Linked List";
            Console.Clear();
            
            LinkedList<int> mainLinkedList = new LinkedList<int>(); mainLinkedList.Clear(); //объявление объекта класса LinkedList<int>
            

            Console.WriteLine("1 - ручной ввод");
            Console.WriteLine("2 - сгенерированные числа");
            int.TryParse(Console.ReadLine(), out var ask);
            Adding(mainLinkedList, length, ask);
            

            bool final = false;
            do
            {
                Console.Clear();
                
                Console.WriteLine("Выберите действие: ");
                Console.WriteLine("1 - Вывод всех элементов");
                Console.WriteLine("2 - Добавление элемента");
                Console.WriteLine("3 - Поиск узла с заданным значением");
                Console.WriteLine("4 - Вставка элемента после максимального");
                Console.WriteLine("5 - Удаление");
                
                Console.WriteLine("0 - Выход.");
                
                int.TryParse(Console.ReadLine(), out var switcher);
                
                switch (switcher)
                {
                    case 1: //Вывод всех элементов
                        Console.Clear();
                        if (mainLinkedList.Count == 0) Console.WriteLine("Лист пуст");
                        else Console.WriteLine($"В листе {mainLinkedList.Count} элементов(а)");
                        foreach (var element in mainLinkedList)
                        {
                            Console.Write(element);
                            Console.Write(" ");
                        }
                        int.TryParse(Console.ReadLine(), out var waiter); //до тех пор пока пользователь не нажмёт enter окно не закроется
                        break;
                    case 2: //Добавление элемента
                        Console.Clear();
                        Adding(mainLinkedList);
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 3: //Поиск узла с заданным значением
                        Console.Clear();
                        if (mainLinkedList.Count > 0)
                        {
                            Console.Write("Введите искомый элемент: ");
                            int.TryParse(Console.ReadLine(), out var searchedNumber);
                            if (mainLinkedList.Find(searchedNumber) == null) Console.WriteLine("Такого числа в списке нет.");
                            else Console.WriteLine("Такое число присутствует в списке.");
                        }
                        else Console.WriteLine("Список пуст");
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 4: //Вставка элемента после максимального
                        Console.Clear();
                        MaximumSearch(mainLinkedList);
                        int.TryParse(Console.ReadLine(), out waiter);
                        break;
                    case 5: //Удаление
                        Console.Clear();
                        Deleting(mainLinkedList);
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