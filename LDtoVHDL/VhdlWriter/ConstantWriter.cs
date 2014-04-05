using System.IO;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(ConstantBlock))]
	class ConstantWriter : BaseBlockWriter
	{
		public ConstantWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_CONST";
		}

		public override void WriteCode(TextWriter writer, BaseBlock block)
		{
			var constBlock = (ConstantBlock) block;

			var signalName = ProgramWriter.GetSignalName(constBlock.Output.ConnectedSignal);
			var valueConstructor = SignalTypeWriter.GetValueConstructor(constBlock.ValueType, constBlock.Value);
			writer.WriteLine("    {0} <= {1};", signalName, valueConstructor);
		}
	}
}