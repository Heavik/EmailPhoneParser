using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EmailPhoneParser
{
    public class Parser
    {
        private const string EmailWrapper = "<a href=\"mailto:{0}\">{0}</a>";
        private const string PhoneWrapper = "<a href=\"tel:{0}\">{0}</a>";

        private static readonly Regex[] EmailPatterns =
        {
            new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.Compiled)
        };

        private static readonly Regex[] PhonePatterns =
        {
            new Regex(@"^\d{3}-\d{3}-\d{4}$", RegexOptions.Compiled)
        };

        private static readonly Regex[] PhonePrefixesPatterns =
        {
            new Regex(@"^\(\d{3}\)$", RegexOptions.Compiled)
        };

        private static readonly Regex[] PhoneSuffixesPatterns =
        {
            new Regex(@"^\d{3}-\d{4}$", RegexOptions.Compiled)
        };
        
        private readonly string text;
        
        public Parser(string text)
        {
            this.text = text;
        }

        public string[] GetAllEmails()
        {
            return GetMatchValues(text, EmailPatterns);
        }

        public string[] GetAllPhones()
        {
            return GetMatchValues(text, PhonePatterns);
        }

        public string FormatText()
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                string normalized = NormalizeWord(word);
                foreach (Regex pattern in EmailPatterns)
                {
                    if (pattern.IsMatch(normalized))
                    {
                        words[i] = WrapWord(word, normalized, EmailWrapper);
                    }
                }

                foreach (Regex pattern in PhonePatterns)
                {
                    if (pattern.IsMatch(normalized))
                    {
                        words[i] = WrapWord(word, normalized, PhoneWrapper);
                    }
                }

                foreach (Regex pattern in PhonePrefixesPatterns)
                {
                    if (pattern.IsMatch(normalized))
                    {
                        string nextWord = i + 1 >= words.Length ? string.Empty : words[i + 1];
                        string normalizedNextWord = NormalizeWord(nextWord);
                        foreach (Regex suffixPattern in PhoneSuffixesPatterns)
                        {
                            if (suffixPattern.IsMatch(normalizedNextWord))
                            {
                                words[i] = WrapWord($"{word} {nextWord}", $"{normalized} {normalizedNextWord}",
                                    PhoneWrapper);
                                words[i + 1] = string.Empty;
                                i++;
                            }
                        }
                    }
                }
            }

            return string.Join(" ", words.Where(word => !string.IsNullOrWhiteSpace(word)));
        }

        private static string[] GetMatchValues(string text, Regex[] patterns)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new string[] {};
            }

            var values = new List<string>();
            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in words)
            {
                string normalized = NormalizeWord(word);
                values.AddRange(patterns.Where(pattern => pattern.IsMatch(normalized)).Select(pattern => normalized));
            }

            return values.ToArray();
        }

        private static string NormalizeWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return string.Empty;
            }

            string normalized = word.Trim(',', '.', '!', '?', '-', ' ');
            return normalized;
        }

        private static string WrapWord(string word, string normalized, string wrapper)
        {
            string prefix = string.Empty;
            string suffix = string.Empty;
            int position = word.IndexOf(normalized, StringComparison.Ordinal);
            if (position > 0)
            {
                prefix = word.Substring(0, position);
            }

            if (word.Length - position > normalized.Length)
            {
                suffix = word.Substring(normalized.Length + position, word.Length - normalized.Length - position);
            }

            return $"{prefix}{string.Format(wrapper, normalized)}{suffix}";
        }
    }
}