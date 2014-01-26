using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.BlockFactories;
using LDtoVHDL.Blocks;
using Environment = LDtoVHDL.BlockFactories.Environment;

namespace LDtoVHDL
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var parser = new PlcOpenParser(XDocument.Load(File.OpenRead(@"d:\Dropbox\praca magisterska\test_ber\plc.xml")));

			var environment = parser.Parse();

			environment.IdentifyRails();
			environment.ReplaceCompositeSignalsWithOrs();
			environment.DivideBlocksIntoRungs();

			foreach (var rung in environment.Rungs)
			{
				Console.WriteLine("Rung {0}:", environment.Rungs.IndexOf(rung));
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
				foreach (var variable in rung.OutVariables)
					Console.WriteLine("Var: {0} Condition: {1}", variable.Item1.VariableName, variable.Item2);
				Console.WriteLine();
			}

			WriteVhdlCode(environment);

			Console.ReadKey();
		}

		private static void WriteVhdlCode(Environment env)
		{
			Console.WriteLine("VHDL CODE:");
			Console.WriteLine("entity vhdl_code is port();");
			Console.WriteLine("end vhdl_code;");
			Console.WriteLine("architecture behavioral of vhdl_code is");

			foreach (var signal in env.AllSignals)
				Console.WriteLine("    {0}", signal.VhdlDeclaration);

			Console.WriteLine("begin");

			foreach (var block in env.AllBlocks)
			{
				Console.WriteLine("    {0}", block.VhdlCode);
			}

			Console.WriteLine("end behavioral;");
		}
	}
}
