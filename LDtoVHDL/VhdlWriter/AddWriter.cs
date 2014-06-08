using System;
using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(AddBlock))]
	class AddWriter : BaseBlockWriter
	{
		public AddWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			var addBlock = (AddBlock) block;
			return MakeTypedName("BLK_ADD", addBlock.Output.SignalType);
		}
		
		public override string GetComponentReference(BaseBlock block)
		{
			var addBlock = (AddBlock)block;
			return PrepareTemplateForOutput(TemplateResolver.GetWithReplacements("BlockReference/BLK_ADD.ref", new Dictionary<string, string> { { "type", SignalTypeWriter.GetName(addBlock.Output.SignalType) } }));
		}

		public override string GetDefinition(BaseBlock block)
		{
			var addBlock = (AddBlock)block;
			return TemplateResolver.GetWithReplacements("BlockDefinition/BLK_ADD.vhd", new Dictionary<string, string>
			{
				{ "type", SignalTypeWriter.GetName(addBlock.Output.SignalType) },
				{ "zero", SignalTypeWriter.GetValueConstructor(addBlock.Output.SignalType, null) }
			});
		}

		protected override IEnumerable<Tuple<string, string>> GetGenericMapping(BaseBlock block)
		{
			var addBlock = (AddBlock)block;
			yield return Tuple.Create("ins_count", SignalTypeWriter.GetValueConstructor(BuiltinType.Integer, addBlock.InputsCount));

		}

		protected override IEnumerable<Tuple<string, string>> GetPortMapping(BaseBlock block)
		{
			var addBlock = (AddBlock) block;
			yield return Tuple.Create("INS", GetSignalName(addBlock.InputBus));
			yield return Tuple.Create("Q", GetSignalName(addBlock.Output));
			yield return Tuple.Create("EN", GetSignalName(addBlock.Enable));
			yield return Tuple.Create("ENO", GetSignalName(addBlock.EnableOut));
		}
	}
}