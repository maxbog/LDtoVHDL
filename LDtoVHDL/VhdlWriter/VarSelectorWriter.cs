using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(VarSelector))]
	class VarSelectorWriter : BaseBlockWriter
	{
		public VarSelectorWriter(TextWriter writer) : base(writer)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_VAR_SELECTOR";
		}

		protected override IEnumerable<Tuple<string, string>> GetGenericMapping(BaseBlock block)
		{
			var varSelector = (VarSelector) block;
			var inputsBus = ((BusType)varSelector.Inputs.SignalType);
			yield return Tuple.Create("signal_count", inputsBus.SignalCount.ToString(CultureInfo.InvariantCulture));
		}
	}
}