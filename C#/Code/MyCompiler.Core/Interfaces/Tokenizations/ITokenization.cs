﻿using System.Collections.Generic;
using MyCompiler.Core.Interfaces.Graph;

namespace MyCompiler.Core.Interfaces.Tokenizations
{
    public interface ITokenization<T>
    {
        IEnumerable<IToken<T>> LoadTokens(string input);
        void PrintTokens(IEnumerable<IToken<T>> tokens);
    }
}