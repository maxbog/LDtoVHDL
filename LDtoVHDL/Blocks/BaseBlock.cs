using System;
using System.Collections.Generic;
using System.Linq;

namespace LDtoVHDL.Blocks
{
	public class BaseBlock
	{
		public const string LEFT_RAIL = "leftPowerRail";
		public const string RIGHT_RAIL = "rightPowerRail";
		public const string CONTACT = "contact";
		public const string COIL = "coil";
		public const string ADD = "ADD";
		public const string POWER_OR = "_power_or";
		public const string OUT_VARIABLE = "outVariable";
		public const string IN_VARIABLE = "inVariable";
		public const string VAR_SELECTOR = "_var_selector";
		public const string MEMORY_VARIABLE = "_memory_variable";
		public const string BUS_CREATOR = "_bus_creator";

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
					: null;
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

		public virtual string VhdlDeclaration
		{
			get { return null; }
		}

		public virtual List<string> VerifyPortWidths()
		{
			return null;
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

	public class MemoryVariable : VariableBlock
	{
		public MemoryVariable(string id, string variableName, int signalWidth)
			: base(id, variableName, signalWidth, MEMORY_VARIABLE)
		{
			AddPort(new Port(PortDirection.Input, "IN"));
			AddPort(new Port(PortDirection.Input, "LOAD"));
			AddPort(new Port(PortDirection.Output, "OUT"));
		}

		public Port Input { get { return Ports["IN"]; }}
		public Port Output { get { return Ports["OUT"]; } }
		public Port Load { get { return Ports["LOAD"]; } }
	}


	public class OutVariableBlock : VariableBlock
	{
		public OutVariableBlock(string id, string variableName, int signalWidth)
			: base(id, variableName, signalWidth, OUT_VARIABLE)
		{
			AddPort(new Port(PortDirection.Output, "MEM_OUT"));
		}

		public Port Input { get { return Ports.Values.Single(port => port.Direction == PortDirection.Input); } }
		public Port MemoryOutput { get { return Ports["MEM_OUT"]; }}
		public Signal WriteCondition { get { return Input.OtherSidePorts.Single().ParentBaseBlock.EnablePort.ConnectedSignal; }}
	}

	public class InVariableBlock : VariableBlock
	{
		public InVariableBlock(string id, string variableName, int signalWidth)
			: base(id, variableName, signalWidth, IN_VARIABLE)
		{
			AddPort(new Port(PortDirection.Input, "MEM_IN"));
		}

		public Port Output { get { return Ports.Values.Single(port => port.Direction == PortDirection.Output); } }
		public Port MemoryInput { get { return Ports["MEM_IN"]; } }
	}
}
