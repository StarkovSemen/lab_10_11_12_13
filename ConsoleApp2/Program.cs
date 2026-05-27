using System;

namespace Компилятор
{
    class Program
    {
        static void Main()
        {
            string filePath = @"example.pas";
            CreateTestFile(filePath);

            (uint line, byte col, byte code)[] errorSpots = new (uint, byte, byte)[]
            {
                (10, 4, 100),
                (12, 4, 100),
                (13, 4, 147),
                (14, 4, 147)
            };

            InputOutput.Initialize(filePath);

            while (!InputOutput.EndOfFile)
            {
                foreach (var spot in errorSpots)
                {
                    if (InputOutput.LineNumber == spot.line &&
                        InputOutput.CharNumber == spot.col)
                    {
                        InputOutput.Error(spot.code);
                        break;
                    }
                }
                InputOutput.NextCh();
            }
        }

        private static void CreateTestFile(string path)
        {
            string[] lines = {
                "program example ( input, output );",
                "const c = 3;",
                "b = 56;",
                "var a : 'a' ... 'c';",
                "k, i : integer;",
                "begin",
                "read( k, i );",
                "for a := 'a' to 'c' do",
                "case a of",
                "    k: i := i * k;",
                "    'b': i := i + 1;",
                "    i : k := k + 2;",
                "    b: i := i - k;",
                "    c: i := ( i + k ) * 2",
                "end;",
                "writeln( i, k )",
                "end."
            };
            System.IO.File.WriteAllLines(path, lines);
        }
    }
}
