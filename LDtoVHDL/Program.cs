using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var parser = new PlcOpenParser(XDocument.Load(File.OpenRead(@"d:\dokumenty\copy\praca magisterska\test_ber\plc.xml")));

			var environment = parser.Parse();
			var compiler = new Compiler();
			compiler.Transform(environment);

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
				foreach (var variable in rung.Blocks.OfType<IOutVariableBlock>())
					Console.WriteLine("Var: {0} Condition: {1}", variable.VariableName, variable.WriteCondition);
				Console.WriteLine();
			}

			WriteVhdlCode(environment);

			Console.ReadKey();
		}

		private static void WriteVhdlCode(Environment env)
		{
			Console.WriteLine("VHDL CODE:");
			Console.WriteLine("entity vhdl_code is port(");
			var inPortsSpec = string.Join(";\n", env.AllBlocks
				.OfType<InputVariable>()
				.Select(outVar => string.Format("    {0} : in {1}", outVar.VariableName, outVar.Output.SignalType.VhdlName)));
			var outPortsSpec = string.Join(";\n", env.AllBlocks
				.OfType<OutputVariable>()
				.Select(outVar => string.Format("    {0} : out {1}", outVar.VariableName, outVar.Output.SignalType.VhdlName)));
			Console.Write(inPortsSpec);
			if (inPortsSpec.Length > 0)
				Console.Write(";\n");
			Console.Write(outPortsSpec);
			Console.WriteLine(");");
			Console.WriteLine("end vhdl_code;");
			Console.WriteLine("architecture behavioral of vhdl_code is");
			
			foreach (var signal in env.AllSignals)
				Console.WriteLine("    {0}", signal.VhdlDeclaration);
			foreach (var block in env.AllBlocks.Where(blk => blk.VhdlDeclaration != null))
				Console.WriteLine("    {0}", block.VhdlDeclaration);

			Console.WriteLine("begin");

			foreach (var block in env.AllBlocks)
				Console.WriteLine("    {0}", block.VhdlCode);
			foreach (var outputVariable in env.AllBlocks.OfType<OutputVariable>())
				Console.WriteLine("    {0} <= {1};", outputVariable.VariableName, outputVariable.Output.ConnectedSignal.VhdlName);

			Console.WriteLine("end behavioral;");
		}
	}
}
