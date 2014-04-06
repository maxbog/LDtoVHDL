namespace LDtoVHDL.Model.Blocks
{
	class OutputVariableStorageBlock : VariableStorageBlock, IWritableVariableStorageBlock
	{
		public const string TYPE = "output_variable";
		public OutputVariableStorageBlock(string variableName, SignalType signalType, object initialValue)
			: base("output", variableName, signalType, initialValue)
		{
		}
	}
}