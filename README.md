# Purpose
Convert mods translation files from legacy txt format to B42.15+ json format.

# How To
## By drag and drop
You can drag and drop any file or directory on PZTranslationConverter.exe \
It behaves the same as by command line with target being the valid path of the dropped file or directory

## By commande line
PZTranslationConverter.exe [Target1] [Target2]  [TargetN] \
There can be any number of target \
Target can be a file path or a directory path

### Valid file path as Target
If the file is a txt, with at least one recognized translation pattern, a json will be created.

### Valid directory path as Target
Any txt file recursively found in the directory, with at least one recognized translation pattern, is converted.

## By double click
It behaves the same by command line with target being the executable directory path

## Translation Pattern used
string patternLine = "\\s*(\\S[^=^\\s]*)\\s*=\\s*\"(.*)\"";
