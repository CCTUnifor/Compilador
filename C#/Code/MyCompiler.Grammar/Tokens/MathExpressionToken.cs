﻿using MyCompiler.Core.Enums.MathExpression;
using MyCompiler.Core.Interfaces.Graph;

namespace MyCompiler.Grammar.Tokens
{
    public class MathExpressionToken : _IToken<MathExpressionGrammarClass>
    {
        public string Value { get; private set; }
        public MathExpressionGrammarClass GrammarClass { get; private set; }
        public int Line { get; private set; }
        public int? Collumn { get; private set; }

        public MathExpressionToken(string value, MathExpressionGrammarClass grammarClass, int line, int collumn = default)
        {
            Value = value;
            GrammarClass = grammarClass;
            Line = line;
            Collumn = collumn;
        }

        public bool SameGramarClassTo(_IToken<MathExpressionGrammarClass> currentToken)
            => GrammarClass == currentToken?.GrammarClass;

        void _IToken<MathExpressionGrammarClass>.ConcatValue(string value)
            => Value += value;

        public override bool Equals(object obj)
        {
            var x = (MathExpressionToken)obj;
            return Value == x.Value && GrammarClass == x.GrammarClass && Line == x.Line && Collumn == x.Collumn;
        }
    }
}