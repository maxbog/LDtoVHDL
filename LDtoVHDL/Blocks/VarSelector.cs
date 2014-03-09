using System.Diagnostics;

namespace LDtoVHDL.Blocks
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
			var inputsBus = Inputs.SignalType is BusType ? ((BusType) Inputs.SignalType) : null;
			var controlsBus = Controls.SignalType is BusType ? ((BusType) Controls.SignalType) : null;
			if (Controls.SignalType != null && Inputs.SignalType != null)
			{
				Debug.Assert(inputsBus != null);
				Debug.Assert(controlsBus != null);
				Debug.Assert(controlsBus.BaseType == BuiltinType.Boolean);

				MemoryInput.SignalType = inputsBus.BaseType;
			}
			else if (Controls.SignalType != null)
			{
				Debug.Assert(controlsBus != null);
				Debug.Assert(controlsBus.BaseType == BuiltinType.Boolean);
				Inputs.SignalType = new BusType(MemoryInput.SignalType, controlsBus.SignalCount);
			}
			else
			{
				Debug.Assert(inputsBus != null);
				Controls.SignalType = new BusType(BuiltinType.Boolean, inputsBus.SignalCount);	
			}
		}
	}
}