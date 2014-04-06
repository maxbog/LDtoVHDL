using System;
using System.Collections.Generic;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(TonBlock))]
	class TonWriter : BaseBlockWriter
	{
		public TonWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_TON";
		}
		
		protected override IEnumerable<Tuple<string, string>> GetPortMapping(BaseBlock block)
		{
			var tonBlock = (TonBlock)block;
			yield return Tuple.Create("VAR_WRITE", GetSignalName(tonBlock.MemoryOutput));
			yield return Tuple.Create("VAR_READ", GetSignalName(tonBlock.MemoryInput));
			yield return Tuple.Create("EN", GetSignalName(tonBlock.Input));
			yield return Tuple.Create("ENO", GetSignalName(tonBlock.Output));
			yield return Tuple.Create("PT", GetSignalName(tonBlock.PresetTime));
			yield return Tuple.Create("ET", GetSignalName(tonBlock.ElapsedTime));
		}

		public override string GetComponentReference(BaseBlock block)
		{
			return PrepareTemplateForOutput(TemplateResolver.GetWithReplacements("BlockReference/BLK_TON.ref"));
		}


		public override string GetDefinition(BaseBlock block)
		{
			return TemplateResolver.GetWithReplacements("BlockDefinition/BLK_TON.vhd");
		}
	}
}