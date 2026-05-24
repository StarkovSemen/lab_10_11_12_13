using System.Collections.Generic;

namespace Компилятор
{
    static class ErrorTable
    {
        private static readonly Dictionary<byte, string>
            _messages = new Dictionary<byte, string>
        {
            { 100, "использование имени не соответствует описанию" },
            { 147, "тип метки не совпадает с типом выбирающего выражения" },
            { 203, "целая константа превышает допустимый диапазон" }
        };

        public static string? GetDescription(byte code)
        {
            _messages.TryGetValue(code, out string? desc);
            return desc;
        }
    }
}