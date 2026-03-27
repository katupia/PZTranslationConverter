using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Convert
{
    internal class Ctxt2json
    {
        private static string[] languages = {
            "_AR",
            "_CA",
            "_CH",
            "_CN",
            "_CS",
            "_DA",
            "_DE",
            "_EN",
            "_ES",
            "_FI",
            "_FR",
            "_HU",
            "_ID",
            "_IT",
            "_JP",
            "_KO",
            "_NL",
            "_NO",
            "_PH",
            "_PL",
            "_PTBR",
            "_PT",
            "_RO",
            "_RU",
            "_TH",
            "_TR",
            "_UA"
        };

        private static string[] prefixePatterns = {
            "^(\\s*)ItemName_(.*)$",
            "^(\\s*)EvolvedRecipeName_(.*)$",
            "^(\\s*)Recipe_(.*)$"
        };

        private static string patternLine = "\\s*(\\S[^=^\\s]*)\\s*=\\s*\"(.*)\"";
        //TODO evaluate sdk ZedScript regex (beware, it can return initial match without value): KEY_VALUE_TRANSLATION_REGEX = /^(?!\s*[--])\s*(?<key>\S+[^=]*\S+)\s*=\s*(?<quote>"(?<value>[\S ]*)")?(?<comma>,?)/;
        public static string convertFileName(string path)
        {
            //& remove name language suffix (e.g. _EN)
            //& change file extension from .txt to .json
            string dir = Path.GetDirectoryName(path);
            string filename = Path.GetFileNameWithoutExtension(path);
            foreach (string lang in languages)
            {
                filename = Regex.Replace(filename, lang, "");
            }
            filename += ".json";
            return Path.Combine(dir, filename);
        }

        public static void translate(string path)
        {
            //write the json
            //& remove name language suffix (remove _EN)
            //& change file extension from .txt to .json
            //& change format to utf-8 (without BOM)
            //& replace
            //key_. Name = "<anything>",
            //with
            //"key_. Name": "<anything>",
            //& remove last,
            //& remove anything before first {
            //& remove ItemName_ prefix (& EvolvedRecipeName_, Recipe_)
            //& replace special character codes ?

            using StreamReader sr = File.OpenText(path);
            string outText = "{";
            bool firstDone = false;
            string s = "";
            var keys = new Dictionary<string, bool>();
            while ((s = sr.ReadLine()) != null)
            {
                MatchCollection matches = Regex.Matches(s, patternLine);
                foreach (Match match in matches)
                {
                    string key = match.Groups[1].Value;
                    foreach (string prefixePattern in prefixePatterns) 
                    {
                        Match prefixMatch = Regex.Match(key, prefixePattern);
                        if (prefixMatch.Success)
                        {
                            key = prefixMatch.Groups[1].Value + prefixMatch.Groups[2].Value;
                        }
                    }
                    if (!keys.ContainsKey(key))
                    {
                        keys.Add(key, true);
                        if (firstDone)
                        {
                            outText += ",";
                        }
                        string value = match.Groups[2].Value;
                        value = value.Replace("\"", "\\\"");
                        outText += "\n    \"" + key + "\": \"" + value + "\"";
                        firstDone = true;
                    }
                }
            }
            outText += "\n}";

            if (firstDone)
            {
                string outputPath = convertFileName(path);
                using FileStream jsonFile = File.Create(outputPath);
                UTF8Encoding encoding = new UTF8Encoding(false);//no BOM for PZ utf-8 of choice
                byte[] outBytes = encoding.GetBytes(outText);
                jsonFile.Write(outBytes, 0, outBytes.Length);
            }
        }
    }
}
