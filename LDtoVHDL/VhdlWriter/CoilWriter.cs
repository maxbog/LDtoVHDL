using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(CoilBlock))]
	[WriterFor(typeof(NegatedCoilBlock))]
	class CoilWriter : VariableBlockWriter
	{

		public CoilWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return block is CoilBlock ? "BLK_COIL" : "BLK_NEG_COIL";
		}

		public override string GetDefinition(BaseBlock block)
		{
			return TemplateResolver.GetWithReplacements(string.Format("BlockDefinition/{0}.vhd", GetVhdlType(block)));
		}

		public override string GetComponentReference(BaseBlock block)
		{
			return PrepareTemplateForOutput(TemplateResolver.GetWithReplacements(string.Format("BlockReference/{0}.ref", GetVhdlType(block))));
		}
	}
}