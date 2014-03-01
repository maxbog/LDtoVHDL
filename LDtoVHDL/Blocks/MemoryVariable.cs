namespace LDtoVHDL.Blocks
{
	public class MemoryVariable : VariableBlock
	{
		public MemoryVariable(string variableName, SignalType signalType)
			: base("_var_local_"+variableName, variableName, signalType)
		{
			CreateInputPort("IN");
			CreateInputPort("LOAD");
			CreateOutputPort("OUT");
		}

		public Port Input { get { return Ports["IN"]; }}
		public Port Output { get { return Ports["OUT"]; } }
		public Port Load { get { return Ports["LOAD"]; } }
		public const string TYPE = "_memory_variable"; 

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