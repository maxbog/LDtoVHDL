namespace LDtoVHDL.Blocks
{
	class ContactBlock : InVariableBlock
	{
		public new const string TYPE = "contact";

		public ContactBlock(string id, string variableName) : base(id, variableName, 1)
		{
		}

		protected override string GetNewPortName(PortDirection direction)
		{
			return direction == PortDirection.Input ? "EN" : "ENO";
		}
	}
}