using System;
using System.Collections.Generic;

namespace Компилятор
{
    class SyntaxAnalyzer
    {
        private const byte ERR_VAR = 30;
        private const byte ERR_IDENT = 31;
        private const byte ERR_COLON = 32;
        private const byte ERR_TYPE = 33;
        private const byte ERR_PROC = 34;
        private const byte ERR_BEGIN = 35;
        private const byte ERR_END = 36;
        private const byte ERR_ASSIGN = 37;
        private const byte ERR_SEMI = 38;
        private const byte ERR_STMT = 39;

        private static Dictionary<byte, List<byte>> _syncTable;

        private readonly List<TokenInfo> _tokens;
        private int _position;

        static SyntaxAnalyzer()
        {
            _syncTable = new Dictionary<byte, List<byte>>();

            List<byte> universalSync = new List<byte>();
            universalSync.Add(LexicalAnalyzer.semicolon);
            universalSync.Add(LexicalAnalyzer.endsy);
            universalSync.Add(LexicalAnalyzer.beginsy);
            universalSync.Add(0);

            List<byte> varSync = new List<byte>();
            varSync.Add(LexicalAnalyzer.semicolon);
            varSync.Add(LexicalAnalyzer.beginsy);
            varSync.Add(LexicalAnalyzer.procedurensy);
            varSync.Add(LexicalAnalyzer.endsy);
            varSync.Add(0);

            List<byte> stmtSync = new List<byte>();
            stmtSync.Add(LexicalAnalyzer.semicolon);
            stmtSync.Add(LexicalAnalyzer.endsy);
            stmtSync.Add(0);

            _syncTable.Add(ERR_VAR, varSync);
            _syncTable.Add(ERR_IDENT, varSync);
            _syncTable.Add(ERR_COLON, varSync);
            _syncTable.Add(ERR_TYPE, varSync);
            _syncTable.Add(ERR_SEMI, universalSync);
            _syncTable.Add(ERR_PROC, universalSync);
            _syncTable.Add(ERR_BEGIN, stmtSync);
            _syncTable.Add(ERR_END, stmtSync);
            _syncTable.Add(ERR_ASSIGN, stmtSync);
            _syncTable.Add(ERR_STMT, stmtSync);
        }

        public SyntaxAnalyzer(LexicalAnalyzer lexer)
        {
            _tokens = lexer.Tokens;
            _position = 0;
        }

        public void Analyze()
        {
            _position = 0;

            if (_tokens.Count == 0 || (_tokens.Count == 1 && _tokens[0].Type == 0))
            {
                Console.WriteLine("Нет токенов для синтаксического анализа");
                return;
            }
            Program();
        }

        private TokenInfo CurrentToken
        {
            get
            {
                if (_position < _tokens.Count)
                {
                    return _tokens[_position];
                }                  
                return new TokenInfo { Type = 0, Lexeme = "EOF" };
            }
        }

        private void NextToken()
        {
            if (_position < _tokens.Count)
            {
                _position++;
            }        
        }

        private bool Check(byte type)
        {
            return CurrentToken.Type == type;
        }

        private void NeutralizeError(byte errorCode)
        {
            List<byte> syncTokens;

            if (!_syncTable.TryGetValue(errorCode, out syncTokens))
            {
                syncTokens = new List<byte>();
                syncTokens.Add(LexicalAnalyzer.semicolon);
                syncTokens.Add(LexicalAnalyzer.endsy);
                syncTokens.Add(0);
            }

            Console.WriteLine("    [Нейтрализация ошибки " + errorCode + "]");

            while (_position < _tokens.Count)
            {
                byte currentType = CurrentToken.Type;

                bool found = false;
                for (int i = 0; i < syncTokens.Count; i++)
                {
                    if (currentType == syncTokens[i])
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    Console.WriteLine("    Синхронизация на токене: " + CurrentToken.Lexeme);
                    break;
                }

                Console.WriteLine("    Пропущен токен: " + CurrentToken.Lexeme);
                NextToken();
            }
        }

        private bool Match(byte expected, byte errorCode)
        {
            if (Check(expected))
            {
                NextToken();
                return true;
            }
            else
            {
                TextPosition pos = CurrentToken.Position;
                InputOutput.Error(errorCode, pos);

                Console.WriteLine("  Ошибка: ожидался " + expected + ", встречен " + CurrentToken.Type + " (" + CurrentToken.Lexeme + ")");

                NeutralizeError(errorCode);

                if (Check(expected))
                {
                    NextToken();
                    return true;
                }

                return false;
            }
        }

        private void Program()
        {
            Console.WriteLine("Анализ программы...");

            if (Check(LexicalAnalyzer.programsy))
            {
                Match(LexicalAnalyzer.programsy, 103);
                Match(LexicalAnalyzer.ident, 51);
                Match(LexicalAnalyzer.semicolon, 52);
                Block();
                Match(LexicalAnalyzer.point, 53);
            }
            else
            {
                InputOutput.Error(103, CurrentToken.Position);
                Block();
            }
        }

        private void Block()
        {
            if (Check(LexicalAnalyzer.varsy))
            {
                VarDeclarations();
            }

            while (Check(LexicalAnalyzer.procedurensy) || Check(LexicalAnalyzer.functionsy))
            {
                ProcedureDeclaration();
            }

            CompoundStatement();
        }

        private void VarDeclarations()
        {
            Console.WriteLine("  Анализ описания переменных...");
            Match(LexicalAnalyzer.varsy, ERR_VAR);

            while (Check(LexicalAnalyzer.ident))
            {
                VarDeclaration();
            }
        }

        private void VarDeclaration()
        {
            Console.WriteLine("    Переменная: " + CurrentToken.Lexeme);
            Match(LexicalAnalyzer.ident, ERR_IDENT);

            while (Check(LexicalAnalyzer.comma))
            {
                Match(LexicalAnalyzer.comma, 55);
                Console.WriteLine("    Переменная: " + CurrentToken.Lexeme);
                Match(LexicalAnalyzer.ident, ERR_IDENT);
            }

            Match(LexicalAnalyzer.colon, ERR_COLON);
            Type_();
            Match(LexicalAnalyzer.semicolon, ERR_SEMI);
        }

        private void Type_()
        {
            if (Check(LexicalAnalyzer.ident))
            {
                Console.WriteLine("      Тип: " + CurrentToken.Lexeme);
                NextToken();
            }
            else
            {
                InputOutput.Error(ERR_TYPE, CurrentToken.Position);
                Console.WriteLine("  Ошибка: ожидался тип данных");
                NeutralizeError(ERR_TYPE);
            }
        }

        private void ProcedureDeclaration()
        {
            Console.WriteLine("  Анализ описания процедуры...");
            Match(LexicalAnalyzer.procedurensy, ERR_PROC);
            Console.WriteLine("    Имя процедуры: " + CurrentToken.Lexeme);
            Match(LexicalAnalyzer.ident, ERR_IDENT);
            Match(LexicalAnalyzer.semicolon, ERR_SEMI);
            Block();
            Match(LexicalAnalyzer.semicolon, ERR_SEMI);
        }

        private void CompoundStatement()
        {
            Console.WriteLine("  Анализ составного оператора...");
            Match(LexicalAnalyzer.beginsy, ERR_BEGIN);

            Statement();

            while (Check(LexicalAnalyzer.semicolon))
            {
                Match(LexicalAnalyzer.semicolon, ERR_SEMI);

                if (!Check(LexicalAnalyzer.endsy))
                {
                    Statement();
                }
            }

            Match(LexicalAnalyzer.endsy, ERR_END);
        }

        private void Statement()
        {
            if (Check(LexicalAnalyzer.ident))
            {
                int savedPos = _position;
                NextToken();

                if (Check(LexicalAnalyzer.assign) || Check(LexicalAnalyzer.colon))
                {
                    _position = savedPos;
                    Assignment();
                }
                else
                {
                    _position = savedPos;
                    ProcedureCall();
                }
            }
            else if (Check(LexicalAnalyzer.beginsy))
            {
                CompoundStatement();
            }
            else if (!Check(LexicalAnalyzer.endsy) && !Check(0))
            {
                InputOutput.Error(ERR_STMT, CurrentToken.Position);
                Console.WriteLine("  Ошибка: неверный оператор");
                NeutralizeError(ERR_STMT);
            }
        }

        private void Assignment()
        {
            Console.WriteLine("    Присваивание: " + CurrentToken.Lexeme + " := ...");
            Match(LexicalAnalyzer.ident, ERR_IDENT);
            if (Check(LexicalAnalyzer.colon))
            {
                Match(LexicalAnalyzer.colon, ERR_COLON);
                Match(LexicalAnalyzer.equal, ERR_ASSIGN);
            }
            else
            {
                Match(LexicalAnalyzer.assign, ERR_ASSIGN);
            }
            Expression();
        }

        private void ProcedureCall()
        {
            Console.WriteLine("    Вызов процедуры: " + CurrentToken.Lexeme);
            Match(LexicalAnalyzer.ident, ERR_IDENT);
        }

        private void Expression()
        {
            Term();

            while (Check(LexicalAnalyzer.plus) || Check(LexicalAnalyzer.minus))
            {
                NextToken();
                Term();
            }
        }

        private void Term()
        {
            Factor();

            while (Check(LexicalAnalyzer.star) || Check(LexicalAnalyzer.slash))
            {
                NextToken();
                Factor();
            }
        }

        private void Factor()
        {
            if (Check(LexicalAnalyzer.ident) ||
                Check(LexicalAnalyzer.intc) ||
                Check(LexicalAnalyzer.floatc) ||
                Check(LexicalAnalyzer.stringc))
            {
                NextToken();
            }
            else if (Check(LexicalAnalyzer.leftpar))
            {
                Match(LexicalAnalyzer.leftpar, 56);
                Expression();
                Match(LexicalAnalyzer.rightpar, 57);
            }
        }
    }
}
