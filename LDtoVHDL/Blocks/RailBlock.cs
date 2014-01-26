namespace LDtoVHDL.Blocks
{
	abstract class RailBlock : BaseBlock
	{
		protected RailBlock(string id) : base(id)
		{
		}

		public override bool CanComputePortWidths
		{
			get { return true; }
		}

		public override void ComputePortWidths()
		{
			foreach (var port in Ports.Values)
			{
				port.Width = 1;
			}
		}
	}
}