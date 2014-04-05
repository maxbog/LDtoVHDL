using System.IO;
using System.Xml.Linq;
using LDtoVHDL.Parsing;
using LDtoVHDL.Translation;
using LDtoVHDL.VhdlWriter;

namespace LDtoVHDL
{
	public class EntryPoint
	{
		public static void Main(string[] args)
		{
			const string outputBaseDir = @"d:\dokumenty\copy\praca magisterska\vhdl_output\vhd\";

			var parser = new PlcOpenParser(XDocument.Load(File.OpenRead(@"d:\dokumenty\copy\praca magisterska\test_ber\plc.xml")));

			var program = parser.Parse();
			var translator = new Translator();
			translator.Translate(program);
			var baseDir = Directory.CreateDirectory(outputBaseDir);
			var writer = new ProgramWriter(baseDir);
			writer.WriteVhdlCode(program);
		}

	}
}
