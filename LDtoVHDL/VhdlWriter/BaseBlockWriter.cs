using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(BaseBlock))]
	public class BaseBlockWriter
	{
		protected readonly TextWriter Writer;

		public BaseBlockWriter(TextWriter writer)
		{
			Writer = writer;
		}

		public virtual void WriteDeclaration(BaseBlock block)
		{

		}
		
		public virtual void WriteCode(BaseBlock block)
		{
			Writer.WriteLine("    {0}: {1} {2}port map ({3});", GetName(block), GetVhdlType(block), GetGenericMappingString(block), GetPortMappingStiring(block));
		}

		private string GetPortMappingStiring(BaseBlock block)
		{
			return string.Join(", ", GetPortMapping(block).Select(GetSingleMappingString));
		}

		private string GetGenericMappingString(BaseBlock block)
		{
			var genericMapping = string.Join(", ", GetGenericMapping(block).Select(GetSingleMappingString));
			return genericMapping != "" ? string.Format("generic map({0}) ", genericMapping) : "";
		}

		private static string GetSingleMappingString(Tuple<string, string> mapping)
		{
			return string.Format("{0} => {1}", mapping.Item1, mapping.Item2 ?? "open");
		}

		protected virtual string GetVhdlType(BaseBlock block)
		{
			var fieldInfo = block.GetType().GetField("TYPE", BindingFlags.Static | BindingFlags.Public);
			if (fieldInfo != null)
				return fieldInfo.GetValue(null) as string;
			return null;
		}

		protected virtual string GetName(BaseBlock block)
		{
			return string.Format("block_{0}", block.Id);
		}

		protected virtual IEnumerable<Tuple<string, string>> GetGenericMapping(BaseBlock block)
		{
			return Enumerable.Empty<Tuple<string,string>>();
		}

		protected virtual IEnumerable<Tuple<string, string>> GetPortMapping(BaseBlock block)
		{
			return block.Ports.Select(port => Tuple.Create(port.Key, port.Value.ConnectedSignal == null ? null : string.Format("signal_{0}", port.Value.ConnectedSignal.Hash)));
		}
	}

	[WriterFor(typeof(OutputVariable))]
	class OutputVariableWriter : BaseBlockWriter
	{
		public OutputVariableWriter(TextWriter writer) : base(writer)
		{
		}

		public override void WriteCode(BaseBlock block)
		{
			base.WriteCode(block);

			var outputVariable = (OutputVariable) block;
			Writer.WriteLine("    {0} <= {1};", outputVariable.VariableName, ProgramWriter.GetSignalName(outputVariable.Output.ConnectedSignal));
		}
	}

	[WriterFor(typeof(AddBlock))]
	class AddBlockWriter : BaseBlockWriter
	{
		public AddBlockWriter(TextWriter writer) : base(writer)
		{
		}

		protected override string GetVhdlType(BaseBlock block)
		{
			var addBlock = (AddBlock) block;
			if (addBlock.Output.SignalType.IsSigned)
				return "BLK_ADD_SIGNED";
			if (addBlock.Output.SignalType.IsUnsigned)
				return "BLK_ADD_UNSIGNED";
			throw new InvalidOperationException("ADD block can only operate on integer types");
		}
	}
}