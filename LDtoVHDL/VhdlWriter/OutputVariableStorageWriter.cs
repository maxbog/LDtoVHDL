using System.IO;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(OutputVariableStorageBlock))]
	class OutputVariableStorageWriter : BaseBlockWriter
	{
		public OutputVariableStorageWriter(TextWriter writer) : base(writer)
		{
		}

		public override void WriteCode(BaseBlock block)
		{
			base.WriteCode(block);

			var outputVariable = (OutputVariableStorageBlock) block;
			Writer.WriteLine("    {0} <= {1};", outputVariable.VariableName, ProgramWriter.GetSignalName(outputVariable.Output.ConnectedSignal));
		}

		public override string GetVhdlType(BaseBlock block)
		{
			return "BLK_OUTPUT_VARIABLE_STORAGE";
		}
	}
}