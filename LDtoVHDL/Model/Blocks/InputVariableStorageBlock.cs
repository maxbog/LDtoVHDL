namespace LDtoVHDL.Model.Blocks
{
	class InputVariableStorageBlock : VariableStorageBlock
	{
		public const string TYPE = "_input_variable";
		public InputVariableStorageBlock(string variableName, SignalType signalType)
			: base("input", variableName, signalType)
		{
		}
	}
}