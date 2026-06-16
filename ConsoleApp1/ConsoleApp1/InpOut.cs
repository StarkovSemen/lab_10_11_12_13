using System;
using System.Collections.Generic;
using System.IO;

namespace Компилятор
{
    public struct TextPosition
    {
        public uint LineNumber { get; set; }
        public byte CharNumber { get; set; }

        public TextPosition(uint ln = 0, byte c = 0)
        {
            LineNumber = ln;
            CharNumber = c;
        }
    }

    struct Err
    {
        public TextPosition ErrorPosition { get; set; }
        public byte ErrorCode { get; set; }

        public Err(TextPosition errorPosition, byte errorCode)
        {
            ErrorPosition = errorPosition;
            ErrorCode = errorCode;
        }
    }

    class InputOutput
    {
        private const byte _errMax = 9;

        public static char Ch { get; set; }
        private static TextPosition _positionNow;
        private static string _line;
        private static byte _lastInLine;
        private static List<Err> _err;
        private static StreamReader _file;
        private static uint _errCount;
        private static bool _endOfFile;
        private static Dictionary<byte, string> _errorTable;
        private static Dictionary<uint, List<Err>> _errorsByLine;

        static InputOutput()
        {
            _line = "";
            _err = new List<Err>();
            _errorsByLine = new Dictionary<uint, List<Err>>();
            _errorTable = new Dictionary<byte, string>();

            _errorTable.Add((byte)1, "ошибка ввода-вывода");
            _errorTable.Add((byte)2, "слишком много ошибок в строке");
            _errorTable.Add((byte)50, "неверный символ в программе");
            _errorTable.Add((byte)51, "пропущен идентификатор");
            _errorTable.Add((byte)52, "пропущена точка с запятой");
            _errorTable.Add((byte)53, "пропущена точка");
            _errorTable.Add((byte)54, "пропущено двоеточие");
            _errorTable.Add((byte)55, "пропущена запятая");
            _errorTable.Add((byte)56, "пропущена левая скобка");
            _errorTable.Add((byte)57, "пропущена правая скобка");
            _errorTable.Add((byte)58, "пропущен оператор присваивания :=");
            _errorTable.Add((byte)100, "использование имени не соответствует описанию");
            _errorTable.Add((byte)101, "ожидалось ключевое слово begin");
            _errorTable.Add((byte)102, "ожидалось ключевое слово end");
            _errorTable.Add((byte)103, "пропущено ключевое слово program");
            _errorTable.Add((byte)147, "тип метки не совпадает с типом выбирающего выражения");
            _errorTable.Add((byte)200, "целочисленная константа вне диапазона");
            _errorTable.Add((byte)201, "вещественная константа вне диапазона");
            _errorTable.Add((byte)202, "недопустимый символ в строке");
            _errorTable.Add((byte)203, "константа превышает допустимый предел");
            _errorTable.Add((byte)250, "неожиданный конец файла");
            _errorTable.Add((byte)30, "синтаксическая ошибка: пропущено ключевое слово var");
            _errorTable.Add((byte)31, "синтаксическая ошибка: пропущен идентификатор");
            _errorTable.Add((byte)32, "синтаксическая ошибка: пропущено двоеточие");
            _errorTable.Add((byte)33, "синтаксическая ошибка: пропущен тип");
            _errorTable.Add((byte)34, "синтаксическая ошибка: пропущено ключевое слово procedure");
            _errorTable.Add((byte)35, "синтаксическая ошибка: пропущено ключевое слово begin");
            _errorTable.Add((byte)36, "синтаксическая ошибка: пропущено ключевое слово end");
            _errorTable.Add((byte)37, "синтаксическая ошибка: пропущен оператор присваивания :=");
            _errorTable.Add((byte)38, "синтаксическая ошибка: пропущена точка с запятой");
            _errorTable.Add((byte)39, "синтаксическая ошибка: неверный оператор");
        }

        public static TextPosition PositionNow
        {
            get => _positionNow;
            set => _positionNow = value;
        }

        public static List<Err> Err => _err;
        public static Dictionary<byte, string> ErrorTable => _errorTable;
        public static Dictionary<uint, List<Err>> ErrorsByLine => _errorsByLine;

        public static void OpenFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Ошибка: файл " + filePath + " не найден!");
                return;
            }

            _file = new StreamReader(filePath);
            _errCount = 0;
            _endOfFile = false;
            _positionNow = new TextPosition(1, 0);
            _errorsByLine = new Dictionary<uint, List<Err>>();

            ReadNextLine();

            if (!string.IsNullOrEmpty(_line))
            {
                Ch = _line[0];
                _lastInLine = (byte)(_line.Length - 1);
            }
            else
            {
                Ch = '\0';
                _lastInLine = 0;
                _endOfFile = true;
                End();
            }
        }

        public static void CloseFile()
        {
            _file?.Close();
            _file = null;
        }

        public static void NextCh()
        {
            if (_endOfFile)
            {
                 return;
            }

            if (_positionNow.CharNumber >= _lastInLine)
            {
                ListThisLine();
                ListErrorsForLine(_positionNow.LineNumber);
                ReadNextLine();

                if (_endOfFile)
                {
                    Ch = '\0';
                    End();
                    return;
                }

                _positionNow.LineNumber++;
                _positionNow.CharNumber = 0;
            }
            else
            {
                _positionNow.CharNumber++;
            }

            Ch = (!_endOfFile && _line != null && _positionNow.CharNumber < _line.Length)
                ? _line[_positionNow.CharNumber]
                : '\0';
        }

        private static void ListThisLine()
        {
            if (_line != null)
            {
                Console.WriteLine(_positionNow.LineNumber.ToString().PadLeft(4) + " " + _line);
            }
                
        }

        private static void ReadNextLine()
        {
            if (_file != null && !_file.EndOfStream)
            {
                _line = _file.ReadLine();
                if (_line == null)
                {
                    _line = "";
                    _endOfFile = true;
                }
                else
                {
                    _lastInLine = _line.Length > 0 ? (byte)(_line.Length - 1) : (byte)0;
                }
            }
            else
            {
                _line = "";
                _lastInLine = 0;
                _endOfFile = true;
            }
        }

        static void End()
        {
            Console.WriteLine();
            Console.WriteLine("Компиляция окончена: ошибок - " + _errCount + " !");
            _endOfFile = true;
            Ch = '\0';
            _file?.Close();
            _file = null;
        }

        private static void ListErrorsForLine(uint lineNumber)
        {
            if (!_errorsByLine.ContainsKey(lineNumber))
            {
                return;
            }
               

            foreach (Err item in _errorsByLine[lineNumber])
            {
                _errCount++;
                string errorLine = ("**" + _errCount.ToString().PadLeft(2, '0') + "**")
                    .PadRight(5 + item.ErrorPosition.CharNumber) + "^ ошибка код " + item.ErrorCode;
                Console.WriteLine(errorLine);
                string desc;
                Console.WriteLine("****** " + (_errorTable.TryGetValue(item.ErrorCode, out desc) ? desc : ""));
            }
        }

        public static void Error(byte errorCode, TextPosition position)
        {
            uint lineNum = position.LineNumber;
            if (!_errorsByLine.ContainsKey(lineNum))
            {
                 _errorsByLine[lineNum] = new List<Err>();
            }
               
            if (_errorsByLine[lineNum].Count <= _errMax)
            {
                _errorsByLine[lineNum].Add(new Err(position, errorCode));
            }
               
        }

        public static void PrintErrorTable()
        {
            Console.WriteLine("\n");
            Console.WriteLine("Код | Описание");
            Console.WriteLine("----+---------");
            foreach (var item in _errorTable)
                Console.WriteLine(item.Key.ToString().PadLeft(3) + " | " + item.Value);
            Console.WriteLine();
        }
    }
}
