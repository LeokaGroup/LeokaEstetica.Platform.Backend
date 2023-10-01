using System.Text.RegularExpressions;

namespace LeokaEstetica.Platform.Base.Extensions.HtmlExtensions
{
    /// <summary>
    /// Класс билдера очищает текст от html-тегов.
    /// </summary>
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
