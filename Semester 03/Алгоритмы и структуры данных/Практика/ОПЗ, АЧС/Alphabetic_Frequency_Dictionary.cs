// Алфавитно-частотный словарь
// Туровец В.Ю.
// 27 дек 2022


//using System.Diagnostics;
//using System.Text;

namespace Alphabetic_Frequency_Dictionary
{ 
    class Program
    {
        private static string Cleaner(string text)//Очистка текста от знаков препинания, двойных пробелов, переходов на другую строку, цифр и приведение к нижнему регистру
        {
            foreach (var c in "1234567890—-‑=,./?;:{}[]!@#$%^*()_+№`~«»…\r\n")
                text = text.Replace(c, '&'); //обозначение всех знаков припинания, цифр, символом и переходов на другую строку на &
            text= text.Replace("&", "");    //замена всех условных обозначений на пустое место
            text= text.Replace("  ", " ");  //замена двойных пробелов на одинарные
            
            return text.ToLower(); //приведение всего текста к нижнему регистру
        }
        private static Stack<string> Converter(string readText) //Конвертация текста в стек по-словно
        {
            Stack<string> convertedText = new Stack<string>(); //Создание стека со всеми словами текста
            string[] redundantArray = readText.Split(' '); //Создание массива и запись в него всех слов считаного текста

            foreach (var _ in redundantArray) if (_.Length != 1) convertedText.Push(_); //Запись слов больше 1 символа в длину в стек 

            //foreach (var _ in convertedText) Console.WriteLine(_); //Вывод всех слов стека
            
            return convertedText;
            
        }
        private static List<KeyValuePair<string, int>> Sorter(Stack<string> text, int how = 1) //Слово записывается в словарь с количеством повторений
        {
            
            
            Dictionary<string, int> vacabulary = new Dictionary<string, int>();

            foreach (string word in text)
            {
                if (vacabulary.ContainsKey(word)) vacabulary[word]++;
                else vacabulary.Add(word, 1);
            }

            var sortedDict = new List<KeyValuePair<string, int>>();
            
            switch (how)
            {
                case 1:
                    sortedDict = vacabulary.OrderBy(d => d.Value).ToList();
                    break;
                case 2:
                     sortedDict = vacabulary.OrderBy(obj => obj.Key).ToList();
                    break;
            }

            return sortedDict;
        }
        
         static void Main()
        {
            string readText = File.ReadAllText(" ");

            readText = Cleaner(readText); //Очистка текста от знаков препинания, двойных пробелов, переходов на другую строку, цифр и приведение к нижнему регистру

            Stack<string> convertedText = Converter(readText); //Конвертация текста в стек по-словно
            
            Console.WriteLine($"Количество слов: {convertedText.Count}");

            
            Console.WriteLine("1 - по частоте упоминаний");
            Console.WriteLine("2 - по алфавиту");
            int.TryParse(Console.ReadLine(), out var ask);
            List<KeyValuePair<string, int>> sortedDict = Sorter(convertedText, ask);

            
            foreach (var pair in sortedDict)
            {
                Console.Write($"{pair.Key}:{pair.Value}; ");
            }
            
            int.TryParse(Console.ReadLine(), out var waiter);
        }
    }
}
