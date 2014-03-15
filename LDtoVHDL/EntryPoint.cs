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

			foreach (var rung in program.Rungs)
			{
				Console.WriteLine("Rung {0}:", program.Rungs.IndexOf(rung));
				Console.WriteLine("Blocks:");
				foreach (var block in rung.Blocks)
				{
					Console.WriteLine(block);
					foreach (var port in block.Ports.Values)
					{
						Console.WriteLine("    {0}", port);
						foreach (var otherSide in port.OtherSidePorts)
							Console.WriteLine("        --- {0}", otherSide.ParentBaseBlock);
					}
				}
				Console.WriteLine("WrittenVariables:");
				foreach (var variable in rung.Blocks.OfType<IOutVariableBlock>())
					Console.WriteLine("Var: {0} Condition: {1}", variable.VariableName, variable.WriteCondition);
				Console.WriteLine();
			}

			Console.WriteLine("VHDL CODE:");
			var writer = new ProgramWriter(Console.Out);
			writer.WriteVhdlCode(program);

			Console.ReadKey();
		}

	}
}
