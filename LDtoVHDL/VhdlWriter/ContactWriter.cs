using System.IO;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(ContactBlock))]
	class ContactWriter : BaseBlockWriter
	{
		public ContactWriter(TextWriter writer) : base(writer)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_CONTACT";
		}
	}
}