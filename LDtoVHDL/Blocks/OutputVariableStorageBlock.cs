namespace LDtoVHDL.Blocks
{
	class OutputVariableStorageBlock : VariableStorageBlock, IWritableVariableBlock
	{
		public const string TYPE = "_output_variable";
		public OutputVariableStorageBlock(string variableName, SignalType signalType)
			: base("output", variableName, signalType)
		{
		}
	}
}