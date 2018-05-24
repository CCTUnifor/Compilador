﻿using System;
using CCTUnifor.ConsoleTable;
using CCTUnifor.Logger;
using MyCompiler.CodeGenerator;
using MyCompiler.ConsoleApp.CodeGeneratorCMS.Mocks;
using MyCompiler.Parser;

namespace MyCompiler.ConsoleApp.CodeGeneratorTM
{
    class Program
    {

        public static void ConfigConsole()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ConsoleTableOptions.Pad = 60;
            ConsoleTableOptions.DefaultIfNull = "Error";
            Logger.PathToSave = $"Logs/log{CmsCodeGenerator.Milliseconds}.txt";

            Logger.PrintHeader("# Code Generator - TM");
        }

        //[ConfigConsoleAspect]
        private static void Main(string[] args)
        {
            ConfigConsole();
            try
            {
                var grammar = Read("Grammar", GrammarMocks.Grammar1);
                var input = Read("Code", CodeMocks.Fatorial);

                var parser = new TopDownParser(grammar);
                parser.Parser(input);

                //var codeGenerator = new CmsCodeGenerator(parser, input);
                //codeGenerator.Generator();
                //codeGenerator.Export();
                //codeGenerator.ExecuteVM();

                var codeGenerator = new TmCodeGenerator(parser, input);
                codeGenerator.Generator();
                codeGenerator.Export();
                codeGenerator.ExecuteVM();
            }
            catch (Exception e)
            {
                Logger.PrintLn(e);
            }

            Console.ReadLine();
        }

        private static string Read(string type, string content)
        {
            var code = content;

            Logger.PrintHeader(type);
            Logger.PrintLn(code);

            return code;
        }
    }
}