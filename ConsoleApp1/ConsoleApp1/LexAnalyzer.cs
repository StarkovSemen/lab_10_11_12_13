using System;
using System.Collections.Generic;

namespace Компилятор
{
    public struct TokenInfo
    {
        public byte Type;
        public string Lexeme;
        public TextPosition Position;
        public int IntValue;
        public float FloatValue;
    }

    class LexicalAnalyzer
    {
        public const byte
            star = 21, slash = 60, equal = 16, comma = 20, semicolon = 14,
            colon = 5, point = 61, arrow = 62, leftpar = 9, rightpar = 4,
            lbracket = 11, rbracket = 12, flpar = 63, frpar = 64,
            later = 65, greater = 66, laterequal = 67, greaterequal = 68,
            latergreater = 69, plus = 70, minus = 71, lcomment = 72,
            rcomment = 73, assign = 51, twopoints = 74,
            ident = 2, floatc = 82, intc = 15, charc = 80, stringc = 81,
            casesy = 31, elsesy = 32, filesy = 57, gotosy = 33, thensy = 52,
            typesy = 34, untilsy = 53, dosy = 54, withsy = 37, ifsy = 56,
            insy = 100, ofsy = 101, orsy = 102, tosy = 103, endsy = 104,
            varsy = 105, divsy = 106, andsy = 107, notsy = 108, forsy = 109,
            modsy = 110, nilsy = 111, setsy = 112, beginsy = 113,
            whilesy = 114, arraysy = 115, constsy = 116, labelsy = 117,
            downtosy = 118, packedsy = 119, recordsy = 120, repeatsy = 121,
            programsy = 122, functionsy = 123, procedurensy = 124;

        private byte _symbol;
        private TextPosition _token;
        private string _addrName;
        private int _nmbInt;
        private float _nmbFloat;
        private char _oneSymbol;
        private string _stringValue;
        private readonly KeyWords _keywords;
        private readonly List<byte> _outputCodes;
        private readonly Dictionary<uint, List<byte>> _codesByLine;
        private readonly List<TokenInfo> _tokens;

        public LexicalAnalyzer()
        {
            _addrName = "";
            _stringValue = "";
            _keywords = new KeyWords();
            _outputCodes = new List<byte>();
            _codesByLine = new Dictionary<uint, List<byte>>();
            _tokens = new List<TokenInfo>();
        }

        public byte Symbol => _symbol;
        public TextPosition Token => _token;
        public string AddrName => _addrName;
        public int NmbInt => _nmbInt;
        public float NmbFloat => _nmbFloat;
        public char OneSymbol => _oneSymbol;
        public string StringValue => _stringValue;
        public List<byte> OutputCodes => _outputCodes;
        public Dictionary<uint, List<byte>> CodesByLine => _codesByLine;
        public List<TokenInfo> Tokens => _tokens;

        private byte FindKeyword(string name)
        {
            string lower = name.ToLower();
            if (_keywords.Kw.TryGetValue((byte)lower.Length, out var byLen) &&
                byLen.TryGetValue(lower, out byte code))
            {
                return code;
            }
            return 0;
        }

        private static bool IsLetter(char ch) =>
            (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == '_';

        private static bool IsDigit(char ch) => ch >= '0' && ch <= '9';

        private void AddCode(byte code)
        {
            _outputCodes.Add(code);
            uint lineNum = _token.LineNumber;
            if (!_codesByLine.ContainsKey(lineNum))
            {
                _codesByLine[lineNum] = new List<byte>();
            }       
            _codesByLine[lineNum].Add(code);
        }

        private void SkipComment()
        {
            InputOutput.NextCh();
            InputOutput.NextCh();

            while (true)
            {
                if (InputOutput.Ch == '\0')
                {
                    InputOutput.Error(250, InputOutput.PositionNow);
                    return;
                }

                if (InputOutput.Ch == '*')
                {
                    InputOutput.NextCh();
                    if (InputOutput.Ch == ')') { InputOutput.NextCh(); return; }
                }
                else
                {
                    InputOutput.NextCh();
                }
            }
        }

        private void SkipBraceComment()
        {
            InputOutput.NextCh();
            while (InputOutput.Ch != '}' && InputOutput.Ch != '\0')
            {
                InputOutput.NextCh();
            }               
            if (InputOutput.Ch == '\0')
            {
                InputOutput.Error(250, InputOutput.PositionNow);
            }   
            else
            {
                InputOutput.NextCh();
            }
        }

        public byte NextSym()
        {
            while (InputOutput.Ch == ' ' || InputOutput.Ch == '\t' ||
                   InputOutput.Ch == '\n' || InputOutput.Ch == '\r')
            {
                InputOutput.NextCh();
            }
                

            _token = InputOutput.PositionNow;
            char currentCh = InputOutput.Ch;

            if (currentCh == '\0')
            {
                _symbol = 0;
                _tokens.Add(new TokenInfo { Type = 0, Lexeme = "EOF", Position = _token });
                return 0;
            }

            if (IsLetter(currentCh))
            {
                string name = "";
                while (IsLetter(InputOutput.Ch) || IsDigit(InputOutput.Ch))
                {
                    name += InputOutput.Ch;
                    InputOutput.NextCh();
                }

                byte keywordCode = FindKeyword(name);
                _symbol = keywordCode > 0 ? keywordCode : ident;
                _addrName = name;
                AddCode(_symbol);
                _tokens.Add(new TokenInfo { Type = _symbol, Lexeme = name, Position = _token });
                return _symbol;
            }

            if (IsDigit(currentCh))
            {
                _nmbInt = 0;
                int maxint = 32767;

                while (IsDigit(InputOutput.Ch))
                {
                    byte digit = (byte)(InputOutput.Ch - '0');

                    if (_nmbInt <= maxint / 10)
                    {
                        _nmbInt = 10 * _nmbInt + digit;
                    }
                    else
                    {
                        InputOutput.Error(203, InputOutput.PositionNow);
                        _nmbInt = 0;
                        while (IsDigit(InputOutput.Ch))
                            InputOutput.NextCh();
                        break;
                    }

                    InputOutput.NextCh();
                }

                if (_nmbInt < -32768 || _nmbInt > 32767)
                {
                    InputOutput.Error(200, _token);
                }

                if (InputOutput.Ch == '.')
                {
                    InputOutput.NextCh();

                    if (IsDigit(InputOutput.Ch))
                    {
                        _nmbFloat = (float)_nmbInt;
                        float fraction = 0.1f;

                        while (IsDigit(InputOutput.Ch))
                        {
                            _nmbFloat += (byte)(InputOutput.Ch - '0') * fraction;
                            fraction *= 0.1f;
                            InputOutput.NextCh();
                        }

                        _symbol = floatc;
                    }
                    else
                    {
                        _symbol = intc;
                    }
                }
                else
                {
                    _symbol = intc;
                }

                AddCode(_symbol);
                _tokens.Add(new TokenInfo
                {
                    Type = _symbol,
                    Lexeme = _symbol == intc ? _nmbInt.ToString() : _nmbFloat.ToString(),
                    Position = _token,
                    IntValue = _nmbInt,
                    FloatValue = _nmbFloat
                });
                return _symbol;
            }

            switch (currentCh)
            {
                case '+': _symbol = plus; InputOutput.NextCh(); break;
                case '-': _symbol = minus; InputOutput.NextCh(); break;
                case '*': _symbol = star; InputOutput.NextCh(); break;
                case '/': _symbol = slash; InputOutput.NextCh(); break;
                case '=': _symbol = equal; InputOutput.NextCh(); break;
                case ',': _symbol = comma; InputOutput.NextCh(); break;
                case ';': _symbol = semicolon; InputOutput.NextCh(); break;
                case ')': _symbol = rightpar; InputOutput.NextCh(); break;
                case '[': _symbol = lbracket; InputOutput.NextCh(); break;
                case ']': _symbol = rbracket; InputOutput.NextCh(); break;
                case '^': _symbol = arrow; InputOutput.NextCh(); break;

                case '}':
                    _symbol = frpar;
                    InputOutput.NextCh();
                    break;

                case '(':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '*') 
                    { 
                        SkipComment(); return NextSym(); 
                    }
                    _symbol = leftpar;
                    break;

                case '{':
                    SkipBraceComment();
                    return NextSym();

                case '<':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=') 
                    { 
                        _symbol = laterequal; InputOutput.NextCh(); 
                    }
                    else if (InputOutput.Ch == '>') 
                    { 
                        _symbol = latergreater; InputOutput.NextCh();
                    }
                    else 
                    { 
                        _symbol = later; 
                    }
                    break;

                case '>':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=') 
                    { 
                        _symbol = greaterequal; InputOutput.NextCh(); 
                    }
                    else 
                    { 
                        _symbol = greater; 
                    }
                    break;

                case ':':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '=') 
                    {
                        _symbol = assign; InputOutput.NextCh();
                    }
                    else 
                    { 
                        _symbol = colon; 
                    }
                    break;

                case '.':
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '.') 
                    { 
                        _symbol = twopoints; InputOutput.NextCh(); 
                    }
                    else 
                    { 
                        _symbol = point; 
                    }
                    break;

                case '\'':
                    InputOutput.NextCh();
                    _oneSymbol = InputOutput.Ch;
                    InputOutput.NextCh();

                    if (InputOutput.Ch == '\'')
                    {
                        _symbol = charc;
                        InputOutput.NextCh();
                    }
                    else
                    {
                        _stringValue = _oneSymbol.ToString();
                        while (InputOutput.Ch != '\'' && InputOutput.Ch != '\0')
                        {
                            _stringValue += InputOutput.Ch;
                            InputOutput.NextCh();
                        }

                        if (InputOutput.Ch == '\'')
                        {
                            _symbol = stringc;
                            InputOutput.NextCh();
                        }
                        else
                        {
                            InputOutput.Error(202, InputOutput.PositionNow);
                            _symbol = 0;
                        }
                    }
                    break;

                default:
                    InputOutput.Error(50, InputOutput.PositionNow);
                    _symbol = 0;
                    InputOutput.NextCh();
                    break;
            }

            AddCode(_symbol);
            _tokens.Add(new TokenInfo { Type = _symbol, Lexeme = currentCh.ToString(), Position = _token });
            return _symbol;
        }

        public void PrintOutputCodesByLine()
        {
            Console.WriteLine("\n");
            foreach (var kvp in _codesByLine)
                Console.WriteLine("Строка " + kvp.Key + ": " + string.Join(" ", kvp.Value));

            string codesFile = "codes.txt";
            System.IO.File.WriteAllText(codesFile, string.Join(" ", _outputCodes));
        }
    }
}
