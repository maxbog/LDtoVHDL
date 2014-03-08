using System;
using System.Collections.Generic;
using System.Linq;

namespace LDtoVHDL.Blocks
{
	public interface IReadableVariable
	{
		Port Output { get; }
		SignalType SignalType { get; }
		string VariableName { get; }
	}

	public interface IWritableVariable : IReadableVariable
	{
		Port Input { get; }
	}

	public abstract class MemoryVariable : VariableBlock, IReadableVariable
	{
		protected MemoryVariable(string variableType, string variableName, SignalType signalType)
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

	class LocalVariable : MemoryVariable, IWritableVariable
	{
		public const string TYPE = "_local_variable";
		public LocalVariable(string variableName, SignalType signalType) : base("local", variableName, signalType)
		{
		}
	}

	class InputVariable : MemoryVariable
	{
		public const string TYPE = "_input_variable";
		public InputVariable(string variableName, SignalType signalType)
			: base("input", variableName, signalType)
		{
		}

		protected override IEnumerable<Tuple<string, string>> VhdlPortMapping
		{
			get
			{
				yield return Tuple.Create(Input.Name, VariableName);
				yield return Tuple.Create(Output.Name, Output.ConnectedSignal == null ? null : Output.ConnectedSignal.VhdlName);
				yield return Tuple.Create(Load.Name, Load.ConnectedSignal == null ? null : Load.ConnectedSignal.VhdlName);
			}
		}
	}

	class OutputVariable : MemoryVariable, IWritableVariable
	{
		public const string TYPE = "_output_variable";
		public OutputVariable(string variableName, SignalType signalType)
			: base("output", variableName, signalType)
		{
		}
	}
}