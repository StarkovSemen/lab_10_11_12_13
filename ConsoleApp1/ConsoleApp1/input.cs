using System;
using System.Collections.Generic;
using System.IO;

namespace Компилятор
{
    struct TextPosition
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
            _errorTable = new Dictionary<byte, string>
            {
                { 1,   "ошибка ввода-вывода" },
                { 2,   "слишком много ошибок в строке" },
                { 50,  "неверный символ в программе" },
                { 51,  "пропущен идентификатор" },
                { 52,  "пропущена точка с запятой" },
                { 53,  "пропущена точка" },
                { 54,  "пропущено двоеточие" },
                { 55,  "пропущена запятая" },
                { 56,  "пропущена левая скобка" },
                { 57,  "пропущена правая скобка" },
                { 58,  "пропущен оператор присваивания :=" },
                { 100, "использование имени не соответствует описанию" },
                { 101, "ожидалось ключевое слово begin" },
                { 102, "ожидалось ключевое слово end" },
                { 103, "пропущено ключевое слово program" },
                { 147, "тип метки не совпадает с типом выбирающего выражения" },
                { 200, "целочисленная константа вне диапазона" },
                { 201, "вещественная константа вне диапазона" },
                { 202, "недопустимый символ в строке" },
                { 203, "константа превышает допустимый предел" },
                { 250, "неожиданный конец файла" }
            };
        }

        public static TextPosition PositionNow
        {
            get => _positionNow;
            set => _positionNow = value;
        }

        public static List<Err> Err => _err;
        public static Dictionary<byte, string> ErrorTable => _errorTable;

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
                return;

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
                Console.WriteLine(_positionNow.LineNumber.ToString().PadLeft(4) + " " + _line);
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
                return;

            foreach (Err item in _errorsByLine[lineNumber])
            {
                _errCount++;
                string errorLine = ("**" + _errCount.ToString().PadLeft(2, '0') + "**")
                    .PadRight(5 + item.ErrorPosition.CharNumber) + "^ ошибка код " + item.ErrorCode;
                Console.WriteLine(errorLine);
                Console.WriteLine("****** " + (_errorTable.TryGetValue(item.ErrorCode, out string? desc) ? desc : ""));
            }
        }

        public static void Error(byte errorCode, TextPosition position)
        {
            uint lineNum = position.LineNumber;
            if (!_errorsByLine.ContainsKey(lineNum))
                _errorsByLine[lineNum] = new List<Err>();
            if (_errorsByLine[lineNum].Count <= _errMax)
                _errorsByLine[lineNum].Add(new Err(position, errorCode));
        }

        public static void PrintErrorTable()
        {
            Console.WriteLine("\n=== ТАБЛИЦА ОШИБОК ===");
            Console.WriteLine("Код | Описание");
            Console.WriteLine("----+---------");
            foreach (var item in _errorTable)
                Console.WriteLine(item.Key.ToString().PadLeft(3) + " | " + item.Value);
            Console.WriteLine();
        }
    }
}
