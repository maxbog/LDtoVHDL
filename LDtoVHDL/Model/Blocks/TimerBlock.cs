using System.Linq;

namespace LDtoVHDL.Model.Blocks
{
	public abstract class TimerBlock : BaseBlock, IInVariableBlock, IOutVariableBlock
	{
		protected TimerBlock(string id, string variableName)
			: base(id)
		{
			VariableName = variableName;

			CreateOutputPort("WRITE_CONDITION");
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
			WriteCondition.SignalType = BuiltinType.Boolean;
		}

		public string VariableName { get; private set; }

		public Port WriteCondition { get { return Ports["WRITE_CONDITION"]; } }
	}
}