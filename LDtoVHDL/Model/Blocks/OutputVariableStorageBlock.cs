namespace LDtoVHDL.Model.Blocks
{
	class OutputVariableStorageBlock : VariableStorageBlock, IWritableVariableStorageBlock
	{
		public const string TYPE = "_output_variable";
		public OutputVariableStorageBlock(string variableName, SignalType signalType)
			: base("output", variableName, signalType)
		{
		}
	}
}