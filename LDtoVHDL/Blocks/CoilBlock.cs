namespace LDtoVHDL.Blocks
{
	public class CoilBlock : OutVariableBlock
	{
		public new const string TYPE = "coil";
		public CoilBlock(string id, string variableName)
			: base(id, variableName, 1)
		{
		}
		public override Signal WriteCondition { get { return Enable.ConnectedSignal; } }

		protected override string GetNewPortName(PortDirection direction)
		{
			return direction == PortDirection.Input ? "EN" : "ENO";
		}
	}
}