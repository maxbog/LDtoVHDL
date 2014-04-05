using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(ContactBlock))]
	class ContactWriter : BaseBlockWriter
	{
		private readonly string m_referenceTemplate = PrepareTemplateForOutput(@"
component BLK_CONTACT is
    port (EN      : in  std_logic;
          ENO     : out std_logic;
          MEM_IN  : in std_logic);
end component;");

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_CONTACT";
		}
		public override string GetComponentReference(BaseBlock block)
		{
			return m_referenceTemplate;
		}
	}
}