using System.Collections.Generic;

namespace Компилятор
{
    class KeyWords
    {
        public Dictionary<byte, Dictionary<string, byte>> Kw { get; }

        public KeyWords()
        {
            Kw = new Dictionary<byte, Dictionary<string, byte>>();

            var kw2 = new Dictionary<string, byte>
            {
                ["do"] = LexicalAnalyzer.dosy,
                ["if"] = LexicalAnalyzer.ifsy,
                ["in"] = LexicalAnalyzer.insy,
                ["of"] = LexicalAnalyzer.ofsy,
                ["or"] = LexicalAnalyzer.orsy,
                ["to"] = LexicalAnalyzer.tosy,
            };
            Kw.Add((byte)2, kw2);

            var kw3 = new Dictionary<string, byte>
            {
                ["end"] = LexicalAnalyzer.endsy,
                ["var"] = LexicalAnalyzer.varsy,
                ["div"] = LexicalAnalyzer.divsy,
                ["and"] = LexicalAnalyzer.andsy,
                ["not"] = LexicalAnalyzer.notsy,
                ["for"] = LexicalAnalyzer.forsy,
                ["mod"] = LexicalAnalyzer.modsy,
                ["nil"] = LexicalAnalyzer.nilsy,
                ["set"] = LexicalAnalyzer.setsy,
            };
            Kw.Add((byte)3, kw3);

            var kw4 = new Dictionary<string, byte>
            {
                ["then"] = LexicalAnalyzer.thensy,
                ["else"] = LexicalAnalyzer.elsesy,
                ["case"] = LexicalAnalyzer.casesy,
                ["file"] = LexicalAnalyzer.filesy,
                ["goto"] = LexicalAnalyzer.gotosy,
                ["type"] = LexicalAnalyzer.typesy,
                ["with"] = LexicalAnalyzer.withsy,
            };
            Kw.Add((byte)4, kw4);

            var kw5 = new Dictionary<string, byte>
            {
                ["begin"] = LexicalAnalyzer.beginsy,
                ["while"] = LexicalAnalyzer.whilesy,
                ["array"] = LexicalAnalyzer.arraysy,
                ["const"] = LexicalAnalyzer.constsy,
                ["label"] = LexicalAnalyzer.labelsy,
                ["until"] = LexicalAnalyzer.untilsy,
            };
            Kw.Add((byte)5, kw5);

            var kw6 = new Dictionary<string, byte>
            {
                ["downto"] = LexicalAnalyzer.downtosy,
                ["packed"] = LexicalAnalyzer.packedsy,
                ["record"] = LexicalAnalyzer.recordsy,
                ["repeat"] = LexicalAnalyzer.repeatsy,
            };
            Kw.Add((byte)6, kw6);

            var kw7 = new Dictionary<string, byte> { ["program"] = LexicalAnalyzer.programsy };
            Kw.Add((byte)7, kw7);

            var kw8 = new Dictionary<string, byte> { ["function"] = LexicalAnalyzer.functionsy };
            Kw.Add((byte)8, kw8);

            var kw9 = new Dictionary<string, byte> { ["procedure"] = LexicalAnalyzer.procedurensy };
            Kw.Add((byte)9, kw9);
        }
    }
}
