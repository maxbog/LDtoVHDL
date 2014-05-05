using System.Linq;

namespace LDtoVHDL.Model.Blocks
{
	public abstract class CounterBlock : BaseBlock, IInVariableBlock, IOutVariableBlock
	{
		protected CounterBlock(string id, string variableName)
			: base(id)
		{
			VariableName = variableName;

			CreateOutputPort("VAR_WRITE");
			CreateInputPort("VAR_READ");
		}

		public Port Input { get { return Ports["CU"]; } }
		public Port Output { get { return Ports["Q"]; } }
		public Port PresetValue { get { return Ports["PV"]; } }
		public Port CurrentValue { get { return Ports["CV"]; } }
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
			PresetValue.SignalType = BuiltinType.SInt16;
			CurrentValue.SignalType = BuiltinType.SInt16;
		}

		public string VariableName { get; private set; }

		public Port WriteCondition
		{
			get { return Input.OtherSidePorts.Single(); }
		}
	}

	class CtuBlock : CounterBlock
	{
		public CtuBlock(string id, string variableName) : base(id, variableName)
		{
		}
		public Port Reset { get { return Ports["R"]; } }

		public override void ComputePortTypes()
		{
			base.ComputePortTypes();
			Reset.SignalType = BuiltinType.Boolean;
			MemoryOutput.SignalType = BuiltinType.CounterUp;
			MemoryInput.SignalType = BuiltinType.CounterUp;
		}
	}
	class CtdBlock : CounterBlock
	{
		public CtdBlock(string id, string variableName)
			: base(id, variableName)
		{
		}

		public Port LoadValue { get { return Ports["LD"]; } }

		public override void ComputePortTypes()
		{
			base.ComputePortTypes();
			LoadValue.SignalType = BuiltinType.Boolean;
			MemoryOutput.SignalType = BuiltinType.CounterDown;
			MemoryInput.SignalType = BuiltinType.CounterDown;
		}
	}
}