using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoolEval
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("==================================================");
            Console.WriteLine(" BOOLE-ALGEBRA ES IGAZSAGTABLA ELEMZO SEGEDESZKOZ ");
            Console.WriteLine("==================================================\n");

            Console.WriteLine("Tamogatott operatorok: ! (NOT), & (AND), | (OR), ^ (XOR), ( )");
            Console.WriteLine("Pelda: (A & B) | !C   vagy   A ^ B & C\n");

            while (true)
            {
                Console.Write("Adj meg egy logikai kifejezest (vagy 'exit' a kilepeshez): ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input) || input.Trim().ToLower() == "exit")
                {
                    break;
                }

                try
                {
                    EvaluateAndPrint(input.Trim());
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Hiba az elemzes soran: " + ex.Message);
                    Console.ResetColor();
                }

                Console.WriteLine();
                Console.WriteLine(new string('-', 50));
                Console.WriteLine();
            }
        }

        static void EvaluateAndPrint(string expr)
        {
            var variables = expr.Where(char.IsLetter)
                                .Select(char.ToUpper)
                                .Distinct()
                                .OrderBy(c => c)
                                .ToList();

            if (variables.Count == 0)
            {
                throw new ArgumentException("A kifejezes nem tartalmaz valtozot!");
            }

            if (variables.Count > 6)
            {
                throw new ArgumentException("Az atlathatosag erdekeben legfeljebb 6 valtozo tamogatott.");
            }

            var rpnTokens = ConvertToRpn(expr);

            Console.WriteLine();
            Console.WriteLine("Felismerve: " + variables.Count + " db valtozo (" + string.Join(", ", variables) + ")");
            Console.WriteLine();
            Console.WriteLine("--- IGAZSAGTABLA ---");

            string header = "| " + string.Join(" | ", variables) + " | Kimenet | Minterm |";
            string separator = new string('-', header.Length);

            Console.WriteLine(separator);
            Console.WriteLine(header);
            Console.WriteLine(separator);

            int totalRows = 1 << variables.Count;
            List<int> minterms = new List<int>();
            List<int> maxterms = new List<int>();

            for (int i = 0; i < totalRows; i++)
            {
                var env = new Dictionary<char, bool>();
                for (int v = 0; v < variables.Count; v++)
                {
                    int bit = (i >> (variables.Count - 1 - v)) & 1;
                    env[variables[v]] = bit == 1;
                }

                bool result = EvaluateRpn(rpnTokens, env);
                if (result)
                {
                    minterms.Add(i);
                }
                else
                {
                    maxterms.Add(i);
                }

                string rowValues = string.Join(" | ", variables.Select(v => env[v] ? "1" : "0"));
                string outStr = result ? "1" : "0";
                Console.WriteLine("| " + rowValues + " |    " + outStr + "    |   m" + i.ToString().PadRight(4) + " |");
            }

            Console.WriteLine(separator);
            Console.WriteLine();
            Console.WriteLine("--- ALGEBRAI ERTEKELES ---");
            Console.WriteLine("Minterm indexek:   Sigma m(" + string.Join(", ", minterms) + ")");
            Console.WriteLine("Maxterm indexek:   Pi M(" + string.Join(", ", maxterms) + ")");

            if (minterms.Count == 0)
            {
                Console.WriteLine("Azonosan hamis kifejezes (kontradikcio): Y = 0");
            }
            else if (minterms.Count == totalRows)
            {
                Console.WriteLine("Azonosan igaz kifejezes (tautologia): Y = 1");
            }
            else
            {
                var sopTerms = minterms.Select(m =>
                {
                    var literals = new List<string>();
                    for (int v = 0; v < variables.Count; v++)
                    {
                        int bit = (m >> (variables.Count - 1 - v)) & 1;
                        literals.Add(bit == 1 ? variables[v].ToString() : "!" + variables[v]);
                    }
                    return "(" + string.Join(" & ", literals) + ")";
                });

                Console.WriteLine();
                Console.WriteLine("Diszjunktiv normalforma (SOP):");
                Console.WriteLine("Y = " + string.Join(" | ", sopTerms));
            }
        }

        static List<string> ConvertToRpn(string expr)
        {
            var output = new List<string>();
            var stack = new Stack<char>();

            int i = 0;
            while (i < expr.Length)
            {
                char c = expr[i];

                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                if (char.IsLetter(c))
                {
                    output.Add(char.ToUpper(c).ToString());
                }
                else if (c == '!' || c == '~')
                {
                    stack.Push('!');
                }
                else if (c == '&' || c == '|' || c == '^')
                {
                    while (stack.Count > 0 && stack.Peek() != '(' && GetPrecedence(stack.Peek()) >= GetPrecedence(c))
                    {
                        output.Add(stack.Pop().ToString());
                    }
                    stack.Push(c);
                }
                else if (c == '(')
                {
                    stack.Push(c);
                }
                else if (c == ')')
                {
                    while (stack.Count > 0 && stack.Peek() != '(')
                    {
                        output.Add(stack.Pop().ToString());
                    }
                    if (stack.Count == 0)
                    {
                        throw new ArgumentException("Nem egyezo zarojelek!");
                    }
                    stack.Pop();
                }
                else
                {
                    throw new ArgumentException("Ismeretlen karakter: '" + c + "'");
                }
                i++;
            }

            while (stack.Count > 0)
            {
                char op = stack.Pop();
                if (op == '(' || op == ')')
                {
                    throw new ArgumentException("Nem egyezo zarojelek!");
                }
                output.Add(op.ToString());
            }

            return output;
        }

        static int GetPrecedence(char op)
        {
            switch (op)
            {
                case '!': return 4;
                case '&': return 3;
                case '^': return 2;
                case '|': return 1;
                default: return 0;
            }
        }

        static bool EvaluateRpn(List<string> rpn, Dictionary<char, bool> env)
        {
            var stack = new Stack<bool>();

            foreach (var token in rpn)
            {
                if (token.Length == 1 && char.IsLetter(token[0]))
                {
                    stack.Push(env[token[0]]);
                }
                else if (token == "!")
                {
                    if (stack.Count < 1)
                    {
                        throw new InvalidOperationException("Hibas kifejezes felepites.");
                    }
                    stack.Push(!stack.Pop());
                }
                else if (token == "&" || token == "|" || token == "^")
                {
                    if (stack.Count < 2)
                    {
                        throw new InvalidOperationException("Hibas kifejezes felepites.");
                    }
                    bool right = stack.Pop();
                    bool left = stack.Pop();

                    bool res = false;
                    if (token == "&") res = left && right;
                    else if (token == "|") res = left || right;
                    else if (token == "^") res = left ^ right;

                    stack.Push(res);
                }
            }

            if (stack.Count != 1)
            {
                throw new InvalidOperationException("Kiertekelesi hiba.");
            }
            return stack.Pop();
        }
    }
}