using System.IO;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(OutputVariableStorageBlock))]
	class OutputVariableStorageWriter : VariableStorageWriter
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
	}
}