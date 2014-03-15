namespace LDtoVHDL.Model.Blocks
{
	class VarSelector : InternalBlock
	{
		public VarSelector()
		{
			CreateOutputPort("Q");
			CreateInputPort("INS");
			CreateInputPort("CONTROL");
			CreateInputPort("MEMORY_IN");
		}

		public Port Output { get { return Ports["Q"]; } }
		public Port Controls { get { return Ports["CONTROL"]; } }
		public Port Inputs { get { return Ports["INS"]; } }
		public Port MemoryInput { get { return Ports["MEMORY_IN"]; } }
		public const string TYPE = "_var_selector";

		public override bool CanComputePortTypes
		{
			get
			{
				if (Controls.SignalType != null && Inputs.SignalType != null)
					return true;

				return (Controls.SignalType != null || Inputs.SignalType != null) && (MemoryInput.SignalType != null || Output.SignalType != null);
			}
		}

		public override void ComputePortTypes()
		{
			var inputsBus = GetBusType(Inputs);
			var controlsBus = GetBusType(Controls);
			if (Controls.SignalType != null && Inputs.SignalType != null)
				MemoryInput.SignalType = inputsBus.BaseType;
			else if (Controls.SignalType != null)
				Inputs.SignalType = new BusType(MemoryInput.SignalType, controlsBus.SignalCount);
			else
				Controls.SignalType = new BusType(BuiltinType.Boolean, inputsBus.SignalCount);
		}

		private static BusType GetBusType(Port port)
		{
			return port.SignalType is BusType ? ((BusType)port.SignalType) : null;
		}
	}
}