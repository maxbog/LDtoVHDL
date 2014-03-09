using System.IO;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(InVariableBlock))]
	class InVariableWriter : BaseBlockWriter
	{
		public InVariableWriter(TextWriter writer) : base(writer)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_IN_VARIABLE";
		}
	}
}