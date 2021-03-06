# TagMover
A simple commandline dotnet tool to copy files filtering on metadata.
Basically, when you need to copy some subset of your files based on their metadata/tags - that's it.

* Supporting both Windows and *nix systems(not tested).
* Supporting ID3v2, Xiph, Asf tags.
* Supporting mp3, wav, ogg, wma, etc.

## Install
1. Install dotnet runtime for your system - v.5.0 or higher. (https://dotnet.microsoft.com/download)
2. Unpack release to any folder.
3. Run TagMover.exe from command line.

## Comand line parameters
```
-s, --src     Required. Source folder.
-d, --dest    Required. Destination folder.
-f, --filter  Filter string. More info in readme.
-i, --include Include pattern for filepath - regexp format. Files will be copied only if they are passing both filter and pattern.
-e, --exclude Exclude pattern for filepath - regexp format. Files will be excluded even they are passing filter.
--help        Display this help screen.
--version     Display version information.
```

## Filter Syntax

The syntax for filter parameter is quite similar to foobar2000 or mp3Tag one as i'm using them as an example.

More details you could find here:

https://wiki.hydrogenaud.io/index.php?title=Foobar2000:Query_syntax

https://help.mp3tag.de/main_filter.html

Supported commands:
* AND / OR / () / NOT
* PRESENT / MISSING
* HAS / IS
* GREATER / EQUAL / LESS
* MATCHES

## Example
```
TagMover.exe -s C:\\Folder1 -d F:\\ -f "IGNOREAUTO MISSING" -i "\/(music|sound)\/.*" -e "\/(lyrics)\/.*"
```

## For developers
On changing Antlr grammar (.g4 file) it's needed to
1) Install Antlr4 according to official guide https://github.com/antlr/antlr4
2) Run getCSharp.bat (or copy & use commands from it in *Nix systems)
