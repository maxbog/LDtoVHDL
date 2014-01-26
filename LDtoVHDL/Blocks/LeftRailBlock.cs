using System.Diagnostics;

namespace LDtoVHDL.Blocks
{
	class LeftRailBlock : RailBlock
	{
		public const string TYPE = "leftPowerRail";

		public LeftRailBlock(string id) : base(id)
		{
		}

		private int m_nextPortIndex = 1;
		protected override string GetNewPortName(PortDirection direction)
		{
			Debug.Assert(direction == PortDirection.Output);
			return string.Format("OUT{0}", m_nextPortIndex++);
		}
	}
}