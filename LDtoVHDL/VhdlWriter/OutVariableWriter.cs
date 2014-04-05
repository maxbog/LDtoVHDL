using System.IO;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(OutVariableBlock))]
	class OutVariableWriter : BaseBlockWriter
	{
		public OutVariableWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_OUT_VARIABLE";
		}

		public override void WriteCode(TextWriter writer, BaseBlock block)
		{
		}
	}
}