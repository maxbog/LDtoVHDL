namespace LDtoVHDL.Model.Blocks
{
	public abstract class EdgeDetectorBlock : InternalBlock, IInVariableBlock, IOutVariableBlock
	{
		protected EdgeDetectorBlock(string variableName)
		{
			VariableName = variableName;

			CreateInputPort("EN");
			CreateOutputPort("ENO");

			CreateOutputPort("PREV_STATE_WRITE");
			CreateOutputPort("WRITE_OCCURS");
			CreateInputPort("PREV_STATE_READ");
		}

		public Port Input { get { return Ports["EN"]; } }
		public Port Output { get { return Ports["ENO"]; } }
		public override Port Enable
		{
			get { return Input; }
		}

		public override Port EnableOut
		{
			get { return Output; }
		}

		public Port MemoryOutput { get { return Ports["PREV_STATE_WRITE"]; } }
		public Port WriteOccurs { get { return Ports["WRITE_OCCURS"]; } }
		public Port MemoryInput { get { return Ports["PREV_STATE_READ"]; } }

		public override bool CanComputePortTypes
		{
			get { return true; }
		}

		public override void ComputePortTypes()
		{
			Input.SignalType = BuiltinType.Boolean;
			Output.SignalType = BuiltinType.Boolean;
			WriteOccurs.SignalType = BuiltinType.Boolean;
			MemoryOutput.SignalType = BuiltinType.Boolean;
			MemoryInput.SignalType = BuiltinType.Boolean;
		}

		public string VariableName { get; private set; }

		public Port WriteCondition
		{
			get { return WriteOccurs; }
		}
	}

	public class PositiveEdgeDetectorBlock : EdgeDetectorBlock
	{
		public PositiveEdgeDetectorBlock(string variableName) : base(variableName)
		{
		}
	}

	public class NegativeEdgeDetectorBlock : EdgeDetectorBlock
	{
		public NegativeEdgeDetectorBlock(string variableName)
			: base(variableName)
		{
		}
	}
}