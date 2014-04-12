namespace LDtoVHDL.Model.Blocks
{
	public abstract class TimerBlock : BaseBlock, IInVariableBlock, IOutVariableBlock
	{
		protected TimerBlock(string id, string variableName)
			: base(id)
		{
			VariableName = variableName;

			CreateOutputPort("VAR_WRITE");
			CreateInputPort("VAR_READ");
		}

		public Port Input { get { return Ports["IN"]; } }
		public Port Output { get { return Ports["Q"]; } }
		public Port PresetTime { get { return Ports["PT"]; } }
		public Port ElapsedTime { get { return Ports["ET"]; } }
		public override Port Enable
		{
			get { return Input; }
		}

		public override Port EnableOut
		{
			get { return Output; }
		}

		public Port MemoryOutput { get { return Ports["VAR_WRITE"]; } }
		public Port MemoryInput { get { return Ports["VAR_READ"]; } }

		public override bool CanComputePortTypes
		{
			get { return true; }
		}

		public override void ComputePortTypes()
		{
			Input.SignalType = BuiltinType.Boolean;
			Output.SignalType = BuiltinType.Boolean;
			PresetTime.SignalType = BuiltinType.Time;
			ElapsedTime.SignalType = BuiltinType.Time;
		}

		public string VariableName { get; private set; }

		public Signal WriteCondition
		{
			get { return Input.ConnectedSignal; }
		}
	}

	class TonBlock : TimerBlock
	{
		public const string TYPE = "TON";
		public TonBlock(string id, string variableName) : base(id, variableName)
		{
		}

		public override void ComputePortTypes()
		{
			MemoryInput.SignalType = BuiltinType.TimerOn;
			MemoryOutput.SignalType = BuiltinType.TimerOn;
		}
	}

	class TofBlock : TimerBlock
	{
		public const string TYPE = "TOF";
		public TofBlock(string id, string variableName)
			: base(id, variableName)
		{
		}
		public override void ComputePortTypes()
		{
			MemoryInput.SignalType = BuiltinType.TimerOff;
			MemoryOutput.SignalType = BuiltinType.TimerOff;
		}
	}
}