using System;
using System.Collections.Generic;
using System.Diagnostics;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(TonBlock))]
	[WriterFor(typeof(TofBlock))]
	class TimerWriter : VariableBlockWriter
	{
		public TimerWriter(TemplateResolver templateResolver) : base(templateResolver)
		{

		}

		public override string GetVhdlType(BaseBlock block)
		{
			return block is TonBlock ? "BLK_TON" 
				 : block is TofBlock ? "BLK_TOF" 
				 : null;
		}
		
		protected override IEnumerable<Tuple<string, string>> GetPortMapping(BaseBlock block)
		{
			var tonBlock = (TimerBlock)block;
			yield return Tuple.Create("VAR_WRITE", GetSignalName(tonBlock.MemoryOutput));
			yield return Tuple.Create("VAR_READ", GetSignalName(tonBlock.MemoryInput));
			yield return Tuple.Create("EN", GetSignalName(tonBlock.Input));
			yield return Tuple.Create("ENO", GetSignalName(tonBlock.Output));
			yield return Tuple.Create("PT", GetSignalName(tonBlock.PresetTime));
			yield return Tuple.Create("ET", GetSignalName(tonBlock.ElapsedTime));
		}

		public override string GetComponentReference(BaseBlock block)
		{
			return PrepareTemplateForOutput(TemplateResolver.GetWithReplacements(string.Format("BlockReference/{0}.ref", GetVhdlType(block))));
		}


		public override string GetDefinition(BaseBlock block)
		{
			return TemplateResolver.GetWithReplacements(string.Format("BlockDefinition/{0}.vhd", GetVhdlType(block)));
		}

		protected override IEnumerable<Tuple<string, string>> GetGenericMapping(BaseBlock block)
		{
			yield return Tuple.Create("CLK_frequency", "CLK_frequency");
		}

	}
}