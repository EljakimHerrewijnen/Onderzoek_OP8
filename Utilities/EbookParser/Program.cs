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


            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Comma Seperated Values|.csv";
            saveFile.ShowDialog();
            var writer = new StreamWriter(saveFile.FileName);
            foreach (var word in words.Keys)
                writer.WriteLine(word + ";" + words[word].ToString());
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
            text = text.Replace('.', ' ');
            text = text.Replace('!', ' ');
            text = text.Replace('?', ' ');
            text = text.Replace(',', ' ');
            text = text.Replace('"', ' ');
            text = text.Replace(':', ' ');
            text = text.Replace(';', ' ');
            text = text.Replace('\'', ' ');
            text = text.Replace('-', ' ');
            text = text.Replace('‘', ' ');
            text = text.Replace('’', ' ');
            text = text.Replace('“', ' ');
            text = text.Replace('”', ' ');

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
