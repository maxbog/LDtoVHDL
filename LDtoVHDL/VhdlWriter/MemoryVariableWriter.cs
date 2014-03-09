using System;
using System.Collections.Generic;
using System.IO;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
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