namespace ICSS.Client.Helpers
{
    public static class StringHelper
    {
        public static string ToSentenceCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.Trim();

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
    }

}
