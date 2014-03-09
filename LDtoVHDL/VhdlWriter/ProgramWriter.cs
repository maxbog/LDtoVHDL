using System;
using System.IO;
using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	public class ProgramWriter
	{
		private readonly TextWriter m_writer;

		private readonly ObjectDictionary<Type, BaseBlockWriter, WriterForAttribute> m_blockWriters;
		private readonly ObjectDictionary<Type, SignalTypeWriter, WriterForAttribute> m_signalTypeWriters;

		public ProgramWriter(TextWriter writer)
		{
			m_writer = writer;
			m_blockWriters = ObjectDictionary<Type, BaseBlockWriter, WriterForAttribute>.FromExecutingAssembly(type => type.BaseType, ffa => ffa.FormattedType, new object[] { m_writer });
			m_signalTypeWriters = ObjectDictionary<Type, SignalTypeWriter, WriterForAttribute>.FromExecutingAssembly(type => type.BaseType, ffa => ffa.FormattedType, new object[] { m_writer });
		}

		public void WriteVhdlCode(Environment env)
		{
			WriteEntityDeclaration(env);
			WriteArchitectureDefinition(env);
		}

		private void WriteArchitectureDefinition(Environment env)
		{
			m_writer.WriteLine("architecture behavioral of vhdl_code is");

			WriteSignalDeclarations(env);
			WriteBlockDelcarations(env);

			m_writer.WriteLine("begin");

			WriteBlocksCode(env);

			m_writer.WriteLine("end behavioral;");
		}

		private void WriteBlocksCode(Environment env)
		{
			foreach (var block in env.AllBlocks)
				WriteBlockCode(block);
		}

		private void WriteBlockDelcarations(Environment env)
		{
			foreach (var block in env.AllBlocks)
				WriteBlockDeclaration(block);
		}

		private void WriteSignalDeclarations(Environment env)
		{
			foreach (var signal in env.AllSignals)
				WriteSignalDeclaration(signal);
		}

		private void WriteEntityDeclaration(Environment env)
		{
			m_writer.WriteLine("entity vhdl_code is port(");
			WritePortMappings(env);
			m_writer.WriteLine(");");
			m_writer.WriteLine("end vhdl_code;");
		}

		private void WritePortMappings(Environment env)
		{
			var inPortsSpec = string.Join(";\n", env.AllBlocks
				.OfType<InputVariableStorageBlock>()
				.Select(outVar => string.Format("    {0} : in {1}", outVar.VariableName, GetSignalTypeName(outVar.Output.SignalType))));

			var outPortsSpec = string.Join(";\n", env.AllBlocks
				.OfType<OutputVariableStorageBlock>()
				.Select(outVar => string.Format("    {0} : out {1}", outVar.VariableName, GetSignalTypeName(outVar.Output.SignalType))));

			if (inPortsSpec.Length > 0)
				m_writer.WriteLine(inPortsSpec + (outPortsSpec.Length > 0 ? ";" : ""));
			if (outPortsSpec.Length > 0)
				m_writer.WriteLine(outPortsSpec);
		}

		private string GetSignalTypeName(SignalType signalType)
		{
			var writer = m_signalTypeWriters.Get(signalType.GetType());
			return writer != null ? writer.GetName(signalType) : null;
		}

		private void WriteSignalDeclaration(Signal signal)
		{
			m_writer.WriteLine("    signal {0} : {1};", GetSignalName(signal), GetSignalTypeName(signal.Type));
		}

		public static string GetSignalName(Signal signal)
		{
			return string.Format("signal_{0}", signal.Hash);
		}

		private void WriteBlockDeclaration(BaseBlock block)
		{
			var writer = m_blockWriters.Get(block.GetType());
			if(writer != null)
				writer.WriteDeclaration(block);
		}

		private void WriteBlockCode(BaseBlock block)
		{
			var writer = m_blockWriters.Get(block.GetType());
			if (writer != null)
				writer.WriteCode(block);
		}
	}
}