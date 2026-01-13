namespace ChoresHub.Application.Common
{
    public class Utilities
    {

        public static string CapitalizeFirstLetter(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return char.ToUpper(text[0]) + text.Substring(1);
        }

    }
}