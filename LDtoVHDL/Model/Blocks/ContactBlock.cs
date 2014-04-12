namespace LDtoVHDL.Model.Blocks
{
	public abstract class ContactBlock : BaseBlock, IInVariableBlock
	{
		public const string TYPE = "contact";

		protected ContactBlock(string id, string variableName)
			: base(id)
		{
			VariableName = variableName;
			CreateInputPort("MEM_IN");
		}

		protected override string GetNewPortName(PortDirection direction)
		{
			return direction == PortDirection.Input ? "EN" : "ENO";
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

		public string VariableName { get; private set; }
		public Port MemoryInput { get { return Ports["MEM_IN"]; } }
	}

	class NccBlock : ContactBlock
	{
		public NccBlock(string id, string variableName) : base(id, variableName)
		{
		}
	}

	class NocBlock : ContactBlock
	{
		public NocBlock(string id, string variableName) : base(id, variableName)
		{
		}
	}
}