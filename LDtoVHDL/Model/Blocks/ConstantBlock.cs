using System.Diagnostics;
using System.Linq;

namespace LDtoVHDL.Model.Blocks
{
	public class ConstantBlock : BaseBlock
	{
		public ConstantBlock(string id, SignalType valueType, object value) : base(id)
		{
			ValueType = valueType;
			Value = value;
		}

		public SignalType ValueType { get; private set; }
		public object Value { get; set; }

		public override bool CanComputePortTypes
		{
			get
			{
				if(ValueType != null)
					return true;
				return Output.ConnectedSignal != null && Output.ConnectedSignal.Type != null;
			}
		}

		public Port Output { get { return Ports.Values.Single(port => port.Direction == PortDirection.Output); } }

		public override void ComputePortTypes()
		{
			ValueType = ValueType ?? Output.ConnectedSignal.Type;
			Output.SignalType = ValueType;
		}

		protected override string GetNewPortName(PortDirection direction)
		{
			Debug.Assert(direction == PortDirection.Output);
			return "OUT";
		}
	}
}