using System;

namespace LDtoVHDL.Blocks
{
	public interface IVariableBlock
	{
		string VariableName { get; }
	}

	public abstract class VariableBlock : BaseBlock
	{
		protected VariableBlock(string id, string variableName, SignalType signalType) : base(id)
		{
			VariableName = variableName;
			SignalType = signalType;
		}

		public string VariableName { get; private set; }
		public SignalType SignalType { get; private set; }

		public override string ToString()
		{
			return String.Format("{0}: {1}", base.ToString(), VariableName);
		}
	}
}