﻿using MyCompiler.Core.Enums;
using MyCompiler.Core.Exceptions;
using MyCompiler.Grammar.Tokens;
using MyCompiler.Tokenization;

namespace MyCompiler.Parser
{
    public class TinyParser
    {
        public TinyTokenization LexicalAnalyze { get; set; }

        public TinyToken Peek => LexicalAnalyze.LastToken;
        private bool HasNext() => LexicalAnalyze.Any();

        private void Eat(string c)
        {
            if (Peek == null)
                throw new ExpectedException(c, "null", Peek.Line);
            if (c.ToLower() != Peek.Value.ToLower())
                throw new ExpectedException(c, Peek.Value, Peek.Line);

            //Console.WriteLine($"++++++ EAT - {c} ++++++");
            LexicalAnalyze.GetNextToken();
        }

        private void Eat(TinyGrammar c)
        {
            if (Peek == null)
                throw new ExpectedException("c", "null", Peek.Line);
            if (c != Peek.Grammar)
                throw new ExpectedException(c.ToString(), Peek.Grammar.ToString(), Peek.Line);

            //Console.WriteLine($"++++++ EAT - {c} ++++++");
            LexicalAnalyze.GetNextToken();
        }

        public void Parser(int countLine, string input)
        {
            LexicalAnalyze = new TinyTokenization(input);
            LexicalAnalyze.GetNextToken();
            DeclSequencia();
        }

        private void DeclSequencia()
        {
            Declaration();
            if (HasNext() && Peek.Grammar == TinyGrammar.SemiColon)
            {
                Eat(";");
                DeclSequencia();
            }
        }

        private void Declaration()
        {
            switch (Peek.Grammar)
            {
                case TinyGrammar.ReserveWord:
                    switch (Peek.Value)
                    {
                        case "read":
                            ReadDecl();
                            break;
                        case "write":
                            WriteDecl();
                            break;
                        case "if":
                            CondDecl();
                            break;
                        case "repeat":
                            RepetDecl();
                            break;
                        default:
                            throw new ExpectedException("ReserveWord", Peek.Value, Peek.Line);
                    }

                    break;
                case TinyGrammar.Identifier:
                    AtribDecl();
                    break;
                default:
                    throw new CompilationException($"in line :{Peek.Line} - <declaration>");
            }
        }

        private void ReadDecl()
        {
            Eat("read");
            Eat(TinyGrammar.Identifier);
        }

        private void WriteDecl()
        {
            Eat("write");
            Exp();
        }

        private void Exp()
        {
            ExpSimple();
            if (HasNext() && Peek.Grammar == TinyGrammar.Operator)
            {
                CompOp();
                ExpSimple();
            }
        }

        private void CompOp()
        {
            if (Peek.Value == "<")
                Eat("<");
            else
                Eat("=");
        }

        private void ExpSimple()
        {
            Term();
            if (HasNext() && Peek.Grammar == TinyGrammar.Sum)
                ExpSimpleLine();
        }

        private void ExpSimpleLine()
        {
            Sum();
            Term();
            if (Peek.Grammar == TinyGrammar.Sum)
                ExpSimpleLine();
        }

        private void Sum()
        {
            if (Peek.Value == "+")
                Eat("+");
            else
                Eat("-");
        }

        private void Term()
        {
            Factor();
            if (HasNext() && Peek.Grammar == TinyGrammar.Prod)
                TermLine();
        }

        private void Factor()
        {
            if (Peek.Grammar == TinyGrammar.Parentheses)
            {
                Eat("(");
                Exp();
                Eat(")");
            }
            else if (Peek.Grammar == TinyGrammar.Digit)
                Eat(TinyGrammar.Digit);
            else if (Peek.Grammar == TinyGrammar.Identifier)
                Eat(TinyGrammar.Identifier);
            else
                throw new CompilationException($"in line {Peek.Line} - <factor>");
        }

        private void TermLine()
        {
            Mult();
            Factor();
            if (Peek.Grammar == TinyGrammar.Prod)
                TermLine();
        }

        private void Mult()
        {
            if (Peek.Value == "*")
                Eat("*");
            else
                Eat("/");
        }

        private void CondDecl()
        {
            Eat("if");
            Exp();
            Eat("then");
            DeclSequencia();

            if (HasNext() && Peek.Value == "else")
            {
                Eat("else");
                DeclSequencia();
            }

            Eat("end");
        }

        private void RepetDecl()
        {
            Eat("repeat");
            DeclSequencia();
            Eat("until");
            Exp();
        }

        private void AtribDecl()
        {
            Eat(TinyGrammar.Identifier);
            Eat(TinyGrammar.Attribution);
            Exp();
        }
    }
}