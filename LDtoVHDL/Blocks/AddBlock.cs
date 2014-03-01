namespace LDtoVHDL.Blocks
{
	class AddBlock : BaseBlock
	{
		public const string TYPE = "ADD";

		public AddBlock(string id) : base(id)
		{
		}

		public Port Input1 { get { return Ports["IN1"]; } }
		public Port Input2 { get { return Ports["IN2"]; } }
		public Port Output { get { return Ports["OUT"]; } }

		public override bool CanComputePortTypes
		{
			get { return Input1.SignalType != null || Input2.SignalType != null || Output.SignalType != null; }
		}

		public override void ComputePortTypes()
		{
			var variableWidth = Input1.SignalType ?? (Input2.SignalType ?? Output.SignalType);
			Input1.SignalType = Input2.SignalType = Output.SignalType = variableWidth;
			Enable.SignalType = EnableOut.SignalType = BuiltinType.Boolean;
		}
	}
}