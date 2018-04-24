﻿using CCTUnifor.Logger;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace MyCompiler.Tokenization.Aspects
{
    [PSerializable]
    public class LogAnalyserAspectAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            Logger.IsToPrintInConsole = true;
            Logger.PrintHeader("Analyse the input:");
            base.OnEntry(args);
        }
        
    }
}