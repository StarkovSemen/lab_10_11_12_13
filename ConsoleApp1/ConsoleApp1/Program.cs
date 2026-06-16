using System;

namespace Компилятор
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "program.pas";
            System.IO.File.WriteAllText(filePath,
@"program test;
var
    x : integer;
begin
    x := 1000000000;
end.");

            InputOutput.OpenFile(filePath);

            LexicalAnalyzer lexer = new LexicalAnalyzer();

            byte sym;
            do
            {
                sym = lexer.NextSym();
            } while (sym != 0);

            lexer.PrintOutputCodesByLine();

            Console.WriteLine("\n");

            SyntaxAnalyzer parser = new SyntaxAnalyzer(lexer);
            parser.Analyze();

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
