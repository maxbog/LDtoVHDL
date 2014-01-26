using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LDtoVHDL.Blocks
{
	public class InternalBlock : BaseBlock
	{
		public static int _nextId;
		public InternalBlock(string type)
			: base(GetNextId(), type)
		{
		}

		public static string GetNextId()
		{
			return (_nextId++).ToString(CultureInfo.InvariantCulture);
		}
	}

	class VarSelector : InternalBlock
	{
		public VarSelector() : base(VAR_SELECTOR)
		{
			AddPort();
		}

		public Port Output { get { return Ports.Values.Single(port => port.Direction == PortDirection.Output); } }

		public void AddSelection

	}

	public class PowerOrBlock : InternalBlock
	{
		public PowerOrBlock()
			: base(POWER_OR)
		{
			AddPort(new Port(PortDirection.Output, "OUT", 1));
		}

		public Port Output { get { return Ports["OUT"]; } }
		public IEnumerable<Port> Inputs { get { return Ports.Values.Where(port => port.Direction == PortDirection.Input); } }

		public override string VhdlCode
		{
			get
			{
				var inputSignals = string.Join(" or ", Inputs.Select(port => port.ConnectedSignal.VhdlName));
				return string.Format("{0} <= {1};", Output.ConnectedSignal.VhdlName, inputSignals);
			}
		}
	}
}