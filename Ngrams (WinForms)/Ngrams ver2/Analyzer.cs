using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ngrams_ver2
{
    class Analyzer
    {
        public static int d = 0;
        public static Dictionary<string, string> GetMostFrequentNextWords(List<List<string>> text)
        {
            var result1 = GetBigrams(text);
            var result2 = GetTrigrams(text);
            result2.ToList().ForEach(x => result1.Add(x.Key, x.Value));
            return result1;
        }

        public static Dictionary<string, string> GetBigrams(List<List<string>> text)
        {
            var result = new Dictionary<string, string>();
            var dictFrequency = new Dictionary<string, Dictionary<string, int>>();

            foreach (List<string> sentence in text)
            {
                for (int i = 0; i < sentence.Count - 1; i++)
                {
                    if (!dictFrequency.ContainsKey(sentence[i]))
                    {
                        dictFrequency[sentence[i]] = new Dictionary<string, int>();
                        dictFrequency[sentence[i]][sentence[i + 1]] = 1;
                    }
                    else if (dictFrequency.ContainsKey(sentence[i])
                        && !dictFrequency[sentence[i]].ContainsKey(sentence[i + 1]))
                    {
                        dictFrequency[sentence[i]][sentence[i + 1]] = 1;
                    }
                    else
                    {
                        dictFrequency[sentence[i]][sentence[i + 1]]++;
                    }
                }
            }

            foreach (var pair in dictFrequency)
            {
                string maxKey = GetMaxKey(dictFrequency, pair.Key);
                result[pair.Key] = maxKey;
            }

            return result;
        }

        public static Dictionary<string, string> GetTrigrams(List<List<string>> text)
        {
            var result = new Dictionary<string, string>();
            var dictFrequency = new Dictionary<string, Dictionary<string, int>>();

            foreach (List<string> sentence in text)
            {
                for (int i = 0; i < sentence.Count - 2; i++)
                {
                    if (!dictFrequency.ContainsKey(sentence[i] + " " + sentence[i + 1]))
                    {
                        dictFrequency[sentence[i] + " " + sentence[i + 1]] = new Dictionary<string, int>();
                        dictFrequency[sentence[i] + " " + sentence[i + 1]][sentence[i + 2]] = 1;
                    }
                    else if (dictFrequency.ContainsKey(sentence[i] + " " + sentence[i + 1])
                        && !dictFrequency[sentence[i] + " " + sentence[i + 1]].ContainsKey(sentence[i + 2]))
                    {
                        dictFrequency[sentence[i] + " " + sentence[i + 1]][sentence[i + 2]] = 1;
                    }
                    else
                    {
                        dictFrequency[sentence[i] + " " + sentence[i + 1]][sentence[i + 2]]++;
                    }
                }
            }

            foreach (var pair in dictFrequency)
            {
                string maxKey = GetMaxKey(dictFrequency, pair.Key);
                result[pair.Key] = maxKey;
            }

            return result;
        }

        public static string GetMaxKey(Dictionary<string, Dictionary<string, int>> dict, string byLetter)
        {
            KeyValuePair<string, int> max = new KeyValuePair<string, int>();
            foreach (var kvpInternal in dict[byLetter])
            {
                if (kvpInternal.Value > max.Value)
                    max = kvpInternal;
                else if (max.Value == kvpInternal.Value && string.CompareOrdinal(kvpInternal.Key, max.Key) < 0)
                    max = kvpInternal;
            }

            return max.Key;
        }
    }
}
