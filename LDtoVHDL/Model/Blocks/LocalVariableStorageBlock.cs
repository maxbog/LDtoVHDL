namespace LDtoVHDL.Model.Blocks
{
	class LocalVariableStorageBlock : VariableStorageBlock, IWritableVariableStorageBlock
	{
		public const string TYPE = "_local_variable";
		public LocalVariableStorageBlock(string variableName, SignalType signalType) : base("local", variableName, signalType)
		{
		}
	}
}