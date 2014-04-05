using System;
using System.Collections.Generic;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(InputVariableStorageBlock))]
	class InputVariableStorageWriter : VariableStorageWriter
	{
		public InputVariableStorageWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
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