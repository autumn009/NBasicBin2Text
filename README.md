# NBasicBin2Text
Converter from N-BASIC binary saved file to ASCII text

バイナリー保存されたN-BASICプログラムのファイルからテキスト形式に変換します

## What's this?
　This program converts Binary Saved Basic Program File from N-BASIC to ASCII Saved human readable format.

  You can use SAVE "filename",A to get ASCII source code, but you don't have any environment to run N-BASIC, or if you want to some options to get source codes, you can use this tool. 

　このプログラムはNEC PC-8001のN-BASICからバイナリーセーブされたファイルを人間が読めるASCII形式に変換します。

　SAVE "filename",Aでも、ASCII形式のファイルは得られますが、N-BASICが動作する環境がない、または"pretty", "extend space"オプションを使うときはこのツールを利用できます。

## Requied Environment
  .NET Core 6.0

  (Windows, Mac or Linux supported by .NET Core 6.0)

  (.NET Core 6.0でサポートされたWindows, Mac, Linux)

## How to use?

dotnet NBasic2Text [-p] [-e] [-g] [-l] INPUTFILE [OUTPUTFILE]

-p: "pretty" option

-e: "extend space" option

-g: "grph" option

-l: "eof" option

  "pretty" option extract multi-statement to mutiline to readbility.

  "extend space" option is insert space both of front and tail of reserved word. I's useful to implement other BASIC environments, which not allow the reserved words without space.

  "grph" option converts graphics character into "???[0xXX]???" style warning text.

  "eof" option adds EOF(0x1A) into end of the output file.

　"pretty"オプションは読みやすさのためにマルチステートメントを複数行に展開します。

　"extend space"オプションは予約語の前後に空白文字を挿入します。これは予約語の前後に空白文字を必須とする環境にソースコードを移植するために活用できます。

　"grph"オプションはグラフィックキャラクターを"???[0xXX]???"という書式の警告テキストに置き換えます。

　"eof"オプションは出力ファイルの末尾にEOF(0x1A)を追加します。

　入力ファイルの拡張子がCMTの場合、CMT形式と見なしてヘッダー+ファイル名の16バイトを読み飛ばします。

## Limitation

  Integet, single, double numbers replesentation may incompatible due to incompatibity between N-BASIC and .NET Core.

  If you use standard output to get result, some character will lost or converted.  (Recommentation is specifing output filename)

  整数、単精度実数、倍精度実数の表記には互換性がないかもしれません。なぜなら、N-BASICと.NET Coreの間で表記に差が生じるからです。


　もし、標準出力に出力した場合、一部の文字が欠落したり変化してしまう可能性があります。(出力ファイル名を指定することを推奨します)

## License
  This program is distributed under MIT license.

## Created
  This program created by Akira Kawamata.

あなたの知らないN-BASICの真実: PC-8001から98DO+まで

https://www.amazon.co.jp/dp/B07GDBVQ7Q

アタマとカラダで理解する！コンピュータプログラムはどのように動くのか？　技術評論社　ISBN978-4-7741-9499-8

http://gihyo.jp/book/2018/978-4-7741-9499-8

T88Extoracotor: Extract T88 File for PC-8801 Tape Data

https://github.com/autumn009/T88Extractor

EE8080: i8080 & CP/M emulation on the Web

http://ee8080.azurewebsites.net/


end.
