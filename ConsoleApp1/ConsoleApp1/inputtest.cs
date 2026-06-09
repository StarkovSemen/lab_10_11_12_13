using System;
using System.IO;

namespace Компилятор
{
    static class InputOutputTests
    {
        private static string CreateTestFile(string fileName, string content)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
            File.WriteAllText(filePath, content);
            return filePath;
        }

        private static void PrintNumberedLines(string content)
        {
            string[] lines = content.Split('\n');
            for (int i = 0; i < lines.Length; i++)
                Console.WriteLine((i + 1).ToString().PadLeft(4) + " " + lines[i]);
        }

        private static void RunLexer(LexicalAnalyzer lexer)
        {
            byte sym;
            do
            {
                sym = lexer.NextSym();
            } while (sym != 0);
        }

        private static void TestLexerWithErrors()
        {
            string content =
                "program test;\n" +
                "var x : integer;\n" +
                "begin\n" +
                "  x := 9999;\n" +
                "  x := 228 ;\n" +
                "end.";

            string filePath = CreateTestFile("test_lexer_errors.pas", content);
            InputOutput.OpenFile(filePath);

            LexicalAnalyzer lexer = new LexicalAnalyzer();
            RunLexer(lexer);
            lexer.PrintOutputCodesByLine();
            InputOutput.CloseFile();
        }

        public static void RunAllTests()
        {
            TestLexerWithErrors();
        }
    }
}
