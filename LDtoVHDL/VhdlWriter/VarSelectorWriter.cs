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
			return MakeTypedName(string.Format("BLK_VAR_SELECTOR{0}", GetSignalCount(varSelector)), varSelector.Output.SignalType);
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
			var signalCount = GetSignalCount(varSelector);
			return PrepareTemplateForOutput(TemplateResolver.GetWithReplacements("BlockReference/BLK_VAR_SELECTOR.ref", 
				new Dictionary<string, string>
				{
					{ "type", SignalTypeWriter.GetName(varSelector.Output.SignalType) },
					{"signal_count", signalCount.ToString(CultureInfo.InvariantCulture)},
					{"max_vector_idx", (signalCount-1).ToString(CultureInfo.InvariantCulture)}
				}));
		}

		public override string GetDefinition(BaseBlock block)
		{
			var varSelector = (VarSelector)block;
			var signalCount = GetSignalCount(varSelector);
			return TemplateResolver.GetWithReplacements("BlockDefinition/BLK_VAR_SELECTOR.vhd",
				new Dictionary<string, string>
				{
					{"type", SignalTypeWriter.GetName(varSelector.Output.SignalType)},
					{"signal_count", signalCount.ToString(CultureInfo.InvariantCulture)},
					{"max_vector_idx", (signalCount-1).ToString(CultureInfo.InvariantCulture)},
					{"input_chooser", MakeInputChooser(varSelector)}
				});
		}

		private string MakeInputChooser(VarSelector varSelector)
		{
			var signalCount = GetSignalCount(varSelector);
			var result = new StringBuilder();
			for (int i = 0; i < signalCount; ++i)
			{
				var inChooser = "";
				for (int j = 0; j < i; ++j)
				{
					inChooser += '0';
				}
				inChooser += '1';
				for (int j = i+1; j < signalCount; ++j)
				{
					inChooser += '-';
				}
				result.AppendFormat("INS({0}) when std_match(CONTROL, \"{1}\") else\n         ", signalCount - i - 1, inChooser);
			}
			return result.ToString();
		}

		protected override string GetName(BaseBlock block)
		{
			var selector = block as VarSelector;
			Debug.Assert(selector != null, "selector != null");
			return base.GetName(block) + "_" + string.Join("_", selector.Output.OtherSidePorts.Select(port => port.ParentBlock.Id));
		}
	}
}