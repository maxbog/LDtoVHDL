namespace LDtoVHDL.Blocks
{
	public class MemoryVariable : VariableBlock
	{
		public MemoryVariable(string variableName, int signalWidth)
			: base("_var_local_"+variableName, variableName, signalWidth)
		{
			CreateInputPort("IN");
			CreateInputPort("LOAD");
			CreateOutputPort("OUT");
		}

		public Port Input { get { return Ports["IN"]; }}
		public Port Output { get { return Ports["OUT"]; } }
		public Port Load { get { return Ports["LOAD"]; } }
		public const string TYPE = "_memory_variable"; 

		public override bool CanComputePortWidths
		{
			get { return true; }
		}

		public override void ComputePortWidths()
		{
			Ports["IN"].Width = SignalWidth;
			Ports["OUT"].Width = SignalWidth;
			Ports["LOAD"].Width = 1;
		}
	}
}