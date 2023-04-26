using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeokaEstetica.Platform.CallCenter.Builders
{
    public static class ClearHtmlBuilder
    {
        /// <summary>
        /// Метод очищает текст от html-тегов.
        /// </summary>
        /// <param name="text">Текст до очистки.</param>
        /// <returns>Очищенный текст.</returns>
        public static string Clear(string text)
        {
            return Regex.Replace(text, "<.*?>", string.Empty);
        }
    }
}
