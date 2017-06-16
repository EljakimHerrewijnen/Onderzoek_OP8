using DeepLearning;
using MathSyntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    class Program
    {
        public class characterspace
        {
            public List<ArgumentValue> letters = new List<ArgumentValue>();

            public characterspace()
            {
                for (int i = 97; i < 123; i++)
                {
                    letters.Add(new ArgumentValue(""));
                }
            }

            public void SetChar(char letter)
            {
                int value = (int)letter;
                for (int i = 0; i < letters.Count; i++)
                {
                    letters[i].Value = 0;
                }
                if ((int)letter < 97 || (int)letter > 122)
                {
                    return;
                }
                letters[(int)letter - 97].Value = 1;
            }
        }

        public class Word
        {
            List<characterspace> word = new List<characterspace>();
            public Word(int lenght)
            {
                for (int i = 0; i < lenght; i++)
                {
                    word.Add(new characterspace());
                }
            }
            public List<ArgumentValue> get()
            {
                List<ArgumentValue> toreturn = new List<ArgumentValue>();
                foreach(var i in word)
                {
                    toreturn.AddRange(i.letters);
                }
                return toreturn;
            }
            public void set(string wordtoset)
            {
                for (int i = 0; i < wordtoset.Length; i++)
                {
                    word[i].SetChar(wordtoset[i]);
                }
            }
        }
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            List<string> NederlandseWoorden = new List<string>();
            List<string> EngelseWoorden = new List<string>();
            List<string> ItaliaanseWoorden = new List<string>();

            StreamReader file = new StreamReader("Data/WoordenNL.csv");
            string line = file.ReadLine();
            while (line != null)
            {
                NederlandseWoorden.Add(line.ToLower());
                line = file.ReadLine();
            }
            file.Close();

            file = new StreamReader("Data/WoordenIT.csv");
            line = file.ReadLine();
            while (line != null)
            {
                ItaliaanseWoorden.Add(line.ToLower());
                line = file.ReadLine();
            }
            file.Close();

            file = new StreamReader("Data/WoordenEN.csv");
            line = file.ReadLine();
            while (line != null)
            {
                EngelseWoorden.Add(line.ToLower());
                line = file.ReadLine();
            }
            file.Close();

            Word word = new Word(10);
            OutputData Nederlands = new OutputData();
            OutputData Italiaans = new OutputData();
            OutputData Engels = new OutputData();

            //NieuwNetwerk(word.get(), new List<OutputData> { Nederlands, Italiaans, Engels });
            LearnNetwerk(word, Nederlands, Engels, Italiaans, NederlandseWoorden, EngelseWoorden, ItaliaanseWoorden);
            //TestNetwerk(word, Nederlands, Engels, Italiaans, NederlandseWoorden, EngelseWoorden, ItaliaanseWoorden);
        }

        static void TestNetwerk(Word word, OutputData Nederlands, OutputData Engels, OutputData Italiaans,
            List<string> NederlandseWoorden,
        List<string> EngelseWoorden,
        List<string> ItaliaanseWoorden)
        {
            var neuralnet = NeuralNetwork.Load(word.get(), new List<OutputData> { Nederlands, Italiaans, Engels });
            int wrong = 0;
            int right = 0;

            foreach (var _word in NederlandseWoorden)
            {
                word.set(_word);
                neuralnet.CalculateResults();
                if (Nederlands.Value > Engels.Value && Nederlands.Value > Italiaans.Value)
                {
                    right++;
                }
                else
                {
                    wrong++;
                }
            }
            foreach (var _word in EngelseWoorden)
            {
                word.set(_word);
                neuralnet.CalculateResults();
                if (Engels.Value > Nederlands.Value && Engels.Value > Italiaans.Value)
                {
                    right++;
                }
                else
                {
                    wrong++;
                }
            }
            foreach (var _word in ItaliaanseWoorden)
            {
                word.set(_word);
                neuralnet.CalculateResults();
                if (Italiaans.Value > Engels.Value && Italiaans.Value > Nederlands.Value)
                {
                    right++;
                }
                else
                {
                    wrong++;
                }
            }
            Console.WriteLine((double)right / (double)(right + wrong) * 100.0);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void LearnNetwerk(Word word, OutputData Nederlands, OutputData Engels, OutputData Italiaans,
            List<string> NederlandseWoorden,
        List<string> EngelseWoorden,
        List<string> ItaliaanseWoorden)
        {
            var neuralnet = NeuralNetwork.Load(word.get(), new List<OutputData> { Nederlands, Italiaans, Engels });
            StreamWriter performance = new StreamWriter("Accuracy");
            Random RNG = new Random();

            for (int times = 0; times < 1; times++)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    switch (RNG.Next(0, 3))
                    {
                        case 0:
                            word.set(NederlandseWoorden[RNG.Next(0, NederlandseWoorden.Count)]);
                            Nederlands.MustBeHigh = true;
                            Italiaans.MustBeHigh = false;
                            Engels.MustBeHigh = false;
                            break;
                        case 1:
                            word.set(EngelseWoorden[RNG.Next(0, EngelseWoorden.Count)]);
                            Nederlands.MustBeHigh = false;
                            Italiaans.MustBeHigh = false;
                            Engels.MustBeHigh = true;
                            break;
                        case 2:
                            word.set(ItaliaanseWoorden[RNG.Next(0, ItaliaanseWoorden.Count)]);
                            Nederlands.MustBeHigh = false;
                            Italiaans.MustBeHigh = true;
                            Engels.MustBeHigh = false;
                            break;
                        default:
                            break;
                    }
                    neuralnet.Learn();
                    if (i % 1000 == 0)
                    {
                        Console.WriteLine(i);
                        performance.WriteLine(i.ToString() + ";" + TestExistingNetwork(neuralnet, word, Nederlands, Engels, Italiaans, NederlandseWoorden, EngelseWoorden, ItaliaanseWoorden).ToString());
                    }

                }
            }
            performance.Close();
            neuralnet.Save();
        }

        static double TestExistingNetwork(NeuralNetwork neuralnet, Word word, OutputData Nederlands, OutputData Engels, OutputData Italiaans,
            List<string> NederlandseWoorden,
        List<string> EngelseWoorden,
        List<string> ItaliaanseWoorden)
        {
            int wrong = 0;
            int right = 0;

            for (int i = 0; i < NederlandseWoorden.Count/100; i++)
            {
                var _word = NederlandseWoorden[i];
                word.set(_word);
                neuralnet.CalculateResults();
                if (Nederlands.Value > Engels.Value && Nederlands.Value > Italiaans.Value)
                {
                    right++;
                }
                else
                {
                    wrong++;
                }
            }
            for (int i = 0; i < EngelseWoorden.Count / 100; i++)
            {
                var _word = EngelseWoorden[i];
                word.set(_word);
                neuralnet.CalculateResults();
                if (Engels.Value > Nederlands.Value && Engels.Value > Italiaans.Value)
                {
                    right++;
                }
                else
                {
                    wrong++;
                }
            }
            for (int i = 0; i < ItaliaanseWoorden.Count / 100; i++)
            {
                var _word = ItaliaanseWoorden[i];
                word.set(_word);
                neuralnet.CalculateResults();
                if (Italiaans.Value > Engels.Value && Italiaans.Value > Nederlands.Value)
                {
                    right++;
                }
                else
                {
                    wrong++;
                }
            }
            return (double)right / (double)(right + wrong) * 100.0;
        }

        static void NieuwNetwerk(List<ArgumentValue> inputs, List<OutputData> outputs)
        {
            var network = new NeuralNetwork(inputs, outputs, new int[] { 30 });
            network.Save();
        }
    }
}
