using System.Collections.Generic;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(LocalVariableStorageBlock))]
	class VariableStorageWriter : BaseBlockWriter
	{
		public VariableStorageWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetComponentReference(BaseBlock block)
		{
			var varBlock = (VariableStorageBlock) block;
			return PrepareTemplateForOutput(TemplateResolver.GetWithReplacements("BlockReference/BLK_VARIABLE_STORAGE.ref", new Dictionary<string, string> { { "type", SignalTypeWriter.GetName(varBlock.SignalType) } }));
		}

		public override string GetVhdlType(BaseBlock block)
		{
			var ivsBlock = (VariableStorageBlock)block;
			return MakeTypedName("BLK_VARIABLE_STORAGE", ivsBlock.SignalType);
		}

		public override string GetDefinition(BaseBlock block)
		{
			var varBlock = (VariableStorageBlock)block;
			return TemplateResolver.GetWithReplacements("BlockDefinition/BLK_VARIABLE_STORAGE.vhd", new Dictionary<string, string> { { "type", SignalTypeWriter.GetName(varBlock.SignalType) } });
		}
	}
}