namespace LDtoVHDL.Model.Blocks
{
	public interface IVariableStorageBlock
	{
		Port Output { get; }
		SignalType SignalType { get; }
		string VariableName { get; }
		Port Load { get; }

	}

	public interface IWritableVariableStorageBlock : IVariableStorageBlock
	{
		Port Input { get; }
	}

	public abstract class VariableStorageBlock : VariableBlock, IVariableStorageBlock
	{
		protected VariableStorageBlock(string variableType, string variableName, SignalType signalType)
			: base(string.Format("_var_{0}_{1}", variableType, variableName), variableName, signalType)
		{
			CreateInputPort("IN");
			CreateInputPort("LOAD");
			CreateOutputPort("OUT");
		}

		public Port Input { get { return Ports["IN"]; }}
		public Port Output { get { return Ports["OUT"]; } }
		public Port Load { get { return Ports["LOAD"]; } } 

		public override bool CanComputePortTypes
		{
			get { return true; }
		}

		public override void ComputePortTypes()
		{
			Ports["IN"].SignalType = SignalType;
			Ports["OUT"].SignalType = SignalType;
			Ports["LOAD"].SignalType = BuiltinType.Boolean;
		}
	}
}