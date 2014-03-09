using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			var portMapping = string.Join(", ",
				GetVhdlPortMapping(block).Select(mapping => string.Format("{0} => {1}", mapping.Item1, mapping.Item2 ?? "open")));
			var genericMapping = string.Join(", ",
				block.VhdlGenericMapping.Select(mapping => string.Format("{0} => {1}", mapping.Item1, mapping.Item2 ?? "open")));
			if (genericMapping != "")
				genericMapping = string.Format("generic map({0}) ", genericMapping);
			Writer.WriteLine("    {0}: {1} {2}port map ({3});", block.VhdlName, block.VhdlType, genericMapping, portMapping);
		}

		protected virtual IEnumerable<Tuple<string, string>> GetVhdlPortMapping(BaseBlock block)
		{
			return block.Ports.Select(port => Tuple.Create(port.Key, port.Value.ConnectedSignal == null ? null : port.Value.ConnectedSignal.VhdlName));
		}
	}

	[WriterFor(typeof(InputVariable))]
	class MemoryVariableWriter : BaseBlockWriter
	{
		public MemoryVariableWriter(TextWriter writer) : base(writer)
		{
		}

		protected override IEnumerable<Tuple<string, string>> GetVhdlPortMapping(BaseBlock block)
		{
			var inputVar = (InputVariable)block;
			yield return Tuple.Create(inputVar.Input.Name, inputVar.VariableName);
			yield return Tuple.Create(inputVar.Output.Name, inputVar.Output.ConnectedSignal == null ? null : inputVar.Output.ConnectedSignal.VhdlName);
			yield return Tuple.Create(inputVar.Load.Name, inputVar.Load.ConnectedSignal == null ? null : inputVar.Load.ConnectedSignal.VhdlName);
		}
	}
}