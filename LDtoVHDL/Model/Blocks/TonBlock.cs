namespace LDtoVHDL.Model.Blocks
{
	class TonBlock : TimerBlock
	{
		public const string TYPE = "TON";
		public TonBlock(string id, string variableName) : base(id, variableName)
		{
		}

		public override void ComputePortTypes()
		{
			MemoryInput.SignalType = BuiltinType.TimerOn;
			MemoryOutput.SignalType = BuiltinType.TimerOn;
		}
	}
}