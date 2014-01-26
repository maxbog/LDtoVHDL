using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LDtoVHDL.Blocks
{
	public class BaseBlock
	{
		public const String LEFT_RAIL = "leftPowerRail";
		public const String RIGHT_RAIL = "rightPowerRail";
		public const String CONTACT = "contact";
		public const String COIL = "coil";
		public const String ADD = "ADD";
		public const String POWER_OR = "_power_or";
		public const String OUT_VARIABLE = "outVariable";
		public const String IN_VARIABLE = "inVariable";
		public const String VAR_SELECTOR = "_var_selector";

		public BaseBlock(string id, string type)
		{
			Type = type;
			Ports = new Dictionary<string, Port>();
			Id = id;
		}

		public virtual string Id { get; private set; }
		public Dictionary<string, Port> Ports { get; private set; }
		public string Type { get; private set; }

		public virtual Port EnablePort
		{
			get
			{
				return Ports.ContainsKey("EN") 
					? Ports["EN"] 
					: Ports.Values.Single(port => port.Direction == PortDirection.Input);
			}
		}

		public IEnumerable<BaseBlock> ConnectedBlocks
		{
			get
			{
				return Ports.Values
					.Select(p => p.OtherSidePorts)
					.SelectMany(op => op.Select(p => p.ParentBaseBlock));
			}
		}

		public void AddPort(Port port)
		{
			Ports.Add(port.Name, port);
			port.ParentBaseBlock = this;
		}

		public override string ToString()
		{
			return String.Format("[b.{0}]{1}", Id, Type);
		}

		protected virtual IEnumerable<Tuple<string, Signal>> VhdlPortMapping
		{
			get { return Ports.Select(port => Tuple.Create(port.Key, port.Value.ConnectedSignal)); }
		}

		protected virtual string VhdlName
		{
			get { return string.Format("block_{0}", Id); }
		}

		protected virtual string VhdlType
		{
			get { return Type; }
		}

		public virtual string VhdlCode
		{
			get
			{
				var portMapping = string.Join(", ",
					VhdlPortMapping.Select(mapping => string.Format("{0} => {1}", mapping.Item1, mapping.Item2== null ? "open" : mapping.Item2.VhdlName )));
				return string.Format("{0}: {1} port map ({2});", VhdlName, VhdlType, portMapping);
			}
		}
	}

	public class VariableBlock : BaseBlock
	{
		public VariableBlock(string id, string variableName, int signalWidth, string blockName) : base(id, blockName)
		{
			VariableName = variableName;
			SignalWidth = signalWidth;
		}

		public string VariableName { get; private set; }
		public int SignalWidth { get; set; }

		public override string ToString()
		{
			return String.Format("[b.{0}]{1}: {2}", Id, Type, VariableName);
		}
	}

	public class OutVariableBlock : VariableBlock
	{
		public OutVariableBlock(string id, string variableName, int signalWidth)
			: base(id, variableName, signalWidth, OUT_VARIABLE)
		{
			AddPort(new Port(PortDirection.Output, "MEM_OUT", signalWidth));
		}

		public Port Input { get { return Ports.Values.Single(port => port.Direction == PortDirection.Input); } }
		public Port MemoryOutput { get { return Ports["MEM_OUT"]; }}
	}

	public class InVariableBlock : VariableBlock
	{
		public InVariableBlock(string id, string variableName, int signalWidth)
			: base(id, variableName, signalWidth, IN_VARIABLE)
		{
			AddPort(new Port(PortDirection.Input, "MEM_IN", signalWidth));
		}

		public Port Output { get { return Ports.Values.Single(port => port.Direction == PortDirection.Output); } }
		public Port MemoryInput { get { return Ports["MEM_IN"]; } }
	}
}
