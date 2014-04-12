using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(NocBlock))]
	[WriterFor(typeof(NccBlock))]
	class ContactWriter : BaseBlockWriter
	{

		public ContactWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return block is NocBlock ? "BLK_NOC"
				 : block is NccBlock ? "BLK_NCC"
				 : null;
		}

		public override string GetDefinition(BaseBlock block)
		{
			return TemplateResolver.GetWithReplacements(string.Format("BlockDefinition/{0}.vhd", GetVhdlType(block)));
		}

		public override string GetComponentReference(BaseBlock block)
		{
			return TemplateResolver.GetWithReplacements(string.Format("BlockReference/{0}.ref", GetVhdlType(block)));
		}
	}
}