namespace finsyncapi.Helpers
{
    public static class Base62Converter
    {
        private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static string Encode(long number)
        {
            if (number == 0) return "0";

            var result = "";
            while (number > 0)
            {
                result = Alphabet[(int)(number % 62)] + result;
                number /= 62;
            }
            return result;
        }

        public static long Decode(string encoded)
        {
            long result = 0;
            foreach (var c in encoded)
            {
                result = result * 62 + Alphabet.IndexOf(c);
            }
            return result;
        }
    }
}
