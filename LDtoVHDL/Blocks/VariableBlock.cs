using System;

namespace LDtoVHDL.Blocks
{
	public abstract class VariableBlock : BaseBlock
	{
		protected VariableBlock(string id, string variableName, int signalWidth) : base(id)
		{
			VariableName = variableName;
			SignalWidth = signalWidth;
		}

		public string VariableName { get; private set; }
		public int SignalWidth { get; private set; }

		public override string ToString()
		{
			return String.Format("[b.{0}]{1}: {2}", Id, VhdlType, VariableName);
		}
	}
}