﻿using System;
using MyCompiler.Grammar.Extensions;
using MyCompiler.Grammar.Tokens;

namespace MyCompiler.Grammar
{
    public abstract class Token
    {
        public string Value { get; private set; }

        protected Token(string value)
        {
            Value = value;
        }

        public bool IsEmpty()
        {
            return Value == "ε";
        }

        public override string ToString() => $"{Value}";
        public override bool Equals(object obj) => ((Token)obj).Value == Value;

        protected bool Equals(Token other)
        {
            return string.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        public static bool operator ==(Token token1, Token token2) => Equals(token1, token2);
        public static bool operator !=(Token token1, Token token2) => !Equals(token1, token2);

        public TerminalToken ToTerminalToken() => new TerminalToken(Value);
        public NonTerminalToken ToNonTerminalToken() => new NonTerminalToken(Value);

        public bool IsTerminal() => this is TerminalToken;
        public bool IsIdentifier() => Value.IsLetter();
        public bool IsNumber() => Value.IsNumber();
    }
}