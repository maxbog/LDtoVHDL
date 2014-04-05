using System.IO;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(InVariableBlock))]
	class InVariableWriter : BaseBlockWriter
	{
		public InVariableWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_IN_VARIABLE";
		}

		public override void WriteCode(TextWriter writer, BaseBlock block)
		{
		}
	}
}