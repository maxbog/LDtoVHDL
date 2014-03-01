namespace LDtoVHDL.Blocks
{
	abstract class RailBlock : BaseBlock
	{
		protected RailBlock(string id) : base(id)
		{
		}

		public override bool CanComputePortTypes
		{
			get { return true; }
		}

		public override void ComputePortTypes()
		{
			foreach (var port in Ports.Values)
				port.SignalType = BuiltinType.Boolean;
		}
	}
}