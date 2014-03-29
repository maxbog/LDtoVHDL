namespace LDtoVHDL.Model.Blocks
{
	class ClockBlock : BaseBlock
	{
		public ClockBlock(string id) : base(id)
		{
			CreateOutputPort("CLK_OUT");
		}

		public override bool CanComputePortTypes
		{
			get { return true; }
		}

		public override void ComputePortTypes()
		{
			ClockOut.SignalType = BuiltinType.Boolean;
		}

		public Port ClockOut { get { return Ports["CLK_OUT"]; }}
	}
}