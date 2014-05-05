using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(PositiveEdgeDetectorBlock))]
	[WriterFor(typeof(NegativeEdgeDetectorBlock))]
	class EdgeDetectorWriter : VariableBlockWriter
	{

		public EdgeDetectorWriter(TemplateResolver templateResolver)
			: base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return block is PositiveEdgeDetectorBlock ? "BLK_POSITIVE_EDGE_DETECTOR"
				 : block is NegativeEdgeDetectorBlock ? "BLK_NEGATIVE_EDGE_DETECTOR"
				 : null;
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