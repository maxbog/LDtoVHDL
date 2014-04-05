using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(CoilBlock))]
	class CoilWriter : BaseBlockWriter
	{
		private readonly string m_referenceTemplate = PrepareTemplateForOutput(@"
component BLK_COIL is
    port (EN      : in  std_logic;
          ENO     : out std_logic;
          MEM_OUT : out std_logic);
end component;");

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_COIL";
		}

		public override string GetComponentReference(BaseBlock block)
		{
			return m_referenceTemplate;
		}
	}
}