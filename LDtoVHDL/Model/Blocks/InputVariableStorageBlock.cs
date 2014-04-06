namespace LDtoVHDL.Model.Blocks
{
	class InputVariableStorageBlock : VariableStorageBlock
	{
		public const string TYPE = "input_variable";
		public InputVariableStorageBlock(string variableName, SignalType signalType, object initialValue)
			: base("input", variableName, signalType, initialValue)
		{
		}
	}
}