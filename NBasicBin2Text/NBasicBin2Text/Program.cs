using System;
using System.IO;

namespace NBasicBin2Text
{
    class Program
    {
        private static string[] keywordsBase = new string[126]
        {
    "END",
    "FOR",
    "NEXT",
    "DATA",
    "INPUT",
    "DIM",
    "READ",
    "LET",
    "GOTO",
    "RUN",
    "IF",
    "RESTORE",
    "GOSUB",
    "RETURN",
    "REM",

    "STOP",
    "PRINT",
    "CLEAR",
    "LIST",
    "NEW",
    "ON",
    "WAIT",
    "DEF",
    "POKE",
    "CONT",
    "CSAVE",
    "CLOAD",
    "OUT",
    "LPRINT",
    "LLIST",
    "CONSOLE",

    "WIDTH",
    "ELSE",
    "TRON",
    "TROFF",
    "SWAP",
    "ERASE",
    "ERROR",
    "RESUME",
    "DELETE",
    "AUTO",
    "RENUM",
    "DEFSTR",
    "DEFINT",
    "DEFSNG",
    "DEFDBL",
    "LINE",

    "PRESET",
    "PSET",
    "BEEP",
    "FORMAT",
    "KEY",
    "COLOR",
    "TERM",
    "MON",
    "CMD",
    "MOTOR",
    "POLL",
    "RBYTE",
    "WBYTE",
    "ISET",
    "IRESET",
    "TALK",

    "MAT",
    "LISTEN",
    "DSKO$",
    "REMOVE",
    "MOUNT",
    "OPEN",
    "FIELD",
    "GET",
    "PUT",
    "SET",
    "CLOSE",
    "LOAD",
    "MERGE",
    "FILES",
    "NAME",
    "KILL",

    "LSET",
    "RSET",
    "SAVE",
    "LFILES",
    "INIT",
    "LOCATE",
    "???[0xd6]???",
    "TO",
    "THEN",
    "TAB(",
    "STEP",
    "USR",
    "FN",
    "SPC(",
    "NOT",
    "ERL",

    "ERR",
    "STRING$",
    "USING",
    "INSTR",
    ",",
    "VARPTR",
    "CSRLIN",
    "ATTR$",
    "DSKI$",
    "INKEY$",
    "TIME$",
    "DATE$",
    "???[0xec]???",
    "SQR",
    "STATUS",
    "POINT",

    ">",
    "=",
    "<",
    "+",
    "-",
    "*",
    "/",
    "^",
    "AND",
    "OR",
    "XOR",
    "EQV",
    "IMP",
    "MOD",
    "\\"
};

        private static string[] keywordsFF = new string[45]
        {
    "LEFT$",
    "RIGHT$",
    "MID$",
    "SGN",
    "INT",
    "ABS",
    "SQR",
    "RND",
    "SIN",
    "LOG",
    "EXP",
    "COS",
    "TAN",
    "ATN",
    "FRE",
    "INP",
    "POS",
    "LEN",
    "STR$",
    "VAL",
    "ASC",
    "CHR$",
    "PEEK",
    "SPACE$",
    "OCT$",
    "HEX$",
    "LPOS",
    "PORT",
    "DEC",
    "BCD$",
    "CINT",
    "CSNG",
    "CDBL",
    "FIX",
    "CVI",
    "CVS",
    "CVD",
    "DSKF",
    "EOF",
    "LOC",
    "LOF",
    "FPOS",
    "MKI$",
    "MKS$",
    "MKD$"
};
        private static int stricmp(string a, string b)
        {
            return a.ToLower() == b.ToLower() ? 0 : 1;
        }

        private const int EOF = -1;

        private static int fgetc(FileStream stream)
        {
            return stream.ReadByte();
        }

        private static void usage()
        {
            Console.WriteLine("N-BASIC Binary to Text converter Version 2.0");
            Console.WriteLine("usage: dotnet NBasic2Text [-p] [-e] [-g] [-l] INPUTFILE [OUTPUTFILE]");
        }

        static int Main(string[] args)
        {
            int argc = args.Length;
            string[] argv = args;

            bool bPretty = false;
            bool bGraphWarn = false;
            bool bLastEOF = false;
            string extraSpace = "";
            string srcFileName = null;
            string dstFileName = null;
            foreach (var item in args)
            {
                if (stricmp(item, "-p") == 0) bPretty = true;
                else if (stricmp(item, "-e") == 0) extraSpace = " ";
                else if (stricmp(item, "-g") == 0) bGraphWarn = true;
                else if (stricmp(item, "-l") == 0) bLastEOF = true;
                else if (srcFileName == null) srcFileName = item;
                else dstFileName = item;
            }
            if( srcFileName == null)
            {
                usage();
                return 2;
            }

            FileStream dstFile = null;
            using (var srcFile = File.OpenRead(srcFileName))
            {
                if (dstFileName != null) dstFile = File.Create(dstFileName);
                try
                {

                    while (true)
                    {
                        // get line header
                        int linkLow = fgetc(srcFile);
                        if (linkLow == EOF)
                        {
                            Console.Error.WriteLine("Encounted unexpected EOF");
                            return 2;
                        }
                        int linkHigh = fgetc(srcFile);
                        if (linkLow == 0 && linkHigh == 0) break;

                        int lineNumberLow = fgetc(srcFile);
                        int lineNumberHigh = fgetc(srcFile);
                        int lineNumber = (lineNumberLow & 0xff) + ((lineNumberHigh & 0xff) << 8);
                        putstr(string.Format("{0} ", lineNumber));

                        bool remOrDataMode = false;
                        bool quoteMode = false;
                        // process a line
                        while (true)
                        {
                            int ch = fgetc(srcFile);
                            if (ch == EOF)
                            {
                                Console.Error.WriteLine("Encounted unexpected EOF");
                                return 2;
                            }
                            ch = ch & 0xff; // must be unsigned value
                            if (ch == 0)
                            {   // end of line
                                putnl();
                                break;
                            }
                            else if (ch == 0x0b)
                            {   // octal constatnt
                                int vl = fgetc(srcFile);
                                int vh = fgetc(srcFile);
                                int val = (vl & 0xff) + ((vh & 0xff) << 8);
                                putstr("&O");
                                putstr(Convert.ToString(val, 8));
                            }
                            else if (ch == 0x0c)
                            {   // hexa constant
                                int vl = fgetc(srcFile);
                                int vh = fgetc(srcFile);
                                int val = (vl & 0xff) + ((vh & 0xff) << 8);
                                putstr("&H");
                                putstr(Convert.ToString(val, 16));
                            }
                            else if (ch == 0x0d)
                            {   // absolute address replaced from line number
                                Console.Error.WriteLine("Warning: Encounted Absolute Address (0x0d)");
                                fgetc(srcFile);
                                fgetc(srcFile);
                            }
                            else if (ch == 0x0e)
                            {   // line number
                                int vl = fgetc(srcFile);
                                int vh = fgetc(srcFile);
                                int val = (vl & 0xff) + ((vh & 0xff) << 8);
                                putstr(val);
                            }
                            else if (ch == 0x0f)
                            {   // single byte constant
                                int val = (fgetc(srcFile) & 0xff);
                                putstr(val);
                            }
                            else if (ch >= 0x11 && ch <= 0x1a)
                            {   // one digit constant
                                int val = ch - 0x11;
                                putstr(val);
                            }
                            else if (ch == 0x1c)
                            {   // two byte integer
                                int vl = fgetc(srcFile);
                                int vh = fgetc(srcFile);
                                int val = (vl & 0xff) + ((vh & 0xff) << 8);
                                if ((val & 0x8000) != 0)
                                {
                                    unchecked
                                    {
                                        val |= (int)0xffff0000;
                                    }
                                }
                                putstr(val);
                            }
                            else if (ch == 0x1d)
                            {   // four byte float
                                int v1 = fgetc(srcFile);
                                int v2 = fgetc(srcFile);
                                int v3 = fgetc(srcFile);
                                int v4 = fgetc(srcFile);
                                int kasu = (v1 & 0xff) + ((v2 & 0xff) << 8) + ((v3 & 0x7f) << 16);
                                System.Diagnostics.Debug.Assert((v3 & 0x80) == 0);
                                int sisu = v4 - 0x81;
                                float r = 1;
                                float d = 0.5f;
                                int mask = 0x400000;
                                for (int i = 0; i < 23; i++)
                                {
                                    if ((kasu & mask) != 0)
                                    {
                                        r = r + d;
                                    }
                                    mask >>= 1;
                                    d /= 2.0f;
                                }
                                if (sisu == 0)
                                {
                                    r = (float)0.0;
                                }
                                if (sisu < 0)
                                {
                                    for (int i = 0; i < Math.Abs(sisu); i++)
                                    {
                                        r /= 2.0f;
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < sisu; i++)
                                    {
                                        r *= 2.0f;
                                    }
                                }
                                putstr(r);
                            }
                            else if (ch == 0x1f)
                            {   // eight byte float
                                int v1 = fgetc(srcFile);
                                int v2 = fgetc(srcFile);
                                int v3 = fgetc(srcFile);
                                int v4 = fgetc(srcFile);
                                int v5 = fgetc(srcFile);
                                int v6 = fgetc(srcFile);
                                int v7 = fgetc(srcFile);
                                int v8 = fgetc(srcFile);
                                int kasu1 = (v1 & 0xff) + ((v2 & 0xff) << 8) + ((v3 & 0xff) << 16);
                                int kasu2 = (v4 & 0xff) + ((v5 & 0xff) << 8) + ((v6 & 0xff) << 16) + ((v7 & 0x7f) << 24);
                                System.Diagnostics.Debug.Assert((v7 & 0x80) == 0);
                                int sisu = v8 - 0x81;
                                double r = 1;
                                double d = 0.5;
                                int mask2 = 0x40000000;
                                for (int i = 0; i < 31; i++)
                                {
                                    if ((kasu2 & mask2) != 0)
                                    {
                                        r = r + d;
                                    }
                                    mask2 >>= 1;
                                    d /= 2.0;
                                }
                                int mask1 = 0x800000;
                                for (int i = 0; i < 24; i++)
                                {
                                    if ((kasu1 & mask1) != 0)
                                    {
                                        r = r + d;
                                    }
                                    mask1 >>= 1;
                                    d /= 2.0;
                                }
                                if (sisu == 0)
                                {
                                    r = 0.0;
                                }
                                if (sisu < 0)
                                {
                                    for (int i = 0; i < Math.Abs(sisu); i++)
                                    {
                                        r /= 2.0;
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < sisu; i++)
                                    {
                                        r *= 2.0;
                                    }
                                }
                                var s = r.ToString();
                                if (s.Contains("E")) s = s.Replace("E", "D");
                                else s = s + "#";
                                putstr(s);
                            }
                            else if (ch >= 0x81 && ch <= 0xfe && quoteMode == false && remOrDataMode == false)
                            {
                                putstr(string.Format(extraSpace + "{0}" + extraSpace, keywordsBase[ch - 0x81]));
                                if (ch == 0x8f)
                                {   // REM
                                    remOrDataMode = true;
                                }
                                if (ch == 0x84)
                                {   // DATA
                                    remOrDataMode = true;
                                }
                            }
                            else if (ch == 0xff && quoteMode == false && remOrDataMode == false)
                            {
                                int ch2 = (fgetc(srcFile) & 0xff);
                                int ch2Munus81 = ch2 - 0x81;
                                if (ch2Munus81 >= 0 && ch2Munus81 < keywordsFF.Length)
                                {
                                    putstr(string.Format(extraSpace + "{0}" + extraSpace, keywordsFF[ch2Munus81]));
                                }
                                else if (ch2 == 0xec)
                                {
                                    putstr(string.Format(extraSpace + "IEEE" + extraSpace));
                                }
                                else
                                {
                                    putstr(string.Format("???[0x{0:X2}][0x{1:X2}]???", ch, ch2));
                                }
                            }
                            else if (ch >= 0x20 && ch <= 0x7e)
                            {
                                if (ch == '"')
                                {
                                    if (quoteMode)
                                    {
                                        quoteMode = false;
                                    }
                                    else
                                    {
                                        quoteMode = true;
                                    }
                                    putchar(ch);
                                }
                                else if (quoteMode == false && ch == ':')
                                {
                                    remOrDataMode = false;
                                    if (bPretty)
                                    {
                                        putnl();
                                        putstr("\t");
                                    }
                                    else
                                    {
                                        putchar(ch);
                                    }
                                }
                                else
                                {
                                    putchar(ch);
                                }
                            }
                            else if (bGraphWarn && (ch < 0xa1 || ch > 0xdf))
                                putstr(string.Format("???[0x{0:X2}]???", ch));
                            else
                                putchar(ch);
                        }
                    }
                }
                finally
                {
                    if (bLastEOF) putchar(0x1a);
                    if (dstFile != null) dstFile.Dispose();
                }
            }
            return 0;

            void putchar(int ch)
            {
                if (dstFile == null)
                    Console.Write((char)ch);
                else
                    dstFile.WriteByte((byte)ch);
            }
            void putstr(object s)
            {
                foreach (var item in s.ToString()) putchar(item);
            }
            void putnl()
            {
                if (dstFile == null)
                    Console.WriteLine();
                else
                    putstr("\r\n");
            }
        }
    }
}
