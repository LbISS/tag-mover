# TagMover
A simple commandline dotnet tool to copy files filtering on metadata.

* Supporting both Windows/*nix systems.
* Supporting ID3v2 tags.

## Comand line parameters
```
-s, --src       Required. Source folder.
-d, --dest    Required. Destination folder.
-f, --filter    Filter string. More info in readme.
--help         Display this help screen.
--version     Display version information.
```

## Filter Syntax

The syntax for filter parameter is quite similar to foobar2000 or mp3Tag one as i'm using them as an example.

More details you could find here:
https://wiki.hydrogenaud.io/index.php?title=Foobar2000:Query_syntax
https://help.mp3tag.de/main_filter.html

Supported commands:
* PRESENT / MISSING - for ID3v2 custom metadata fields (TXXX frame)

## Example
```
TagMover.exe -s C:\\Folder1 -d F:\\ -f "IGNOREAUTO MISSING"
```
