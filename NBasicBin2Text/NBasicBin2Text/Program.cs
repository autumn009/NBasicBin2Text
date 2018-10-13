﻿using System;
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
            return a.ToLower() == b.ToLower() ? 1 : 0;
        }

        private const int EOF = -1;

        private static int fgetc(FileStream stream)
        {
            return stream.ReadByte();
        }

        private static void putchar(int ch)
        {
            Console.Write((char)ch);
        }

        private static void usage()
        {
            Console.WriteLine("N-BASIC Binary to Text converter");
            Console.WriteLine("usage; NBasic2Text [-p] INPUTFILE >OUTPUTFILE");
        }

        static int Main(string[] args)
        {
            int argc = args.Length;
            string[] argv = args;

            if (argc != 1 && argc != 2)
            {
                usage();
                return 2;
            }

            bool bPretty = false;
            string filename = null;
            if (argc == 2)
            {
                if (stricmp(argv[0], "-p") == 0)
                {
                    bPretty = true;
                }
                else
                {
                    usage();
                    return 2;
                }
                filename = argv[1];
            }
            else
            {
                filename = argv[0];
            }

            using (var fp = File.OpenRead(filename))
            {
                while (true)
                {
                    // get line header
                    int linkLow = fgetc(fp);
                    if (linkLow == EOF)
                    {
                        Console.Error.WriteLine("Encounted unexpected EOF");
                        return 2;
                    }
                    int linkHigh = fgetc(fp);
                    if (linkLow == 0 && linkHigh == 0) break;

                    int lineNumberLow = fgetc(fp);
                    int lineNumberHigh = fgetc(fp);
                    int lineNumber = (lineNumberLow & 0xff) + ((lineNumberHigh & 0xff) << 8);
                    Console.Write("{0} ", lineNumber);

                    bool remOrDataMode = false;
                    bool quoteMode = false;
                    // process a line
                    while (true)
                    {
                        int ch = fgetc(fp);
                        if (ch == EOF)
                        {
                            Console.Error.WriteLine("Encounted unexpected EOF");
                            return 2;
                        }
                        ch = ch & 0xff; // must be unsigned value
                        if (ch == 0)
                        {   // end of line
                            Console.WriteLine();
                            break;
                        }
                        else if (ch == 0x0b)
                        {   // octal constatnt
                            int vl = fgetc(fp);
                            int vh = fgetc(fp);
                            int val = (vl & 0xff) + ((vh & 0xff) << 8);
                            Console.Write("&O");
                            Console.Write(Convert.ToString(val, 8));
                        }
                        else if (ch == 0x0c)
                        {   // hexa constant
                            int vl = fgetc(fp);
                            int vh = fgetc(fp);
                            int val = (vl & 0xff) + ((vh & 0xff) << 8);
                            Console.Write("&H");
                            Console.Write(Convert.ToString(val, 16));
                        }
                        else if (ch == 0x0d)
                        {   // absolute address replaced from line number
                            Console.Error.WriteLine("Warning: Encounted Absolute Address (0x0d)");
                            fgetc(fp);
                            fgetc(fp);
                        }
                        else if (ch == 0x0e)
                        {   // line number
                            int vl = fgetc(fp);
                            int vh = fgetc(fp);
                            int val = (vl & 0xff) + ((vh & 0xff) << 8);
                            Console.Write(val);
                        }
                        else if (ch == 0x0f)
                        {   // single byte constant
                            int val = (fgetc(fp) & 0xff);
                            Console.Write(val);
                        }
                        else if (ch >= 0x11 && ch <= 0x1a)
                        {   // one digit constant
                            int val = ch - 0x11;
                            Console.Write(val);
                        }
                        else if (ch == 0x1c)
                        {   // two byte integer
                            int vl = fgetc(fp);
                            int vh = fgetc(fp);
                            int val = (vl & 0xff) + ((vh & 0xff) << 8);
                            if ((val & 0x8000) != 0)
                            {
                                unchecked
                                {
                                    val |= (int)0xffff0000;
                                }
                            }
                            Console.Write(val);
                        }
                        else if (ch == 0x1d)
                        {   // four byte float
                            int v1 = fgetc(fp);
                            int v2 = fgetc(fp);
                            int v3 = fgetc(fp);
                            int v4 = fgetc(fp);
                            int kasu = (v1 & 0xff) + ((v2 & 0xff) << 8) + ((v3 & 0x7f) << 16);
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
                            Console.Write(r);
                        }
                        else if (ch == 0x1f)
                        {   // eight byte float
                            int v1 = fgetc(fp);
                            int v2 = fgetc(fp);
                            int v3 = fgetc(fp);
                            int v4 = fgetc(fp);
                            int v5 = fgetc(fp);
                            int v6 = fgetc(fp);
                            int v7 = fgetc(fp);
                            int v8 = fgetc(fp);
                            int kasu1 = (v1 & 0xff) + ((v2 & 0xff) << 8) + ((v3 & 0xff) << 16);
                            int kasu2 = (v4 & 0xff) + ((v5 & 0xff) << 8) + ((v6 & 0xff) << 16) + ((v7 & 0x7f) << 24);
                            int sisu = v8 - 0x81;
                            double r = 1;
                            double d = 0.5;
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
                            int mask2 = 0x40000000;
                            for ( int i = 0; i < 31; i++)
                            {
                                if ((kasu2 & mask2) != 0)
                                {
                                    r = r + d;
                                }
                                mask2 >>= 1;
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
                            Console.Write(r);
                        }
                        else if (ch >= 0x81 && ch <= 0xfe && quoteMode == false && remOrDataMode == false)
                        {
                            Console.Write(" {0} ",keywordsBase[ch - 0x81]);
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
                            int ch2 = (fgetc(fp) & 0xff);
                            if (ch2 >= 0x81 && ch2 <= 0xfd)
                            {
                                Console.Write(" {0} ", keywordsFF[ch2 - 0x81]);
                            }
                            else if (ch2 == 0xec)
                            {
                                Console.Write(" IEEE ");
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
                                    Console.WriteLine();
                                    Console.Write("\t");
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
                        else if (ch >= 0xa1 && ch <= 0xdf)
                        {
                            putchar(ch);
                        }
                        else
                        {
                            Console.Write("???[0x{X2}]???", ch);
                        }
                    }
                }
            }
            return 0;
        }
    }
}
