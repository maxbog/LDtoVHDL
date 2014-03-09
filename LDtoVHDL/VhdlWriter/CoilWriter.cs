using System.IO;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(CoilBlock))]
	class CoilWriter : BaseBlockWriter
	{
		public CoilWriter(TextWriter writer) : base(writer)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_COIL";
		}
	}
}