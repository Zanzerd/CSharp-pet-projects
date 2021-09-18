using System;
using System.Collections.Generic;
using System.Text;

namespace Ngrams_ver2
{
    class Generator
    {
        public static string ContinuePhrase(
            Dictionary<string, string> nextWords,
            string phraseBeginning,
            int wordsCount)
        {
            string[] wordsBeginning = phraseBeginning.Split(' ');
            List<string> phrase = new List<string>();
            foreach (var word in wordsBeginning)
            {
                phrase.Add(word);
            }
            for (int i = 0; i < wordsCount; i++)
            {
                if (phrase.Count >= 2 && nextWords.ContainsKey(phrase[phrase.Count - 2] + " " +
                    phrase[phrase.Count - 1]))
                    phrase.Add(nextWords[phrase[phrase.Count - 2] + " " + phrase[phrase.Count - 1]]);
                else if (nextWords.ContainsKey(phrase[phrase.Count - 1]))
                    phrase.Add(nextWords[phrase[phrase.Count - 1]]);
                else
                    break;
            }
            var result = string.Join(" ", phrase);
            return result;
        }
    }
}
