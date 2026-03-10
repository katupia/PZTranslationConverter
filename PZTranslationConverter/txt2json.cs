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
            "_PT",
            "_PTBR",
            "_RO",
            "_RU",
            "_TH",
            "_TR",
            "_UA"
        };
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
            //key_. Name = "content_%-09*$^",
            //with
            //"key_. Name": "content_%-09*$^",
            //& remove last,
            //& remove anything before first {
            //& replace special character codes ?
            // Open the stream and read it back.
            using StreamReader sr = File.OpenText(path);
            string patternLine = "\\s*(\\S[^=^\\s]*)\\s*=\\s*\"(.*)\"";
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
                    key = key.Replace("ItemName_", "");
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
                UTF8Encoding encoding = new(false);//no BOM for PZ utf-8 of choice
                byte[] outBytes = encoding.GetBytes(outText);
                jsonFile.Write(outBytes, 0, outBytes.Length);
            }
        }
    }
}
