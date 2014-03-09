using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	public class ProgramWriter
	{
		private readonly TextWriter m_writer;

		private readonly Dictionary<Type, BaseBlockWriter> m_blockWriters = new Dictionary<Type, BaseBlockWriter>();
		private readonly Dictionary<Type, SignalTypeWriter> m_signalTypeWriters = new Dictionary<Type, SignalTypeWriter>();

		public ProgramWriter(TextWriter writer)
		{
			m_writer = writer;
			FindWriters(m_blockWriters);
			FindWriters(m_signalTypeWriters);
		}

		private void FindWriters<T>(IDictionary<Type, T> writersDictionary)
		{
			var types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (var writerType in types.Where(type => typeof(T).IsAssignableFrom(type) && !type.IsAbstract))
			{
				var writerAttributes = (WriterForAttribute[])writerType.GetCustomAttributes(typeof(WriterForAttribute), false);
				if (!writerAttributes.Any())
					continue;
				var constructorInfo = writerType.GetConstructor(new[] { typeof(TextWriter) });
				Debug.Assert(constructorInfo != null, "constructorInfo != null");
				var factory = (T)constructorInfo.Invoke(new object[] { m_writer });
				foreach (var type in writerAttributes.Select(ffa => ffa.FormattedType))
					writersDictionary.Add(type, factory);
			}
		}

		private T GetWriter<T>(IDictionary<Type, T> writersDictionary, Type formattedType)
		{
			if (writersDictionary.ContainsKey(formattedType))
				return writersDictionary[formattedType];

			var baseType = formattedType;
			while (baseType != null && !writersDictionary.ContainsKey(baseType))
				baseType = baseType.BaseType;

			if (baseType != null)
				writersDictionary.Add(formattedType, writersDictionary[baseType]);

			return writersDictionary[formattedType];
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
				.OfType<InputVariable>()
				.Select(outVar => string.Format("    {0} : in {1}", outVar.VariableName, GetSignalTypeName(outVar.Output.SignalType))));

			var outPortsSpec = string.Join(";\n", env.AllBlocks
				.OfType<OutputVariable>()
				.Select(outVar => string.Format("    {0} : out {1}", outVar.VariableName, GetSignalTypeName(outVar.Output.SignalType))));

			if (inPortsSpec.Length > 0)
				m_writer.WriteLine(inPortsSpec + (outPortsSpec.Length > 0 ? ";" : ""));
			if (outPortsSpec.Length > 0)
				m_writer.WriteLine(outPortsSpec);
		}

		private string GetSignalTypeName(SignalType signalType)
		{
			var writer = GetWriter(m_signalTypeWriters, signalType.GetType());
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
			var writer = GetWriter(m_blockWriters, block.GetType());
			if(writer != null)
				writer.WriteDeclaration(block);
		}

		private void WriteBlockCode(BaseBlock block)
		{
			var writer = GetWriter(m_blockWriters, block.GetType());
			if (writer != null)
				writer.WriteCode(block);
		}
	}
}