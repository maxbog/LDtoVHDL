using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;
using LDtoVHDL.TypeFinder;

namespace LDtoVHDL.VhdlWriter
{
	public class ProgramWriter
	{
		private readonly ObjectDictionary<Type, BaseBlockWriter, WriterForAttribute> m_blockWriters;
		private readonly string m_baseDirectory;
		private readonly TemplateResolver m_templateResolver;

		public ProgramWriter(string baseDirectory)
		{
			m_baseDirectory = baseDirectory;
			var startingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Debug.Assert(startingDir != null, "startingDir != null");
			m_templateResolver = new TemplateResolver(Path.Combine(startingDir, "Templates"));

			m_blockWriters = ObjectDictionary<Type, BaseBlockWriter, WriterForAttribute>.FromExecutingAssembly(type => type.BaseType, ffa => ffa.FormattedType, new object[] { m_templateResolver });
		}
		
		public TextWriter GetWriterForFile(string fileName)
		{
			var mainFilePath = Path.Combine(m_baseDirectory, fileName);
			return new StreamWriter(File.Open(mainFilePath, FileMode.Create));
		}

		public void WriteVhdlCode(Program program)
		{
			using (var writer = GetWriterForFile(program.Name + ".vhd"))
			{
				WriteEntityDeclaration(writer, program);
				WriteArchitectureDefinition(writer, program);
			}

			using (var writer = GetWriterForFile("types.vhd"))
			{
				writer.WriteLine(m_templateResolver.GetWithReplacements("types.vhd"));
			}

			WriteBlockDefinitions(program);
		}

		private void WriteArchitectureDefinition(TextWriter writer, Program program)
		{
			writer.WriteLine("architecture behavioral of {0} is", program.Name);

			WriteComponentReferences(writer, program);
			WriteSignalDeclarations(writer, program);
			WriteBlockDelcarations(writer, program);

			writer.WriteLine("begin");

			WriteBlocksCode(writer, program);

			writer.WriteLine("end behavioral;");
		}

		private void WriteComponentReferences(TextWriter writer, Program program)
		{
			var references = new HashSet<string>();
			foreach (var block in program.AllBlocks)
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

		private void WriteBlockDefinitions(Program program)
		{
			var definitions = new HashSet<string>();
			foreach (var block in program.AllBlocks)
			{
				var blockWriter = m_blockWriters.Get(block.GetType());
				var vhdlType = blockWriter.GetVhdlType(block);
				if (vhdlType == null)
					continue;
				if (definitions.Contains(vhdlType))
					continue;
				definitions.Add(vhdlType);

				var definition = blockWriter.GetDefinition(block);
				if (definition == null) 
					continue;

				using (var writer = GetWriterForFile(vhdlType + ".vhd"))
				{
					writer.WriteLine(definition);
				}
			}
		}

		private void WriteBlocksCode(TextWriter writer, Program program)
		{
			foreach (var block in program.AllBlocks)
			{
				var blockWriter = m_blockWriters.Get(block.GetType());
				if (blockWriter != null)
					blockWriter.WriteCode(writer, block);
			}
		}

		private void WriteBlockDelcarations(TextWriter writer, Program program)
		{
			foreach (var block in program.AllBlocks)
			{
				var blockWriter = m_blockWriters.Get(block.GetType());
				if(blockWriter != null)
					blockWriter.WriteDeclaration(writer, block);
			}
		}

		private void WriteSignalDeclarations(TextWriter writer, Program program)
		{
			foreach (var signal in program.AllSignals)
				writer.WriteLine("    signal {0} : {1};", GetSignalName(signal), SignalTypeWriter.GetName(signal.Type));
		}

		private void WriteEntityDeclaration(TextWriter writer, Program program)
		{
			writer.WriteLine("entity {0} is port(", program.Name);
			WritePortMappings(writer, program);
			writer.WriteLine(");");
			writer.WriteLine("end {0};", program.Name);
		}

		private void WritePortMappings(TextWriter writer, Program program)
		{
			var inPortsSpec = string.Join(";\n", program.AllBlocks
				.OfType<InputVariableStorageBlock>()
				.Select(outVar => string.Format("    {0} : in {1}", outVar.VariableName, SignalTypeWriter.GetName(outVar.Output.SignalType))));

			var outPortsSpec = string.Join(";\n", program.AllBlocks
				.OfType<OutputVariableStorageBlock>()
				.Select(outVar => string.Format("    {0} : out {1}", outVar.VariableName, SignalTypeWriter.GetName(outVar.Output.SignalType))));

			if (inPortsSpec.Length > 0)
				writer.Write(inPortsSpec + (outPortsSpec.Length > 0 ? ";\n" : ""));
			if (outPortsSpec.Length > 0)
				writer.Write(outPortsSpec);

			if (program.AllBlocks.OfType<ClockBlock>().Any())
				writer.Write(";\n    CLK : in std_logic");

			writer.WriteLine();
		}

		public static string GetSignalName(Signal signal)
		{
			return string.Format("signal_{0}", signal.Hash);
		}
	}
}