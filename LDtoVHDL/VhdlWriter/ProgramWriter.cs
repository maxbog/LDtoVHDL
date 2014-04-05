﻿using System;
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
		private readonly TextWriter m_writer;

		private readonly ObjectDictionary<Type, BaseBlockWriter, WriterForAttribute> m_blockWriters;

		public ProgramWriter(TextWriter writer)
		{
			m_writer = writer;
			m_blockWriters = ObjectDictionary<Type, BaseBlockWriter, WriterForAttribute>.FromExecutingAssembly(type => type.BaseType, ffa => ffa.FormattedType, new object[] { m_writer });
		}

		public void WriteVhdlCode(Program env)
		{
			WriteEntityDeclaration(env);
			WriteArchitectureDefinition(env);
		}

		private void WriteArchitectureDefinition(Program env)
		{
			m_writer.WriteLine("architecture behavioral of vhdl_code is");

			WriteComponentReferences(env);
			WriteSignalDeclarations(env);
			WriteBlockDelcarations(env);

			m_writer.WriteLine("begin");

			WriteBlocksCode(env);

			m_writer.WriteLine("end behavioral;");
		}

		private void WriteComponentReferences(Program env)
		{
			var references = new HashSet<string>();
			foreach (var block in env.AllBlocks)
			{
				var reference = m_blockWriters.Get(block.GetType()).GetComponentReference(block);
				if (reference == null)
					continue;
				if (references.Contains(reference)) 
					continue;
				m_writer.WriteLine(reference);
				references.Add(reference);
			}
		}

		private void WriteBlocksCode(Program env)
		{
			foreach (var block in env.AllBlocks)
			{
				var writer = m_blockWriters.Get(block.GetType());
				if (writer != null)
					writer.WriteCode(block);
			}
		}

		private void WriteBlockDelcarations(Program env)
		{
			foreach (var block in env.AllBlocks)
			{
				var writer = m_blockWriters.Get(block.GetType());
				if(writer != null)
					writer.WriteDeclaration(block);
			}
		}

		private void WriteSignalDeclarations(Program env)
		{
			foreach (var signal in env.AllSignals)
				m_writer.WriteLine("    signal {0} : {1};", GetSignalName(signal), SignalTypeWriter.GetName(signal.Type));
		}

		private void WriteEntityDeclaration(Program env)
		{
			m_writer.WriteLine("entity vhdl_code is port(");
			WritePortMappings(env);
			m_writer.WriteLine(");");
			m_writer.WriteLine("end vhdl_code;");
		}

		private void WritePortMappings(Program env)
		{
			var inPortsSpec = string.Join(";\n", env.AllBlocks
				.OfType<InputVariableStorageBlock>()
				.Select(outVar => string.Format("    {0} : in {1}", outVar.VariableName, SignalTypeWriter.GetName(outVar.Output.SignalType))));

			var outPortsSpec = string.Join(";\n", env.AllBlocks
				.OfType<OutputVariableStorageBlock>()
				.Select(outVar => string.Format("    {0} : out {1}", outVar.VariableName, SignalTypeWriter.GetName(outVar.Output.SignalType))));

			if (inPortsSpec.Length > 0)
				m_writer.Write(inPortsSpec + (outPortsSpec.Length > 0 ? ";\n" : ""));
			if (outPortsSpec.Length > 0)
				m_writer.Write(outPortsSpec);

			if (env.AllBlocks.OfType<ClockBlock>().Any())
				m_writer.Write(";\n    CLK : in std_logic");

			m_writer.WriteLine();
		}

		public static string GetSignalName(Signal signal)
		{
			return string.Format("signal_{0}", signal.Hash);
		}
	}
}