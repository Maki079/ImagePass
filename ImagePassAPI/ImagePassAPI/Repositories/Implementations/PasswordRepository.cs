using ImagePassAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using System.Text.RegularExpressions;

namespace ImagePassAPI.Repositories.Implementations
{
    public class PasswordRepository : IPasswordRepository
    {
        public async Task<string> GeneratePasswordAsync(string sentense)
        {

            var words = Regex.Split(sentense, @"\W+").Where(s => s.Length > 0).ToArray();

            // delete short words
            words = words.Where(w => w.Length > 2).ToArray();

            Random rnd = new Random();

            int numWords = Math.Min(Math.Max(3, words.Length / 3), 4);

            // select numWords of indexes in words
            int[] indexes = Enumerable.Range(0, words.Length).OrderBy(x => rnd.Next()).Take(numWords).ToArray();

            Array.Sort(indexes);

            var selectedWords = indexes.Select(i => words[i]).ToArray();

            for (int i = 0; i < selectedWords.Length; i++)
            {
 
                int numDigits = rnd.Next(0, 2);

                string digits = new string(Enumerable.Repeat("0123456789", numDigits).Select(s => s[rnd.Next(s.Length)]).ToArray());

                if (rnd.NextDouble() > 0.5)
                {
                    selectedWords[i] = digits + selectedWords[i];
                }
                else
                {
                    selectedWords[i] += digits;
                }
  
                selectedWords[i] = new string(selectedWords[i].Select(c => ReplaceChar(c, rnd)).ToArray());


                int numToCapitalize = (int)Math.Ceiling(selectedWords[i].Length * 0.2);

                selectedWords[i] = new string(selectedWords[i].Select((c, index) => index < numToCapitalize ? char.ToUpper(c) : c).ToArray());
            }

            var result = string.Concat(selectedWords);

            return result;
        }
        public char ReplaceChar(char c, Random rnd)
        {
            var replacements = new Dictionary<char, char>
            {
                { 'I', '|' },
                { 'E', '3' },
                { 'S', '$' },
                { 'a', '@' },
                { 'O', '0' },
                { 'e', '&' },
                { 'A', '4' },
                { 'C', '(' },
                { 'l', '!' },
                { 'x', '+' },
                { 's', '5' }
            };

            if (rnd.NextDouble() < 0.3 && replacements.ContainsKey(c))
            {
                return replacements[c];
            }
            return c;
        }
    }
}
