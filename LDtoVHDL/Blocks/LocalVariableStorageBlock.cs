namespace LDtoVHDL.Blocks
{
	class LocalVariableStorageBlock : VariableStorageBlock, IWritableVariableBlock
	{
		public const string TYPE = "_local_variable";
		public LocalVariableStorageBlock(string variableName, SignalType signalType) : base("local", variableName, signalType)
		{
		}
	}
}