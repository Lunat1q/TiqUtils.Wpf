using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MoreLinq;

namespace TiqUtils.Wpf.Helpers
{
    public static class CsvHelper
    {
        private static readonly Dictionary<LineSeparator, Func<string, string[]>> DictionaryOfLineSeparatorAndItsFunc = new Dictionary<LineSeparator, Func<string, string[]>>();

        static CsvHelper()
        {
            DictionaryOfLineSeparatorAndItsFunc[LineSeparator.Unknown] = ParseLineNotSeparated;
            DictionaryOfLineSeparatorAndItsFunc[LineSeparator.Tab] = ParseLineTabSeparated;
            DictionaryOfLineSeparatorAndItsFunc[LineSeparator.Semicolon] = ParseLineSemicolonSeparated;
            DictionaryOfLineSeparatorAndItsFunc[LineSeparator.Comma] = ParseLineCommaSeparated;
        }

        // ******************************************************************
        private enum LineSeparator
        {
            Unknown = 0,
            Tab,
            Semicolon,
            Comma
        }

        // ******************************************************************
        private static LineSeparator GuessCsvSeparator(string oneLine)
        {
            var listOfLineSeparatorAndThereFirstLineSeparatedValueCount = new List<Tuple<LineSeparator, int>>
            {
                new Tuple<LineSeparator, int>(LineSeparator.Tab, ParseLineTabSeparated(oneLine).Length),
                new Tuple<LineSeparator, int>(LineSeparator.Semicolon, ParseLineSemicolonSeparated(oneLine).Length),
                new Tuple<LineSeparator, int>(LineSeparator.Comma, ParseLineCommaSeparated(oneLine).Length)
            };


            var bestBet = listOfLineSeparatorAndThereFirstLineSeparatedValueCount.MaxBy((n) => n.Item2);

            if (bestBet != null && bestBet.Item2 > 1)
            {
                return bestBet.Item1;
            }

            return LineSeparator.Unknown;
        }

        // ******************************************************************
        public static string[] ParseLineCommaSeparated(string line)
        {
            // CSV line parsing : From "jgr4" in http://www.kimgentes.com/worshiptech-web-tools-page/2008/10/14/regex-pattern-for-parsing-csv-files-with-embedded-commas-dou.html
            var matches = Regex.Matches(line, @"\s?((?<x>(?=[,]+))|""(?<x>([^""]|"""")+)""|""(?<x>)""|(?<x>[^,]+)),?",
                RegexOptions.ExplicitCapture);

            var values = (from Match m in matches
                select m.Groups["x"].Value.Trim().Replace("\"\"", "\"")).ToArray();

            return values;
        }

        // ******************************************************************
        private static string[] ParseLineTabSeparated(string line)
        {
            var matchesTab = Regex.Matches(line, @"\s?((?<x>(?=[\t]+))|""(?<x>([^""]|"""")+)""|""(?<x>)""|(?<x>[^\t]+))\t?",
                RegexOptions.ExplicitCapture);

            var values = (from Match m in matchesTab
                select m.Groups["x"].Value.Trim().Replace("\"\"", "\"")).ToArray();

            return values;
        }

        // ******************************************************************
        private static string[] ParseLineSemicolonSeparated(string line)
        {
            // CSV line parsing : From "jgr4" in http://www.kimgentes.com/worshiptech-web-tools-page/2008/10/14/regex-pattern-for-parsing-csv-files-with-embedded-commas-dou.html
            var matches = Regex.Matches(line, @"\s?((?<x>(?=[;]+))|""(?<x>([^""]|"""")+)""|""(?<x>)""|(?<x>[^;]+));?",
                RegexOptions.ExplicitCapture);

            var values = (from Match m in matches
                select m.Groups["x"].Value.Trim().Replace("\"\"", "\"")).ToArray();

            return values;
        }

        // ******************************************************************
        private static string[] ParseLineNotSeparated(string line)
        {
            var lineValues = new string[1];
            lineValues[0] = line;
            return lineValues;
        }

        // ******************************************************************
        public static List<string[]> ParseText(string text)
        {
            var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.None);
            return ParseString(lines);
        }

        // ******************************************************************
        private static List<string[]> ParseString(string[] lines)
        {
            var result = new List<string[]>();

            var lineSeparator = LineSeparator.Unknown;
            if (lines.Any())
            {
                lineSeparator = GuessCsvSeparator(lines[0]);
            }

            var funcParse = DictionaryOfLineSeparatorAndItsFunc[lineSeparator];

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                result.Add(funcParse(line));
            }

            return result;
        }

        // ******************************************************************
    }
}