using System;
using System.IO;
using System.Xml.Linq;
using LDtoVHDL.Parsing;
using LDtoVHDL.Translation;
using LDtoVHDL.VhdlWriter;
using NDesk.Options;

namespace LDtoVHDL
{
	public class EntryPoint
	{
		public static void Main(string[] args)
		{

			string inputFile = null;
			string outputBaseDir = null;
			var optionsParser = new OptionSet
			{
				{"i|input=", "Path to plc.xml file", v => inputFile = v},
				{"o|output=", "Base output directory, where vhd files will be stored", v => outputBaseDir = v}
			};

			try
			{
				optionsParser.Parse(args);
			}
			catch (OptionException e)
			{
				Console.Write("LdToVHDL: ");
				Console.WriteLine(e.Message);
				Console.WriteLine("Try `LdToVHDL --help' for more information.");
				return;
			}

			if (inputFile == null || outputBaseDir == null)
			{
				Console.WriteLine("ERROR: Both input file and base output dir options are required");
				return;
			}

			var parser = new PlcOpenParser(XDocument.Load(File.OpenRead(inputFile)));

			var program = parser.Parse();
			var translator = new Translator();
			translator.Translate(program);
			var writer = new ProgramWriter(outputBaseDir);
			writer.WriteVhdlCode(program);
		}

	}
}
