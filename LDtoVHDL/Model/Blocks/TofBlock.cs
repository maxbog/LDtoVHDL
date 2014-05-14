namespace LDtoVHDL.Model.Blocks
{
	class TofBlock : TimerBlock
	{
		public const string TYPE = "TOF";
		public TofBlock(string id, string variableName)
			: base(id, variableName)
		{
		}
		public override void ComputePortTypes()
		{
			base.ComputePortTypes();
			MemoryInput.SignalType = BuiltinType.TimerOff;
			MemoryOutput.SignalType = BuiltinType.TimerOff;
		}
	}
}