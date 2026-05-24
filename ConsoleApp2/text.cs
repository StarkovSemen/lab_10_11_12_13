namespace Компилятор
{
    public struct TextPosition
    {
        public uint LineNumber;
        public byte CharNumber;

        public TextPosition(uint ln = 0, byte c = 0)
        {
            LineNumber = ln;
            CharNumber = c;
        }
    }
}