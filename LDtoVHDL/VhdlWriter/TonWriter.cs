using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(TonBlock))]
	class TonWriter : BaseBlockWriter
	{

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_TON";
		}
	}
}