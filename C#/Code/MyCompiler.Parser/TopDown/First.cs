﻿using System.Collections.Generic;
using System.Linq;
using MyCompiler.Grammar.Tokens;

namespace MyCompiler.Parser.TopDown
{
    public class First : Base
    {

        public First(NonTerminalToken nonTerminal, ICollection<TerminalToken> terminals) : base(nonTerminal, terminals)
        {
        }
        public override string ToString() => $"first({NonTerminal}) => [{string.Join(", ", Terminals)}]";

        public bool AnyEmpty() => Terminals.Any(x => x.Value == "ε");
    }
}