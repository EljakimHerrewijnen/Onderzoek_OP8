using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace EbookParser
{
    class Program
    {
        const int Wordlenght = 10;
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            //selecting files
            OpenFileDialog fileselecter = new OpenFileDialog();
            fileselecter.Multiselect = true;
            fileselecter.ShowDialog();
            List<string> allText = new List<string>();

            foreach (var file in fileselecter.FileNames)
            {
                try
                {
                    var xml = XDocument.Load(file);
                    var df = xml.Root.Name.Namespace;

                    var pElemenets = xml.Root.Descendants(df + "p");
                    foreach (var i in pElemenets)
                    {
                        allText.Add(CleanToText(i));
                    }
                }
                catch
                {

                }
            }

            List<string> Allwords = new List<string>();
            foreach (var line in allText)
            {
                Allwords.AddRange(line.Split(null));
            }
            Allwords.RemoveAll(new Predicate<string>(x => x == ""));


            Dictionary<string, int> words = new Dictionary<string, int>();
            foreach(var word in Allwords)
            {
                if (words.ContainsKey(word))
                    words[word] = words[word] + 1;
                else
                    words.Add(word, 1);
            }

            foreach(var word in words.Keys.ToList())
            {
                bool isclean = true;
                foreach(var character in word)
                {
                    if(!(((int)character >= 65 && (int)character <= 90) || (int)character >= 97 && (int)character <= 122))
                    {
                        isclean = false;
                    }
                }
                if (word.Length > Wordlenght)
                    isclean = false;
                if (!isclean)
                    words.Remove(word);
            }

            //maakt dataset groter door spaties toe te voegen (ook beter voor leren)
            List<string> FinalList = new List<string>();
            foreach(var word in words.Keys)
            {
                for (int i = 0; i <= Wordlenght-word.Length; i++)
                {
                    var newword = "";
                    for (int j = 0; j < i; j++)
                    {
                        newword += " ";
                    }
                    newword += word;
                    while (newword.Length < Wordlenght)
                        newword += " ";
                    FinalList.Add(newword);
                }
            }

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Comma Seperated Values|.csv";
            saveFile.ShowDialog();
            var writer = new StreamWriter(saveFile.FileName);
            foreach (var word in FinalList)
            {
                writer.WriteLine(word);
            }
                
            writer.Close();
        }

        static string CleanToText(XElement element)
        {
            foreach(var i in element.Elements())
            {
                i.ReplaceWith(CleanToText(i));
            }
            return RemoveNumbers(RemovePunctuation(element.Value)).ToLower();
        }

        static string RemovePunctuation(string text)
        {
            const string punctuation = ".!?,\"':;\\/-‘’“”(){}[].@#$%&^*…";
            foreach (var i in punctuation)
            {
                text = text.Replace(i, ' ');
            }

            return text;
        }
        static string RemoveNumbers(string text)
        {
            const string numbers = "1234567890";
            foreach( var i in numbers)
            {
                text = text.Replace(i, ' ');
            }
            return text;
        }
    }
}
