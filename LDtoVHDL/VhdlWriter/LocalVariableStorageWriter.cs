using System.IO;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(LocalVariableStorageBlock))]
	class LocalVariableStorageWriter : BaseBlockWriter
	{
		public LocalVariableStorageWriter(TextWriter writer) : base(writer)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_LOCAL_VARIABLE_STORAGE";
		}
	}
}