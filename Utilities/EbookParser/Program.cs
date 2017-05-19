using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows.Forms;

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
                var xml = XDocument.Load(file);
                var df = xml.Root.Name.Namespace;
                
                var pElemenets = xml.Root.Descendants(df + "p");
                foreach(var i in pElemenets)
                {
                    allText.Add(CleanToText(i));
                }
            }

        }

        static string CleanToText(XElement element)
        {
            foreach(var i in element.Elements())
            {
                i.ReplaceWith(CleanToText(i));
            }
            return element.Value;
        }
    }
}
