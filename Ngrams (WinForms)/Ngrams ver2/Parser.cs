using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ngrams_ver2
{
    class Parser
    {
        public static List<List<string>> ParseSentences(string text)
        {
            var sentencesList = new List<List<string>>();
            string[] sentences = text.Split(new char[] { '.', '!', '?', ':', ';', '(', ')', '[', ']' });
            for (int i = 0; i < sentences.Length; i++)
            {
                int symbolCounter = 0;
                sentences[i] = sentences[i].ToLower();
                sentences[i] = sentences[i].Trim();
                List<string> wordsList = new List<string>();
                var builder = new StringBuilder();
                foreach (char c in sentences[i])
                {
                    if (char.IsLetter(c) || c == '\'')
                    {
                        symbolCounter++;
                        builder.Append(c);
                    }
                    else
                    {
                        if (builder.Length > 0 && !string.IsNullOrEmpty(builder.ToString()))
                        {
                            wordsList.Add(builder.ToString());
                            builder.Clear();
                        }
                    }
                }
                if (builder.Length > 0)
                {
                    wordsList.Add(builder.ToString());
                    builder.Clear();
                }
                if (symbolCounter > 0)
                {
                    sentencesList.Add(wordsList.Where(word => !string.IsNullOrEmpty(word)).ToList());
                }
            }
            return sentencesList;
        }
    }
}
