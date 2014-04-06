using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(ContactBlock))]
	class ContactWriter : BaseBlockWriter
	{

		public ContactWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_CONTACT";
		}

		public override string GetDefinition(BaseBlock block)
		{
			return TemplateResolver.GetWithReplacements("BlockDefinition/BLK_CONTACT.vhd");
		}

		public override string GetComponentReference(BaseBlock block)
		{
			return TemplateResolver.GetWithReplacements("BlockReference/BLK_CONTACT.ref");
		}
	}
}