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
		private readonly string m_referenceTemplate = PrepareTemplateForOutput(@"
component BLK_VAR_SELECTOR_{0} is
	generic (signal_count : integer);
    port (INS        : in  {0}_vector(signal_count-1 downto 0);
		  MEMORY_IN  : in  {0};
		  CONTROL    : in  std_logic_vector(signal_count downto 0);
		  Q          : out {0});
end component;");

		public VarSelectorWriter(TextWriter writer) : base(writer)
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
			return string.Format(m_referenceTemplate, SignalTypeWriter.GetName(varSelector.Output.SignalType));
		}
	}
}