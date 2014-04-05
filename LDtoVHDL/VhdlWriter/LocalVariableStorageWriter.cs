using System.IO;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	class LocalVariableStorageWriter : VariableStorageWriter
	{
		public LocalVariableStorageWriter(TextWriter writer) : base(writer)
		{
		}
	}
}