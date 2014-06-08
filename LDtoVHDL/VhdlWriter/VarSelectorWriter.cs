using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
			var varSelector = (VarSelector)block;
			return MakeTypedName("BLK_VAR_SELECTOR", varSelector.Output.SignalType);
		}

		private static int GetSignalCount(VarSelector varSelector)
		{
			var inputsBus = ((BusType) varSelector.Inputs.SignalType);
			var signalCount = inputsBus.SignalCount;
			return signalCount;
		}

		public override string GetComponentReference(BaseBlock block)
		{
			var varSelector = (VarSelector)block;
			return PrepareTemplateForOutput(TemplateResolver.GetWithReplacements("BlockReference/BLK_VAR_SELECTOR.ref", 
				new Dictionary<string, string>
				{
					{ "type", SignalTypeWriter.GetName(varSelector.Output.SignalType) },
				}));
		}

		public override string GetDefinition(BaseBlock block)
		{
			var varSelector = (VarSelector)block;
			return TemplateResolver.GetWithReplacements("BlockDefinition/BLK_VAR_SELECTOR.vhd",
				new Dictionary<string, string>
				{
					{"type", SignalTypeWriter.GetName(varSelector.Output.SignalType)},
				});
		}

		protected override string GetName(BaseBlock block)
		{
			var selector = block as VarSelector;
			Debug.Assert(selector != null, "selector != null");
			return base.GetName(block) + "_" + string.Join("_", selector.Output.OtherSidePorts.Select(port => port.ParentBlock.Id));
		}


		protected override IEnumerable<Tuple<string, string>> GetGenericMapping(BaseBlock block)
		{
			yield return Tuple.Create("signal_count", SignalTypeWriter.GetValueConstructor(BuiltinType.Integer, GetSignalCount(block as VarSelector)));
		}
	}
}