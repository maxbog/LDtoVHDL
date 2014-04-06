namespace LDtoVHDL.Model.Blocks
{
	class LocalVariableStorageBlock : VariableStorageBlock, IWritableVariableStorageBlock
	{
		public const string TYPE = "local_variable";
		public LocalVariableStorageBlock(string variableName, SignalType signalType) : base("local", variableName, signalType)
		{
		}
	}
}