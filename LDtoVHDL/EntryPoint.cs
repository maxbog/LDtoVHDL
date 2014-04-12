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
			const string outputBaseDir = @"d:\dokumenty\copy\praca magisterska\silnik_vhdl\vhd\";

			var parser = new PlcOpenParser(XDocument.Load(File.OpenRead(@"d:\dokumenty\copy\praca magisterska\silnik\plc.xml")));

			var program = parser.Parse();
			var translator = new Translator();
			translator.Translate(program);
			var writer = new ProgramWriter(outputBaseDir);
			writer.WriteVhdlCode(program);
		}

	}
}
