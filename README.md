# NBasicBin2Text
Converter from N-BASIC binary saved file to ASCII text

## What's this?
　This program conberts Binary Saved Basic Program File from N-BASIC to ASCII Saved human readable format.

  You can use SAVE "filename",A to get ASCII source code, but you don't have any environment to run N-BASIC, you can use this tool. Or, if you want to "pretty" option or "extend space" option, also, this tool help you.

  "pretty" option extract multi-statement to mutiline to readbility.

  "extend space" option is insert space both of front and tail of reserved word. I's useful to implement other BASIC environments, which not allow the reserved words without space.

　このプログラムはNEC PC-8001のN-BASICからバイナリーセーブされたファイルを人間が読めるASCII形式に変換します。

　SAVE "filename",Aでも、ASCII形式のファイルは得られますが、N-BASICが動作する環境がない、または"pretty", "extend space"オプションを使うときはこのツールを利用できます。

　"pretty"オプションは読みやすさのためにマルチステートメントを複数行に展開します。

　"extend space"オプションは予約語の前後に空白文字を挿入します。これは予約語の前後に空白文字を必須とする環境にソースコードを移植するために活用できます。

## Requied Environment
  .NET Core 2.1

  (Windows, Mac or Linux supported by .NET Core 2.1)

## How to use?

dotnet NBasicBin2Text.dll [-p] [-e] FILENAME

　converted output will available from standard output

-p: "pretty" option
-e: "extend space" option

## Limitation



## License


## Created


