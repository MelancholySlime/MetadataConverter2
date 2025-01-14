﻿using MetadataConverter2.Managers;
using MetadataConverter2.MetaTypes;
using System.Reflection;

if (args.Length is not 3 and not 4)
{
    Console.WriteLine(GetHelpMessage());
    return;
}

try
{
    bool convert = false;
    FileInfo inputPath = new(args[0]);
    FileInfo outputPath = new(args[1]);

    if (!Enum.TryParse(args[2], true, out MetaType gameType))
    {
        throw new Exception($"Invalid Meta\n" + MetaManager.SupportedMetas());
    }

    if (args.Length == 4)
    {
        convert = args[3] == "convert";
    }

    Console.WriteLine($"Processing...");
    byte[] metadataBytes = File.ReadAllBytes(inputPath.FullName);

    using MemoryStream metadataStream = new(metadataBytes);
    MetaBase game = MetaManager.GetMeta(gameType);
    game.Decrypt(metadataStream);
    if (convert)
    {
        Console.WriteLine($"Converting...");
        game.Convert(metadataStream);
    }

    Console.WriteLine($"Writing...");
    File.WriteAllBytes(outputPath.FullName, metadataStream.ToArray());

    Console.WriteLine("Done");
}
catch (Exception e)
{
    Console.WriteLine(e);
}

static string GetHelpMessage()
{
    string VersionString = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    string HelpMsg = $@"MetadataConverter2 v{VersionString}
------------------------
Usage:
  MetadataConverter2 <input_path> <output_path> <game> [convert]

Arguments:
  <input_path>  Input metadata file.
  <output_path> Output metadata file.
  <game>        ({string.Join('|', Enum.GetNames(typeof(MetaType)))})
  [convert]     enable converting to standard unity format.
";
    return HelpMsg;
}