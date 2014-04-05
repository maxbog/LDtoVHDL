using System.IO;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(LocalVariableStorageBlock))]
	class VariableStorageWriter : BaseBlockWriter
	{
		private readonly string m_referenceTemplate = PrepareTemplateForOutput(@"
component BLK_VARIABLE_STORAGE_{0} is
    port (VAR_IN  : in  {0};
          VAR_OUT : out {0};
          LOAD    : in  boolean);
end component;");

		public VariableStorageWriter(TextWriter writer) : base(writer)
		{
		}

		public override string GetComponentReference(BaseBlock block)
		{
			var varBlock = (VariableStorageBlock) block;
			return string.Format(m_referenceTemplate, SignalTypeWriter.GetName(varBlock.SignalType));
		}

		public override string GetVhdlType(BaseBlock block)
		{
			var ivsBlock = (VariableStorageBlock)block;
			return MakeTypedName("BLK_VARIABLE_STORAGE", ivsBlock.SignalType);
		}
	}
}