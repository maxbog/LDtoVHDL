using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Model.Blocks;
using LDtoVHDL.Parsing;
using LDtoVHDL.Translation;
using LDtoVHDL.VhdlWriter;

namespace LDtoVHDL
{
	public class EntryPoint
	{
		public static void Main(string[] args)
		{
			var parser = new PlcOpenParser(XDocument.Load(File.OpenRead(@"d:\dokumenty\copy\praca magisterska\test_ber\plc.xml")));

			var program = parser.Parse();
			var translator = new Translator();
			translator.Translate(program);
			using (FileStream outputFile = File.Open(@"d:\dokumenty\copy\praca magisterska\test_ber\plc.vhd", FileMode.Create))
			using (var streamWriter = new StreamWriter(new BufferedStream(outputFile)))
			{
				var writer = new ProgramWriter(streamWriter);
				writer.WriteVhdlCode(program);
			}
		}

	}
}
