using System;
using System.Collections.Generic;
using System.IO;

namespace Компилятор
{
    public class InputOutput
    {
        private struct TextPosition
        {
            public uint lineNumber;
            public byte charNumber;

            public TextPosition(uint ln = 0, byte c = 0)
            {
                lineNumber = ln;
                charNumber = c;
            }
        }

        private struct Err
        {
            public TextPosition errorPosition;
            public byte errorCode;

            public Err(TextPosition errorPosition, byte errorCode)
            {
                this.errorPosition = errorPosition;
                this.errorCode = errorCode;
            }
        }
        
        private const byte ERRMAX = 9;
        private static char ch;
        private static TextPosition positionNow = new TextPosition();
        private static string line;
        private static byte lastInLine = 0;
        private static List<Err> err;
        private static StreamReader file;
        private static uint errCount = 0;
        private static bool endOfFile;
        
        public static char Ch 
        { 
            get => ch; 
            private set => ch = value; 
        }

        public static uint LineNumber => positionNow.lineNumber;
        public static byte CharNumber => positionNow.charNumber;
        
        public static bool EndOfFile 
        { 
            get => endOfFile; 
            private set => endOfFile = value; 
        }

        public static void Initialize(string filePath)
        {
            file = new StreamReader(filePath);
            EndOfFile = false;
            positionNow = new TextPosition(1, 0);
            errCount = 0;
            ReadNextLine();
        }

        public static void NextCh()
        {
            if (EndOfFile) return;

            if (positionNow.charNumber == lastInLine)
            {
                Console.WriteLine($"{positionNow.lineNumber,4}  {line}");
                if (err != null && err.Count > 0)
                    ListErrors();
                ReadNextLine();
                positionNow.lineNumber = positionNow.lineNumber + 1;
                positionNow.charNumber = 0;
                if (EndOfFile) return;
            }
            else 
                ++positionNow.charNumber;
            
            ch = line[positionNow.charNumber];
        }

        private static void ReadNextLine()
        {
            if (!file.EndOfStream)
            {
                line = file.ReadLine();
                lastInLine = (byte)(line.Length - 1);
                err = new List<Err>();
            }
            else
            {
                EndOfFile = true;
                Console.WriteLine($"\nКомпиляция завершена: ошибок — {errCount}!");
                file.Close();
            }
        }

        private static void ListErrors()
        {
            int pos = 6 - $"{positionNow.lineNumber} ".Length;
            string s;
            foreach (Err item in err)
            {
                ++errCount;
                s = "**";
                if (errCount < 10) s += "0";
                s += $"{errCount}**";
                while (s.Length - 1 < pos + item.errorPosition.charNumber) 
                    s += " ";
                s += $"^ ошибка код {item.errorCode}";
                Console.WriteLine(s);

                string desc = GetErrorDescription(item.errorCode);
                if (!string.IsNullOrEmpty(desc))
                    Console.WriteLine(new string(' ', pos + item.errorPosition.charNumber + 1) + desc);
            }
        }

        private static string GetErrorDescription(byte code)
        {
            switch (code)
            {
                case 100: return "использование имени не соответствует описанию";
                case 147: return "тип метки не совпадает с типом выбирающего выражения";
                case 203: return "целая константа превышает допустимый диапазон";
                default: return null;
            }
        }

        public static void Error(byte errorCode)
        {
            if (err != null && err.Count <= ERRMAX)
                err.Add(new Err(positionNow, errorCode));
        }
    }
}
