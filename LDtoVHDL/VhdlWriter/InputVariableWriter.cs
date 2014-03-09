using System;
using System.Collections.Generic;
using System.IO;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(InputVariable))]
	class InputVariableWriter : BaseBlockWriter
	{
		public InputVariableWriter(TextWriter writer) : base(writer)
		{
		}

		protected override IEnumerable<Tuple<string, string>> GetPortMapping(BaseBlock block)
		{
			var inputVar = (InputVariable)block;
			yield return Tuple.Create(inputVar.Input.Name, inputVar.VariableName);
			yield return Tuple.Create(inputVar.Output.Name, inputVar.Output.ConnectedSignal == null ? null : string.Format("signal_{0}", inputVar.Output.ConnectedSignal.Hash));
			yield return Tuple.Create(inputVar.Load.Name, inputVar.Load.ConnectedSignal == null ? null : string.Format("signal_{0}", inputVar.Load.ConnectedSignal.Hash));
		}
	}
}