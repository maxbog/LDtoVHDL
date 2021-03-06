﻿namespace LDtoVHDL.Model.Blocks
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
		public object InitialValue { get; private set; }

		protected VariableStorageBlock(string variableType, string variableName, SignalType signalType, object initialValue)
			: base(string.Format("var_{0}_{1}", variableType, variableName), variableName, signalType)
		{
			InitialValue = initialValue;
			CreateInputPort("VAR_IN");
			CreateInputPort("LOAD");
			CreateOutputPort("VAR_OUT");
		}

		public Port Input { get { return Ports["VAR_IN"]; } }
		public Port Output { get { return Ports["VAR_OUT"]; } }
		public Port Load { get { return Ports["LOAD"]; } } 

		public override bool CanComputePortTypes
		{
			get { return true; }
		}

		public override void ComputePortTypes()
		{
			Input.SignalType = SignalType;
			Output.SignalType = SignalType;
			Load.SignalType = BuiltinType.Boolean;
		}
	}
}