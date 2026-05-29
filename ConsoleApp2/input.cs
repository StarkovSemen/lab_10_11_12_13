using System;
using System.Collections.Generic;
using System.IO;

namespace Компилятор
{
    public class InputOutput
    {
        private struct TextPosition
        {
            private uint _lineNumber;
            private byte _charNumber;

            public uint LineNumber 
            { 
                get => _lineNumber; 
                set => _lineNumber = value; 
            }
            
            public byte CharNumber 
            { 
                get => _charNumber; 
                set => _charNumber = value; 
            }

            public TextPosition(uint ln = 0, byte c = 0)
            {
                _lineNumber = ln;
                _charNumber = c;
            }
        }

        private struct Err
        {
            private TextPosition _errorPosition;
            private byte _errorCode;

            public TextPosition ErrorPosition 
            { 
                get => _errorPosition; 
                set => _errorPosition = value; 
            }
            
            public byte ErrorCode 
            { 
                get => _errorCode; 
                set => _errorCode = value; 
            }

            public Err(TextPosition errorPosition, byte errorCode)
            {
                _errorPosition = errorPosition;
                _errorCode = errorCode;
            }
        }
        
        private const byte ERRMAX = 9;
        private static char _ch;
        private static TextPosition _positionNow;
        private static string _line;
        private static byte _lastInLine;
        private static List<Err> _err;
        private static StreamReader _file;
        private static uint _errCount;
        private static bool _endOfFile;
        
        static InputOutput()
        {
            _positionNow = new TextPosition();
            _lastInLine = 0;
            _err = new List<Err>();
            _errCount = 0;
            _endOfFile = false;
        }
        
        public static char Ch 
        { 
            get => _ch; 
            private set => _ch = value; 
        }

        public static uint LineNumber => _positionNow.LineNumber;
        public static byte CharNumber => _positionNow.CharNumber;
        
        public static bool EndOfFile 
        { 
            get => _endOfFile; 
            private set => _endOfFile = value; 
        }

        public static void Initialize(string filePath)
        {
            _file = new StreamReader(filePath);
            EndOfFile = false;
            _positionNow = new TextPosition(1, 0);
            _errCount = 0;
            ReadNextLine();
        }

        public static void NextCh()
        {
            if (EndOfFile) 
            {
                return;
            }
            if (_positionNow.CharNumber == _lastInLine)
            {
                Console.WriteLine($"{_positionNow.LineNumber,4}  {_line}");
                if (_err != null && _err.Count > 0)
                {
                    ListErrors();
                }
                ReadNextLine();
                _positionNow = new TextPosition(_positionNow.LineNumber + 1, 0);
                if (EndOfFile) 
                {
                    return;
                }
            }
            else 
            {
                _positionNow = new TextPosition(_positionNow.LineNumber, (byte)(_positionNow.CharNumber + 1));
            }
            
            _ch = _line[_positionNow.CharNumber];
        }

        private static void ReadNextLine()
        {
            if (!_file.EndOfStream)
            {
                _line = _file.ReadLine();
                _lastInLine = (byte)(_line.Length - 1);
                _err = new List<Err>();
            }
            else
            {
                _endOfFile = true;
                Console.WriteLine($"\nКомпиляция завершена: ошибок — {_errCount}!");
                _file.Close();
            }
        }

        private static void ListErrors()
        {
            int pos = 6 - $"{_positionNow.LineNumber} ".Length;
            string s;
            foreach (Err item in _err)
            {
                ++_errCount;
                s = "**";
                if (_errCount < 10) 
                {
                    s += "0";
                }
                s += $"{_errCount}**";
                while (s.Length - 1 < pos + item.ErrorPosition.CharNumber) 
                {
                    s += " ";
                }
                s += $"^ ошибка код {item.ErrorCode}";
                Console.WriteLine(s);

                string desc = GetErrorDescription(item.ErrorCode);
                if (!string.IsNullOrEmpty(desc))
                {
                    Console.WriteLine(new string(' ', pos + item.ErrorPosition.CharNumber + 1) + desc);
                }
            }
        }

        private static string GetErrorDescription(byte code)
        {
            switch (code)
            {
                case 100: 
                    return "использование имени не соответствует описанию";
                case 147: 
                    return "тип метки не совпадает с типом выбирающего выражения";
                case 203: 
                    return "целая константа превышает допустимый диапазон";
                default: 
                    return null;
            }
        }

        public static void Error(byte errorCode)
        {
            if (_err != null && _err.Count <= ERRMAX)
            {
                _err.Add(new Err(_positionNow, errorCode));
            }
        }
    }
}
