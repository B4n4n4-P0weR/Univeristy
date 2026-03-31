// Обратная Польская запись
// Туровец В.Ю.
// 20 дек 2022

using System;
using System.Collections.Generic;

namespace Reverse_Polish_Notation
{
    class RPN
    {
        //Метод возвращает true, если проверяемый символ - разделитель ("пробел" или "равно")
        static private bool IsSeparator(char readChar)
        {
            if ((" =".IndexOf(readChar) != -1))
                return true;
            return false;
        }

        //Метод возвращает true, если проверяемый символ - оператор
        static private bool IsOperator(char readChar)
        {
            char[] operators = new char[] {'+', '-', '/', '*', '^', '(', ')'};
            
            foreach (var oper in operators)
            {
                if (oper == readChar)
                    return true;
            }
            return false;
        }

        //Метод возвращает true, если проверяемый символ - буква (аналог IsDigit)
        static private bool IsLetter(char readChar)
        {
            char[] engLetters = new char[] 
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            };
            
            foreach (var vowel in engLetters)
            {
                if (vowel == readChar)
                    return true;
            }
            return false;
        }

        //Метод возвращает приоритет проверяемого оператора, нужно для сортировки операторов
        static private int GetPriority(char readChar)
        {
            switch (readChar)
            {
                case '(': return 0;
                case ')': return 1;
                case '+': return 2;
                case '-': return 3;
                case '*': return 4;
                case '/': return 4;
                case '^': return 6;
                default: return 6;
            }
        }

 

        
        //Метод, преобразующий входную строку с выражением в постфиксную запись
        static public string GetExpression(string input)
        {
            string output = string.Empty; //Строка для хранения выходного выражения
            Stack<char> operatorsStack = new Stack<char>(); //Стек для хранения операторов

            for (int i = 0; i < input.Length; i++) //Для каждого символа в входной строке
            {
                //Если элемент является разделителем - переходим к следующему символу
                if (IsSeparator(input[i]))
                    continue; 

                //Если символ - число, то считываем и добавляем каждую цифру числа к нашей строке
                if (Char.IsDigit(input[i]) || IsLetter(input[i])) //Если цифра
                {
                    //Читаем до разделителя или оператора, что бы получить число
                    while (!IsSeparator(input[i]) && !IsOperator(input[i]))
                    {
                        output += input[i]; //Добавляем 
                        i++;

                        if (i == input.Length) break; //Если символ - последний, то выходим из цикла
                    }

                    output += " "; //Дописываем после числа пробел в строку с выражением
                    i--; //Возвращаемся на один символ назад, к символу перед разделителем
                }

                if (IsOperator(input[i])) //Если оператор
                {
                    if (input[i] == '(') //Если символ - открывающая скобка
                        operatorsStack.Push(input[i]); //Записываем её в стек для операторов
                    else if (input[i] == ')') //Если символ - закрывающая скобка
                    {
                        //Выписываем все операторы до открывающей скобки в строку
                        char s = operatorsStack.Pop();

                        while (s != '(')
                        {
                            output += s.ToString() + ' ';
                            s = operatorsStack.Pop();
                        }
                    }
                    else //Если любой другой оператор
                    {
                        if (operatorsStack.Count > 0) //Если в стеке есть элементы
                            if (GetPriority(input[i]) <=
                                GetPriority(operatorsStack
                                    .Peek())) //И если приоритет нашего оператора меньше или равен приоритету оператора на вершине стека
                                output += operatorsStack.Pop().ToString() +
                                          " "; //То добавляем последний оператор из стека в строку с выражением

                        operatorsStack.Push(
                            char.Parse(input[i].ToString())); 
                            //Если стек пуст, или же приоритет оператора выше - добавляем операторов на вершину стека

                    }
                }
            }

            //Когда прошли по всем символам, выкидываем из стека все оставшиеся там операторы в строку
            while (operatorsStack.Count > 0)
                output += operatorsStack.Pop() + " ";

            return output; //Возвращаем выражение в постфиксной записи
        }
    }
    
    
    
    
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Задание: a + ( b - c ) * d");
            Console.WriteLine($"Ответ:   {RPN.GetExpression("a+(b-c)*d")}");
            
            Console.WriteLine("Задание: 2 * ( 3 + 5 ) - ( 6 + 7 ) / ( 8 - 9 )");
            Console.WriteLine($"Ответ:   {RPN.GetExpression("2*(3+5)-(6+7)/(8-9)")}");
            
            Console.WriteLine("Задание: 123 * ( a + b ) * c");
            Console.WriteLine($"Ответ:   {RPN.GetExpression("123*(a+b)*c")}");
            
            Console.WriteLine("Задание: a + b * ( c - d ) / e + f");
            Console.WriteLine($"Ответ:   {RPN.GetExpression("a+b*(c-d)/e+f")}");
            
            Console.WriteLine("Задание: ( 8 + 2 * 5 ) / ( 1 + 3 * 2 - 4 )");
            Console.WriteLine($"Ответ:   {RPN.GetExpression("(8+2*5)/(1+3*2-4)")}");
                
            while (true) //Ручной ввод выражений
            {
                Console.Write("Введите выражение: ");
                Console.WriteLine(RPN.GetExpression(Console.ReadLine()));
            }
        }
    }
    
}