using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(VarSelector))]
	class VarSelectorWriter : BaseBlockWriter
	{
		public VarSelectorWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			var addBlock = (VarSelector)block;
			return MakeTypedName("BLK_VAR_SELECTOR", addBlock.Output.SignalType);
		}

		protected override IEnumerable<Tuple<string, string>> GetGenericMapping(BaseBlock block)
		{
			var varSelector = (VarSelector) block;
			var inputsBus = ((BusType)varSelector.Inputs.SignalType);
			yield return Tuple.Create("signal_count", inputsBus.SignalCount.ToString(CultureInfo.InvariantCulture));
		}

		public override string GetComponentReference(BaseBlock block)
		{
			var varSelector = (VarSelector)block;
			return PrepareTemplateForOutput(TemplateResolver.GetWithReplacements("BlockReference/BLK_VAR_SELECTOR.ref", new Dictionary<string, string> { { "type", SignalTypeWriter.GetName(varSelector.Output.SignalType) } }));
		}

		public override string GetDefinition(BaseBlock block)
		{
			var varSelector = (VarSelector)block;
			return TemplateResolver.GetWithReplacements("BlockDefinition/BLK_VAR_SELECTOR.vhd",
					new Dictionary<string, string> {{"type", SignalTypeWriter.GetName(varSelector.Output.SignalType)}});
		}
	}
}