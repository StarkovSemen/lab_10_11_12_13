namespace Компилятор
{
    public struct Err
    {
        private TextPosition _errorPosition;
        private byte _errorCode;

        public TextPosition ErrorPosition
        {
            get
            {
                return _errorPosition;
            }
            set
            {
                _errorPosition = value;
            }
        }

        public byte ErrorCode
        {
            get
            {
                return _errorCode;
            }
            set
            {
                _errorCode = value;
            }
        }

        public Err(TextPosition position, byte code)
        {
            _errorPosition = position;
            _errorCode = code;
        }
    }
}