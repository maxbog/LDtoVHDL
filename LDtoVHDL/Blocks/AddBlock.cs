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

		public override bool CanComputePortWidths
		{
			get { return Input1.Width != 0 || Input2.Width != 0 || Output.Width != 0; }
		}

		public override void ComputePortWidths()
		{
			var variableWidth = Input1.Width != 0 ? Input1.Width : (Input2.Width != 0 ? Input2.Width : Output.Width);
			Input1.Width = Input2.Width = Output.Width = variableWidth;
			Enable.Width = EnableOut.Width = 1;
		}
	}
}