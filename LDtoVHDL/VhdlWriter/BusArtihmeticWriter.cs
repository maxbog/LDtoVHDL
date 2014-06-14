using System;
using System.Collections.Generic;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(AddBlock))]
	[WriterFor(typeof(MulBlock))]
	public class BusArtihmeticWriter : BaseBlockWriter
	{
		public BusArtihmeticWriter(TemplateResolver templateResolver)
			: base(templateResolver)
		{
		}

		private string GetBlockTypeName(BaseBlock block)
		{
			return block is AddBlock ? "BLK_ADD"
				: block is MulBlock ? "BLK_MUL"
				: null;
		}

		public override string GetVhdlType(BaseBlock block)
		{
			var arithBlock = (BusArithmeticBlock)block;
			return MakeTypedName(GetBlockTypeName(block), arithBlock.Output.SignalType);
		}
		
		public override string GetComponentReference(BaseBlock block)
		{
			var arithBlock = (BusArithmeticBlock)block;
			var refTemplateName = string.Format("BlockReference/{0}.ref", GetBlockTypeName(block));
			return PrepareTemplateForOutput(TemplateResolver.GetWithReplacements(refTemplateName, new Dictionary<string, string> { { "type", SignalTypeWriter.GetName(arithBlock.Output.SignalType) } }));
		}

		public override string GetDefinition(BaseBlock block)
		{
			var arithBlock = (BusArithmeticBlock)block;
			var defTemplateName = string.Format("BlockDefinition/{0}.vhd", GetBlockTypeName(block));
			return TemplateResolver.GetWithReplacements(defTemplateName, new Dictionary<string, string>
			{
				{ "type", SignalTypeWriter.GetName(arithBlock.Output.SignalType) },
				{ "one", SignalTypeWriter.GetValueConstructor(arithBlock.Output.SignalType, 1) },
				{ "zero", SignalTypeWriter.GetValueConstructor(arithBlock.Output.SignalType, null) }
			});
		}

		protected override IEnumerable<Tuple<string, string>> GetGenericMapping(BaseBlock block)
		{
			var addBlock = (BusArithmeticBlock)block;
			yield return Tuple.Create("ins_count", SignalTypeWriter.GetValueConstructor(BuiltinType.Integer, addBlock.InputsCount));

		}

		protected override IEnumerable<Tuple<string, string>> GetPortMapping(BaseBlock block)
		{
			var addBlock = (BusArithmeticBlock)block;
			yield return Tuple.Create("INS", GetSignalName(addBlock.InputBus));
			yield return Tuple.Create("Q", GetSignalName(addBlock.Output));
			yield return Tuple.Create("EN", GetSignalName(addBlock.Enable));
			yield return Tuple.Create("ENO", GetSignalName(addBlock.EnableOut));
		}
	}


	[WriterFor(typeof(SubBlock))]
	[WriterFor(typeof(DivBlock))]
	[WriterFor(typeof(ModBlock))]
	public class TwoInputArtihmeticWriter : BaseBlockWriter
	{
		public TwoInputArtihmeticWriter(TemplateResolver templateResolver)
			: base(templateResolver)
		{
		}

		private string GetBlockTypeName(BaseBlock block)
		{
			return block is SubBlock ? "BLK_SUB"
				: block is DivBlock ? "BLK_DIV"
				: block is ModBlock ? "BLK_MOD"
				: null;
		}

		public override string GetVhdlType(BaseBlock block)
		{
			var arithBlock = (TwoInputArithmeticBlock)block;
			return MakeTypedName(GetBlockTypeName(block), arithBlock.Output.SignalType);
		}

		public override string GetComponentReference(BaseBlock block)
		{
			var arithBlock = (TwoInputArithmeticBlock)block;
			var refTemplateName = string.Format("BlockReference/{0}.ref", GetBlockTypeName(block));
			return PrepareTemplateForOutput(TemplateResolver.GetWithReplacements(refTemplateName, new Dictionary<string, string> { { "type", SignalTypeWriter.GetName(arithBlock.Output.SignalType) } }));
		}

		public override string GetDefinition(BaseBlock block)
		{
			var arithBlock = (TwoInputArithmeticBlock)block;
			var defTemplateName = string.Format("BlockDefinition/{0}.vhd", GetBlockTypeName(block));
			return TemplateResolver.GetWithReplacements(defTemplateName, new Dictionary<string, string>
			{
				{ "type", SignalTypeWriter.GetName(arithBlock.Output.SignalType) }
			});
		}
	}
}