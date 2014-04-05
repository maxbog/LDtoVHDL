using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;
using LDtoVHDL.TypeFinder;

namespace LDtoVHDL.VhdlWriter
{
	public class ProgramWriter
	{
		private readonly ObjectDictionary<Type, BaseBlockWriter, WriterForAttribute> m_blockWriters;
		private readonly DirectoryInfo m_baseDirectory;

		public ProgramWriter(DirectoryInfo baseDirectory)
		{
			m_baseDirectory = baseDirectory;
			m_blockWriters = ObjectDictionary<Type, BaseBlockWriter, WriterForAttribute>.FromExecutingAssembly(type => type.BaseType, ffa => ffa.FormattedType);
		}

		public void WriteVhdlCode(Program env)
		{
			var mainFilePath = Path.Combine(m_baseDirectory.FullName, env.ProgramName + ".vhd");
			using (var file = File.Open(mainFilePath, FileMode.Create))
			using (var writer = new StreamWriter(file))
			{
				WriteEntityDeclaration(writer, env);
				WriteArchitectureDefinition(writer, env);
			}
		}

		private void WriteArchitectureDefinition(TextWriter writer, Program env)
		{
			writer.WriteLine("architecture behavioral of vhdl_code is");

			WriteComponentReferences(writer, env);
			WriteSignalDeclarations(writer, env);
			WriteBlockDelcarations(writer, env);

			writer.WriteLine("begin");

			WriteBlocksCode(writer, env);

			writer.WriteLine("end behavioral;");
		}

		private void WriteComponentReferences(TextWriter writer, Program env)
		{
			var references = new HashSet<string>();
			foreach (var block in env.AllBlocks)
			{
				var reference = m_blockWriters.Get(block.GetType()).GetComponentReference(block);
				if (reference == null)
					continue;
				if (references.Contains(reference)) 
					continue;
				writer.WriteLine(reference);
				references.Add(reference);
			}
		}

		private void WriteBlocksCode(TextWriter writer, Program env)
		{
			foreach (var block in env.AllBlocks)
			{
				var blockWriter = m_blockWriters.Get(block.GetType());
				if (blockWriter != null)
					blockWriter.WriteCode(writer, block);
			}
		}

		private void WriteBlockDelcarations(TextWriter writer, Program env)
		{
			foreach (var block in env.AllBlocks)
			{
				var blockWriter = m_blockWriters.Get(block.GetType());
				if(blockWriter != null)
					blockWriter.WriteDeclaration(writer, block);
			}
		}

		private void WriteSignalDeclarations(TextWriter writer, Program env)
		{
			foreach (var signal in env.AllSignals)
				writer.WriteLine("    signal {0} : {1};", GetSignalName(signal), SignalTypeWriter.GetName(signal.Type));
		}

		private void WriteEntityDeclaration(TextWriter writer, Program env)
		{
			writer.WriteLine("entity vhdl_code is port(");
			WritePortMappings(writer, env);
			writer.WriteLine(");");
			writer.WriteLine("end vhdl_code;");
		}

		private void WritePortMappings(TextWriter writer, Program env)
		{
			var inPortsSpec = string.Join(";\n", env.AllBlocks
				.OfType<InputVariableStorageBlock>()
				.Select(outVar => string.Format("    {0} : in {1}", outVar.VariableName, SignalTypeWriter.GetName(outVar.Output.SignalType))));

			var outPortsSpec = string.Join(";\n", env.AllBlocks
				.OfType<OutputVariableStorageBlock>()
				.Select(outVar => string.Format("    {0} : out {1}", outVar.VariableName, SignalTypeWriter.GetName(outVar.Output.SignalType))));

			if (inPortsSpec.Length > 0)
				writer.Write(inPortsSpec + (outPortsSpec.Length > 0 ? ";\n" : ""));
			if (outPortsSpec.Length > 0)
				writer.Write(outPortsSpec);

			if (env.AllBlocks.OfType<ClockBlock>().Any())
				writer.Write(";\n    CLK : in std_logic");

			writer.WriteLine();
		}

		public static string GetSignalName(Signal signal)
		{
			return string.Format("signal_{0}", signal.Hash);
		}
	}
}