using System.Collections.Generic;

namespace Компилятор
{
    class Keywords
    {
        public Dictionary<byte, Dictionary<string, byte>> Kw { get; } = new Dictionary<byte, Dictionary<string, byte>>
        {
            [2] = new Dictionary<string, byte>
            {
                ["do"] = LexicalAnalyzer.dosy,
                ["if"] = LexicalAnalyzer.ifsy,
                ["in"] = LexicalAnalyzer.insy,
                ["of"] = LexicalAnalyzer.ofsy,
                ["or"] = LexicalAnalyzer.orsy,
                ["to"] = LexicalAnalyzer.tosy,
            },
            [3] = new Dictionary<string, byte>
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
            },
            [4] = new Dictionary<string, byte>
            {
                ["then"] = LexicalAnalyzer.thensy,
                ["else"] = LexicalAnalyzer.elsesy,
                ["case"] = LexicalAnalyzer.casesy,
                ["file"] = LexicalAnalyzer.filesy,
                ["goto"] = LexicalAnalyzer.gotosy,
                ["type"] = LexicalAnalyzer.typesy,
                ["with"] = LexicalAnalyzer.withsy,
            },
            [5] = new Dictionary<string, byte>
            {
                ["begin"] = LexicalAnalyzer.beginsy,
                ["while"] = LexicalAnalyzer.whilesy,
                ["array"] = LexicalAnalyzer.arraysy,
                ["const"] = LexicalAnalyzer.constsy,
                ["label"] = LexicalAnalyzer.labelsy,
                ["until"] = LexicalAnalyzer.untilsy,
            },
            [6] = new Dictionary<string, byte>
            {
                ["downto"] = LexicalAnalyzer.downtosy,
                ["packed"] = LexicalAnalyzer.packedsy,
                ["record"] = LexicalAnalyzer.recordsy,
                ["repeat"] = LexicalAnalyzer.repeatsy,
            },
            [7] = new Dictionary<string, byte> { ["program"] = LexicalAnalyzer.programsy },
            [8] = new Dictionary<string, byte> { ["function"] = LexicalAnalyzer.functionsy },
            [9] = new Dictionary<string, byte> { ["procedure"] = LexicalAnalyzer.procedurensy },
        };
    }
}
