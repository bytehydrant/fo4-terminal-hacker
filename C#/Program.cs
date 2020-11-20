using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FalloutHack
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            do
            {
                List<Word> possibles = Collect();

                do
                {
                    possibles.Sort((left, right) => {
                        if (right.Distinction == left.Distinction) return Math.Sign(left.Deviation - right.Deviation);
                        else return right.Distinction - left.Distinction;
                    });
                    Report(possibles);
                    Word best = possibles.First();
                    Console.WriteLine($"Best of {possibles.Count}: {best.Name}");

                    int? likenesses = null;
                    while (true)
                    {
                        Console.Write("Likenesses: ");

                        string input = Console.ReadLine();
                        if (string.IsNullOrEmpty(input))
                        {
                            break;
                        }

                        try
                        {
                            likenesses = int.Parse(input);
                            break;
                        }
                        catch { }
                    }

                    if (!likenesses.HasValue || best.Name.Length == likenesses)
                    {
                        break;
                    }

                    var matches = best.Comparisons
                        .Where(comp => comp.Likenesses == likenesses)
                        .Select(comp => comp.Name);
                    possibles = possibles
                        .Where(w => matches.Contains(w.Name))
                        .Select(w => w.Prune(matches))
                        .ToList();
                } while (true);
            } while (true);

            static void Report(List<Word> possibles)
            {
                foreach (Word word in possibles)
                {
                    Console.WriteLine($"{word.Name} : {word.Distinction}|{Math.Round(word.Deviation, 2):0.00} : {String.Join("|", word.Comparisons.Select(c => c.Likenesses))}");
                }
            }
        }

        private static List<Word> Collect()
        {
            List<Word> possibles = new List<Word>();
            string line;
            do
            {
                Console.Write(">> ");
                line = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                if (possibles.Count > 0)
                {
                    if (possibles[0].Name.Length < line.Length)
                    {
                        Console.WriteLine("Too Long!");
                        continue;
                    }
                    else if (possibles[0].Name.Length > line.Length)
                    {
                        Console.WriteLine("Too Short!");
                        continue;
                    }
                }

                possibles.Add(new Word() { Name = line });

            } while (!String.IsNullOrEmpty(line));

            foreach(Word left in possibles)
            {
                left.BuildComparisons(possibles);
            }

            return possibles;
        }

        class Word
        {
            public string Name { get; set; }

            public List<Comp> Comparisons { get; set;}

            public int Distinction => Comparisons.Select(c => c.Likenesses).Distinct().Count();

            public double Deviation
            {
                get
                {
                    var counts = Comparisons.GroupBy(c => c.Likenesses).Select(g => g.Count());
                    var avg = counts.Average();
                    return Math.Sqrt(counts.Sum(v => Math.Pow(v - avg, 2)) / (counts.Count()-1));
                }
            }

            public void BuildComparisons(List<Word> words)
            {
                Comparisons = new List<Comp>();
                foreach(Word toComp in words)
                {
                    Comparisons.Add(new Comp(toComp.Name, Compare(toComp)));
                }
            }

            private int Compare(Word toComp)
            {
                int like = 0;
                for(int i = 0; i < Name.Length; i++)
                {
                    like += Name[i] == toComp.Name[i] ? 1 : 0;
                }
                return like;
            }

            public Word Prune(IEnumerable<string> names)
            {
                Comparisons = Comparisons.Where(comp => names.Contains(comp.Name)).ToList();
                return this;
            }
        }

        class Comp
        {
            public Comp(string name, int likenesses)
            {
                Name = name;
                Likenesses = likenesses;
            }

            public string Name { get; set; }

            public int Likenesses { get; set; }
        }
    }
}
