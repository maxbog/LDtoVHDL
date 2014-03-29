using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(AddBlock))]
	class AddBlockWriter : BaseBlockWriter
	{
		private readonly string m_referenceTemplate = PrepareTemplateForOutput(@"
component BLK_ADD_{0} is
    port (Exec : in  std_logic;
    EN   : in  std_logic;
    ENO  : out std_logic;
    A    : in  {0};
    B    : in  {0};
    Q    : out {0});
end component;");

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

		public static string PrepareTemplateForOutput(string inputTemplate)
		{
			var trimmedTemplate = inputTemplate.Trim().Replace("\r\n", "\n");
			return PrependIndentation(trimmedTemplate);
		}

		private static string PrependIndentation(string trimmedTemplate)
		{
			return "    " + trimmedTemplate.Replace("\n", "\n    ");
		}

		public override void WriteComponentReferences(IEnumerable<BaseBlock> blocks)
		{
			foreach (var signalType in blocks.OfType<AddBlock>().Select(blk => blk.Output.SignalType).Distinct())
				Writer.WriteLine(m_referenceTemplate, signalType.ToString().ToUpperInvariant());
		}
	}
}