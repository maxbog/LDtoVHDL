using System.Diagnostics;

namespace LDtoVHDL.Blocks
{
	class RightRailBlock : RailBlock
	{
		public const string TYPE = "rightPowerRail";

		public RightRailBlock(string id)
			: base(id)
		{
		}

		private int m_nextPortIndex = 1;
		protected override string GetNewPortName(PortDirection direction)
		{
			Debug.Assert(direction == PortDirection.Input);
			return string.Format("IN{0}", m_nextPortIndex++);
		}
	}
}