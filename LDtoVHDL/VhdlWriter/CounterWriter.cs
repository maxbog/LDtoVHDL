using System;
using System.Collections.Generic;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	public abstract class CounterWriter : VariableBlockWriter
	{
		protected CounterWriter(TemplateResolver templateResolver)
			: base(templateResolver)
		{

		}
		
		protected override IEnumerable<Tuple<string, string>> GetPortMapping(BaseBlock block)
		{
			var counterBlock = (CounterBlock)block;
			yield return Tuple.Create("VAR_WRITE", GetSignalName(counterBlock.MemoryOutput));
			yield return Tuple.Create("VAR_READ", GetSignalName(counterBlock.MemoryInput));
			yield return Tuple.Create("EN", GetSignalName(counterBlock.Input));
			yield return Tuple.Create("ENO", GetSignalName(counterBlock.Output));
			yield return Tuple.Create("PV", GetSignalName(counterBlock.PresetValue));
			yield return Tuple.Create("CV", GetSignalName(counterBlock.CurrentValue));
		}

		public override string GetComponentReference(BaseBlock block)
		{
			return PrepareTemplateForOutput(TemplateResolver.GetWithReplacements(string.Format("BlockReference/{0}.ref", GetVhdlType(block))));
		}


		public override string GetDefinition(BaseBlock block)
		{
			return TemplateResolver.GetWithReplacements(string.Format("BlockDefinition/{0}.vhd", GetVhdlType(block)));
		}

	}

	[WriterFor(typeof(CtuBlock))]
	class CtuWriter : CounterWriter
	{
		public CtuWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_CTU";
		}

		protected override IEnumerable<Tuple<string, string>> GetPortMapping(BaseBlock block)
		{
			var ctuBlock = (CtuBlock)block;
			foreach (var tuple in base.GetPortMapping(block))
			{
				yield return tuple;
			}

			yield return Tuple.Create("R", GetSignalName(ctuBlock.Reset));
		}
	}

	[WriterFor(typeof(CtdBlock))]
	class CtdWriter : CounterWriter
	{
		public CtdWriter(TemplateResolver templateResolver)
			: base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_CTD";
		}

		protected override IEnumerable<Tuple<string, string>> GetPortMapping(BaseBlock block)
		{
			var ctuBlock = (CtdBlock)block;
			foreach (var tuple in base.GetPortMapping(block))
			{
				yield return tuple;
			}

			yield return Tuple.Create("LD", GetSignalName(ctuBlock.LoadValue));
		}
	}
}