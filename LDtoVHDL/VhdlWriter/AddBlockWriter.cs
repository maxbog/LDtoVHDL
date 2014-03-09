using System;
using System.IO;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(AddBlock))]
	class AddBlockWriter : BaseBlockWriter
	{
		public AddBlockWriter(TextWriter writer) : base(writer)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			var addBlock = (AddBlock) block;
			if (addBlock.Output.SignalType.IsSigned)
				return "BLK_ADD_SIGNED";
			if (addBlock.Output.SignalType.IsUnsigned)
				return "BLK_ADD_UNSIGNED";
			throw new InvalidOperationException("ADD block can only operate on integer types");
		}

	}
}