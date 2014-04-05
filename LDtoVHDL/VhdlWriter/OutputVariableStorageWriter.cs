using System.IO;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(OutputVariableStorageBlock))]
	class OutputVariableStorageWriter : VariableStorageWriter
	{
		public OutputVariableStorageWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		public override void WriteCode(TextWriter writer, BaseBlock block)
		{
			base.WriteCode(writer, block);
			var outputVariable = (OutputVariableStorageBlock) block;
			writer.WriteLine("    {0} <= {1};", outputVariable.VariableName, ProgramWriter.GetSignalName(outputVariable.Output.ConnectedSignal));
		}
	}
}