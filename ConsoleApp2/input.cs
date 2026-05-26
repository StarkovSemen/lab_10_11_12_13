using System;
using System.Collections.Generic;
using System.IO;

namespace Компилятор
{
    struct TextPosition
    {
        public uint lineNumber;
        public byte charNumber;

        public TextPosition(uint ln = 0, byte c = 0)
        {
            lineNumber = ln;
            charNumber = c;
        }
    }

    struct Err
    {
        public TextPosition errorPosition;
        public byte errorCode;

        public Err(TextPosition errorPosition, byte errorCode)
        {
            this.errorPosition = errorPosition;
            this.errorCode = errorCode;
        }
    }

    class InputOutput
    {
        const byte ERRMAX = 9;
        public static char Ch { get; set; }
        public static TextPosition positionNow = new TextPosition();
        static string line;
        static byte lastInLine = 0;
        public static List<Err> err;
        static StreamReader File { get; set; }
        static uint errCount = 0;
        public static bool EndOfFile { get; private set; }

        public static void Initialize(string filePath)
        {
            File = new StreamReader(filePath);
            EndOfFile = false;
            positionNow = new TextPosition(1, 0);
            errCount = 0;
            ReadNextLine();
        }

        static public void NextCh()
        {
            if (EndOfFile) return;

            if (positionNow.charNumber == lastInLine)
            {
                Console.WriteLine($"{positionNow.lineNumber,4}  {line}");
                if (err.Count > 0)
                    ListErrors();
                ReadNextLine();
                positionNow.lineNumber = positionNow.lineNumber + 1;
                positionNow.charNumber = 0;
                if (EndOfFile) return;
            }
            else ++positionNow.charNumber;
            Ch = line[positionNow.charNumber];
        }

        private static void ReadNextLine()
        {
            if (!File.EndOfStream)
            {
                line = File.ReadLine();
                lastInLine = (byte)(line.Length - 1);
                err = new List<Err>();
            }
            else
            {
                EndOfFile = true;
                Console.WriteLine($"\nКомпиляция завершена: ошибок — {errCount}!");
                File.Close();
            }
        }

        static void ListErrors()
        {
            int pos = 6 - $"{positionNow.lineNumber} ".Length;
            string s;
            foreach (Err item in err)
            {
                ++errCount;
                s = "**";
                if (errCount < 10) s += "0";
                s += $"{errCount}**";
                while (s.Length - 1 < pos + item.errorPosition.charNumber) s += " ";
                s += $"^ ошибка код {item.errorCode}";
                Console.WriteLine(s);

                string desc = GetErrorDescription(item.errorCode);
                if (!string.IsNullOrEmpty(desc))
                    Console.WriteLine(new string(' ', pos + item.errorPosition.charNumber + 1) + desc);
            }
        }

        static string GetErrorDescription(byte code)
        {
            switch (code)
            {
                case 100: return "использование имени не соответствует описанию";
                case 147: return "тип метки не совпадает с типом выбирающего выражения";
                case 203: return "целая константа превышает допустимый диапазон";
                default: return null;
            }
        }

        static public void Error(byte errorCode, TextPosition position)
        {
            if (err.Count <= ERRMAX)
                err.Add(new Err(position, errorCode));
        }
    }
}