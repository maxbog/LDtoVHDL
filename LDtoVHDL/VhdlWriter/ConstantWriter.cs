using System.IO;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(ConstantBlock))]
	class ConstantWriter : BaseBlockWriter
	{
		public ConstantWriter(TextWriter writer) : base(writer)
		{
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_CONST";
		}

		public override void WriteCode(BaseBlock block)
		{
			var constBlock = (ConstantBlock) block;

			var signalName = ProgramWriter.GetSignalName(constBlock.Output.ConnectedSignal);
			var valueConstructor = SignalTypeWriter.GetValueConstructor(constBlock.ValueType, constBlock.Value);
			Writer.WriteLine("    {0} <= {1};", signalName, valueConstructor);
		}
	}
}