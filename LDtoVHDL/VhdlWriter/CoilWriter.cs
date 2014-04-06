using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(CoilBlock))]
	class CoilWriter : BaseBlockWriter
	{

		public CoilWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_COIL";
		}

		public override string GetDefinition(BaseBlock block)
		{
			return TemplateResolver.GetWithReplacements("BlockDefinition/BLK_COIL.vhd");
		}

		public override string GetComponentReference(BaseBlock block)
		{
			return TemplateResolver.GetWithReplacements("BlockReference/BLK_COIL.ref");
		}
	}
}