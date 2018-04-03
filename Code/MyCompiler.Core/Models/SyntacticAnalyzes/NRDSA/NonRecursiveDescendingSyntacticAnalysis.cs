﻿using System;
using System.Collections.Generic;
using System.Linq;
using MyCompiler.Core.Exceptions;

namespace MyCompiler.Core.Models.SyntacticAnalyzes.NRDSA
{
    public class NonRecursiveDescendingSyntacticAnalysis
    {
        private string Grammar { get; }
        private ICollection<Term> Terms { get; set; }
        private ICollection<First> Firsts { get; set; }
        private ICollection<Follow> Follows { get; set; }

        private ICollection<NonTerminal> NonTerminals { get; set; }
        private List<Terminal> Terminals { get; set; }
        private Term[,] Table { get; set; }
        private bool IsTerminal(string production) => Terminals.Any(x => x.Value == production);

        public NonRecursiveDescendingSyntacticAnalysis(string grammar) => Grammar = grammar;
        private Term GetTermByElement(string firstElement) => Terms.SingleOrDefault(x => x.Caller.Value == firstElement);

        private static int stackCounterPad = 3;
        private static int stackPad = 200;
        private static int stackInputPad = 20;
        private static int stackCalledPad = 50;

        private static void PrintHeaderStack()
            => Printable.Printable.PrintLn($"{" #".PadRight(stackCounterPad + 2)} {" Stack".PadRight(stackPad + 2)} {" Input".PadRight(stackInputPad + 2)} {" Term".PadRight(stackCalledPad + 2)}");
        private static void PrintRowStack(IEnumerable<string> q, int count, IEnumerable<string> restOfTheInput, string termString)
            => Printable.Printable.PrintLn($"[{count.ToString().PadRight(stackCounterPad)}] [{string.Join(", ", q).PadRight(stackPad)}] [{string.Join(" ", restOfTheInput).PadRight(stackInputPad)}] [{termString.PadRight(stackCalledPad)}]");
        private ICollection<NonTerminal> GetNonTerminalsOrdered() => NonTerminals.OrderByDescending(x => x.Value.Length).ToList();
        private int GetIndexNonTerminal(string X) => NonTerminals.Select(x => x.Value).ToList().IndexOf(X);
        private bool IsNum(string s) => s.All(char.IsDigit);
        private bool IsLetter(string s) => s.All(char.IsLetter);

        public void Parser(string input)
        {
            GenerateTerms();
            GenerateFirst();
            GenerateFollows();
            GenerateTable();
            Analyse(input);
        }

        private void GenerateTerms()
        {
            var lines = Grammar.Split("\n").Select(x => x.Replace("\r", "")).ToArray();
            Terms = new List<Term>();
            Firsts = new List<First>();
            Follows = new List<Follow>();
            NonTerminals = new List<NonTerminal>();
            Terminals = new List<Terminal>();

            foreach (var line in lines.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                var caller = line.Split("->").FirstOrDefault()?.Trim().ToNonTerminal();
                var called = line.Split("->").LastOrDefault()?.Trim();

                var term = new Term(caller, called);
                Terms.Add(term);

                InitializeFirst(term);
                InitializeFollow(term);
                if (!NonTerminals.All(x => x.Value != caller.Value)) continue;

                NonTerminals.Add(caller);
            }

            foreach (var line in lines)
            {
                var called = line.Split("->").LastOrDefault()?.Trim();
                var splited = AllTerminals(called);
                var collection = splited.Select(x => x.ToTerminal()).Where(x => Terminals.All(y => y.Value != x.Value));

                Terminals.AddRange(collection);
            }

            Terminals.Add('$'.ToTerminal());
        }


        private IEnumerable<string> AllTerminals(string called)
        {
            var c = GetNonTerminalsOrdered().Select(x => x.Value).ToList();
            c.Add("ε");
            c.Add("|");
            c.Add(" ");

            return called.Split(c.ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Distinct().ToArray();
        }

        private void GenerateFirst()
        {
            foreach (var term in Terms)
                First(term);

            PrintInfo.PrintFirsts(Firsts);
        }

        private void GenerateFollows()
        {
            Follows.First().AddFinalSymble();

            foreach (var term in Terms)
                Follow(term);

            PrintInfo.PrintFollows(Follows);
        }

        private void GenerateTable()
        {
            Table = new Term[NonTerminals.Count, Terminals.Count];
            Printable.Printable.IsToPrintInConsole = false;

            foreach (var term in Terms)
            {
                var A = GetIndexNonTerminal(term.Caller.Value);

                foreach (var termProduction in term.Productions)
                {
                    var production = termProduction.Trim();
                    var first = production.Split(" ").First();
                    if (term.Caller.Value == "E'")
                        Console.WriteLine();

                    var f = IsTerminal(first) || first == "ε"
                        ? new First(term.Caller, new List<Terminal> { first.ToTerminal() })
                        : Firsts.Single(x => x.NonTerminal.Value == first);

                    foreach (var terminal in f.Terminals)
                    {
                        var a = GetIndexTerminal(terminal.Value);
                        var t = new Term(term.Caller, production);

                        if (A >= 0 && a >= 0)
                            PopulateTable(A, a, t);
                        else if (terminal.Value == "ε")
                        {
                            var follow = Follows.Single(x => x.NonTerminal == term.Caller);

                            foreach (var followTerminal in follow.Terminals)
                            {
                                var b = GetIndexTerminal(followTerminal.Value);
                                PopulateTable(A, b, t);
                            }
                        }
                    }
                }
            }

            PrintInfo.PrintTable(Terminals, NonTerminals, Table);
        }

        private void PopulateTable(int i, int j, Term t)
        {
            if (Table[i, j] == null)
                Table[i, j] = t;
            //else
            //    Table[i, j].AddProduction(t.Productions);
        }

        private void Analyse(string input)
        {
            Printable.Printable.IsToPrintInConsole = true;
            Printable.Printable.PrintLn($"++++++ Analyse the input: ++++++\n");

            PrintHeaderStack();

            var lines = input.Split("\n").Select(x => x.Replace("\t", "").Replace("\r", "")).ToArray();
            lines[lines.Length - 1] = lines[lines.Length - 1] + " $";

            var count = 0;
            var q = new Stack<string>();
            var X = NonTerminals.First().Value;

            q.Push("$");
            q.Push(X);

            for (var l = 0; l < lines.Length; l++)
            {
                var line = lines[l].Trim();

                var i = 0;
                var strings = line.Split(" ");

                while (X != "$" && i < strings.Length)
                {
                    var f = strings[i];
                    var M = GetIndexNonTerminal(X);
                    var a = GetIndexTerminal(f);

                    var restOfTheInput = strings.ToList();
                    restOfTheInput.RemoveRange(0, i);
                    var lineString = $"Line: {l + 1} | Collumn: {i + 1}\n\n";
                    if (count == 7)
                        Console.WriteLine();

                    if (X == f || (X == "ide" && IsLetter(f)) || (X == "num" && IsNum(f)))
                    {
                        if (q.Peek() != X && (q.Peek() == "ide" && !IsLetter(X)) && q.Peek() == "num" && !IsNum(X))
                            throw new CompilationException($"{lineString}Expeted: '{X}'; got '{q.Peek()}'");
                        PrintRowStack(q, count, restOfTheInput, "Next");

                        q.Pop();
                        i++;
                    }
                    else if (a >= 0 && Table[M, a] != null)
                    {
                        if (q.Peek() != X)
                            throw new CompilationException($"{lineString}Expeted: '{X}'; got '{q.Peek()}'");
                        var xc = Table[M, a];

                        PrintRowStack(q, count, restOfTheInput, xc.ToString());

                        q.Pop();

                        foreach (var production in xc.Productions)
                        {
                            var productionSplited = production.Split(" ");
                            for (var j = productionSplited.Length - 1; j >= 0; j--)
                            {
                                if (!productionSplited[j].IsEmpty())
                                    q.Push(productionSplited[j]);
                            }
                        }
                    }
                    else
                        throw new CompilationException($"{lineString}The {f} doesn't exists in this grammar!\nStack: [{string.Join(", ", q)}] \nX: '{X}' ; f: '{f}'; \nM: '{M}'; a: '{a}';");

                    X = q.Peek();
                    count++;
                }
            }
            PrintRowStack(q, count, new List<string> { "$" }, "Accepted");



            Printable.Printable.PrintLn("\nCompile success!!!!");
        }

        private int GetIndexTerminal(string f)
        {
            if (f == "ε")
                return -1;

            var i = Terminals.Select(x => x.Value).ToList().IndexOf(f);
            if (i >= 0)
                return i;

            if (IsLetter(f))
                return GetIndexTerminal("ide");
            if (IsNum(f))
                return GetIndexTerminal("num");

            return -1;
        }

        private First First(Term term)
        {
            var currentFirst = Firsts.Single(x => x.NonTerminal == term.Caller);

            foreach (var s in term.Productions)
            {
                var production = s.Trim();
                var elements = production.Split(" ");
                var firstElement = elements.FirstOrDefault();

                if (IsTerminal(firstElement) || firstElement.IsEmpty())
                    currentFirst.AddTerminal(firstElement.ToTerminal());
                else
                {
                    var termDerivated = GetTermByElement(firstElement);
                    currentFirst.AddTerminal(First(termDerivated).RemoveEmpty().Terminals);

                    var allTermsOfAllProductions = elements.Select(GetTermByElement).ToList();
                    for (var i = 1; i < elements.Length; i++)
                    {
                        var X1 = GetTermByElement(elements[i - 1]);
                        var X2 = GetTermByElement(elements[i]);

                        if (X1.AnyEmptyProduction())
                        {
                            if (X2 != null)
                                currentFirst.AddTerminal(First(X2).RemoveEmpty().Terminals);
                        }
                    }
                    if (allTermsOfAllProductions.All(x => x?.AnyEmptyProduction() ?? false))
                        currentFirst.AddTerminal("ε".ToTerminal());
                }
            }

            return currentFirst;
        }

        private void Follow(Term termChoosed)
        {
            var allTermsCurrent = (from term in Terms let y = term.Productions.SelectMany(x => x.Split(" ").ToArray()).Select(x => x.Replace(" ", "")) from termProduction in y where termProduction == termChoosed.Caller.Value select term).ToList();

            foreach (var currentTerm in allTermsCurrent)
            {
                var productionsChosed = currentTerm.Productions.Where(x => x.Contains(termChoosed.Caller.Value)).ToArray();
                foreach (var y in productionsChosed)
                {

                    var production = y.Trim();
                    var splited = production.Split(termChoosed.Caller.Value, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

                    var aBb = splited.Length > 1;
                    var aB = splited.Length > 0;

                    var followB = Follows.Single(x => x.NonTerminal.Value == termChoosed.Caller.Value);
                    var followA = Follows.Single(x => x.NonTerminal == currentTerm.Caller);
                    if (followB.NonTerminal.Value == "TERMO'")
                        Console.WriteLine();
                    if (aBb)
                    {
                        for (int j = 1; j < splited.Length; j++)
                        {
                            var firstb = Firsts.SingleOrDefault(x => x.NonTerminal.Value == splited.Last());
                            var c = splited[j];
                            var v = c.Split(" ").First();

                            if (IsTerminal(v))
                                followB.AddTerminal(v.ToTerminal());
                            else
                                followB.AddTerminal(firstb?.RemoveEmpty().Terminals);
                        }

                    }
                    else if (aB)
                        followB.AddTerminal(followA.Terminals);
                }
            }
        }

        private void InitializeFirst(Term term)
        {
            if (Firsts.All(x => x.NonTerminal != term.Caller))
                Firsts.Add(new First(term.Caller, new List<Terminal>()));
        }

        private void InitializeFollow(Term term)
        {
            if (Follows.All(x => x.NonTerminal != term.Caller))
                Follows.Add(new Follow(term.Caller, new List<Terminal>()));
        }
    }
}