using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ulearnNGrams
{
    class Program
    {
        public static void Main(string[] args)
        {
            string path = File.ReadAllText(@"C:\Users\danil\Desktop\Blood.txt");
            var parseSentences = SentencesParserTask.ParseSentences(path);
            // var frequencyAnal = FrequencyAnalysisTask.GetMostFrequentNextWords(parseSentences);
            FrequencyAnalysisTask.GetMostFrequentNextWords(parseSentences);
        }
    }
    static class SentencesParserTask
    {
        public static List<List<string>> ParseSentences(string text)
        {
            var sentencesList = new List<List<string>>();
            char[] signs = new char[] { ';', '.', '!', '?', ':', '(', ')', '\n', '\r' };
            string[] sentences = text.Split(signs, StringSplitOptions.RemoveEmptyEntries);

            foreach (var words in sentences)
            {
                var sentencesSplitList = new List<string>();
                string[] worldsSplit = words.Split(' ', ',', '\"');
                for (int i = 0; i < worldsSplit.Length; i++)
                {
                    if (worldsSplit[i] != string.Empty)
                        sentencesSplitList.Add(worldsSplit[i]);
                }
                sentencesList.Add(sentencesSplitList);
            }

            return sentencesList;
        }
    }

    static class FrequencyAnalysisTask
    {
        public static Dictionary<string, string> GetMostFrequentNextWords(List<List<string>> text)
        {
            var result = new Dictionary<string, string>();

            Dictionary<string, string> treGram = TreGram(text);

            Dictionary<string, string> biGram = BiGram(text);

            foreach (var item in biGram)
                result.Add(item.Key, item.Value);

            foreach (var item in treGram)
                result.Add(item.Key, item.Value);

            foreach (var element in result)
                Console.WriteLine($"{element.Key}: {element.Value}");

            return result;
        }

        private static void ChangeElementsDictionary(ref Dictionary<(string begin, string end), int> frequencyDictionary, string beginWord, string endWord, string oldBegin, string oldEnd)
        {
            frequencyDictionary.Remove((oldBegin, oldEnd));
            frequencyDictionary.Add((beginWord, endWord), 1);
        }


        private static void AddFrequencyList(ref Dictionary<(string begin, string end), int> frequencyDictionary, string beginWord, string endWord)
        {
            bool flag = true;
            int frequency = 1;
            foreach (var nGram in frequencyDictionary)
            {
                if ((beginWord, endWord) == nGram.Key)
                {
                    frequency = nGram.Value + 1;
                    flag = false;
                }
                else if (beginWord == nGram.Key.begin && endWord != nGram.Key.end && nGram.Value == 1)
                {
                    if (string.CompareOrdinal(endWord, nGram.Key.end) < 0)
                    {
                        ChangeElementsDictionary(ref frequencyDictionary, beginWord, endWord, nGram.Key.begin, nGram.Key.end);
                        return;
                    }
                    else return;
                }
                else if (beginWord == nGram.Key.begin && endWord != nGram.Key.end && nGram.Value > 1) return;
            }

            if (flag)
            {
                frequencyDictionary.Add((beginWord, endWord), 1);
            }
            else frequencyDictionary[(beginWord, endWord)] = frequency;
        }
        private static Dictionary<string, string> TreGram(List<List<string>> text)
        {
            var frequencyDictionary = new Dictionary<(string begin, string end), int>();
            var treGram = new Dictionary<string, string>();

            foreach (var sentence in text)
            {
                if (sentence.Count >= 3)
                {
                    for (int i = 0; i < sentence.Count; i++)
                    {
                        if (sentence.Count - i >= 3)
                        {
                            string begin = sentence[i] + " " + sentence[i + 1];
                            AddFrequencyList(ref frequencyDictionary, begin, sentence[i + 2].ToString());
                        }
                    }
                }
            }

            foreach (var item in frequencyDictionary)
            {
                if (item.Value > 1)
                    treGram.Add(item.Key.begin, item.Key.end);

            }
            return treGram;
        }
        private static Dictionary<string, string> BiGram(List<List<string>> text)
        {
            var frequencyDictionary = new Dictionary<(string begin, string end), int>();
            var biGram = new Dictionary<string, string>();

            foreach (var sentence in text)
            {
                if (sentence.Count >= 2)
                {
                    for (int i = 0; i < sentence.Count; i++)
                    {
                        if (sentence.Count - i >= 2)
                        {
                            string begin = sentence[i];
                            AddFrequencyList(ref frequencyDictionary, begin, sentence[i + 1].ToString());
                        }
                    }
                }
            }

            foreach (var item in frequencyDictionary)
            {
                if (item.Value > 1)
                    biGram.Add(item.Key.begin, item.Key.end);

            }
            return biGram;
        }
    }
}
