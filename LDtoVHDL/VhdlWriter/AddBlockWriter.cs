using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(AddBlock))]
	class AddBlockWriter : BaseBlockWriter
	{
		private readonly string m_referenceTemplate = PrepareTemplateForOutput(@"
component BLK_ADD_{0} is
    port (EN   : in  boolean;
          ENO  : out boolean;
          A    : in  {0};
          B    : in  {0};
          Q    : out {0});
end component;");


		public AddBlockWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			var addBlock = (AddBlock) block;
			return MakeTypedName("BLK_ADD", addBlock.Output.SignalType);
		}
		
		public override string GetComponentReference(BaseBlock block)
		{
			var addBlock = (AddBlock)block;
			return string.Format(m_referenceTemplate, SignalTypeWriter.GetName(addBlock.Output.SignalType));
		}
	}
}