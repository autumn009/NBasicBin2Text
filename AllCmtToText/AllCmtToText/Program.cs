using System.Diagnostics;

if (args.Length != 1)
{
    Console.WriteLine("usage: AllCmdToText DIRECTORY");
    return;
}

foreach (var fullpath in Directory.EnumerateFiles(args[0],"*.cmt", SearchOption.AllDirectories))
{
    byte[] data = File.ReadAllBytes(fullpath);
    if (!data.Take(10).All(c => c == 0xd3))
    {
        Console.WriteLine($"Skipping: {fullpath}");
        continue;
    }
    Console.WriteLine($"Processing: {fullpath}");
    var outpath = Path.ChangeExtension(fullpath,"bas");
    var p = Process.Start("nbasicbin2text", $"\"{fullpath}\" \"{outpath}\"");
    p.WaitForExit();
}
Console.WriteLine("All Done");




