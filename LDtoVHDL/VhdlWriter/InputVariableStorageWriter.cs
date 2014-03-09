using System;
using System.Collections.Generic;
using System.IO;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(InputVariableStorageBlock))]
	class InputVariableStorageWriter : BaseBlockWriter
	{
		public InputVariableStorageWriter(TextWriter writer) : base(writer)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_INPUT_VARIABLE_STORAGE";
		}

		protected override IEnumerable<Tuple<string, string>> GetPortMapping(BaseBlock block)
		{
			var inputVar = (InputVariableStorageBlock)block;
			yield return Tuple.Create(inputVar.Input.Name, inputVar.VariableName);
			yield return Tuple.Create(inputVar.Output.Name, inputVar.Output.ConnectedSignal == null ? null : string.Format("signal_{0}", inputVar.Output.ConnectedSignal.Hash));
			yield return Tuple.Create(inputVar.Load.Name, inputVar.Load.ConnectedSignal == null ? null : string.Format("signal_{0}", inputVar.Load.ConnectedSignal.Hash));
		}
	}
}