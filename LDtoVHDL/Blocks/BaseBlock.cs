using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace LDtoVHDL.Blocks
{
	public abstract class BaseBlock
	{
		protected BaseBlock(string id)
		{
			Ports = new Dictionary<string, Port>();
			Id = id;
		}

		public virtual string Id { get; private set; }
		public Dictionary<string, Port> Ports { get; private set; }

		public virtual Port Enable
		{
			get
			{
				return Ports.ContainsKey("EN") 
					? Ports["EN"] 
					: null;
			}
		}

		public virtual Port EnableOut
		{
			get
			{
				return Ports.ContainsKey("ENO")
					? Ports["ENO"]
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

		private void AddPort(Port port)
		{
			Ports.Add(port.Name, port);
			port.ParentBaseBlock = this;
		}

		protected virtual string GetNewPortName(PortDirection direction)
		{
			return null;
		}

		public override string ToString()
		{
			return String.Format("[b.{0}]{1}", Id, GetType().Name);
		}

		public virtual IEnumerable<Tuple<string, string>> VhdlGenericMapping
		{
			get { return Enumerable.Empty<Tuple<string,string>>(); }
		}
		
		public virtual List<ValidationMessage> Validate()
		{
			return new List<ValidationMessage>();
		}

		[StringFormatMethod("format")]
		public Port CreateInputPort(string format, params object[] args)
		{
			var name = format == null ? GetNewPortName(PortDirection.Input) : string.Format(format, args);
			var port = new Port(PortDirection.Input, name);
			AddPort(port);
			return port;
		}

		[StringFormatMethod("format")]
		public Port CreateOutputPort(string format, params object[] args)
		{
			var name = format == null ? GetNewPortName(PortDirection.Output) : string.Format(format, args);
			var port = new Port(PortDirection.Output, name);
			AddPort(port);
			return port;
		}

		public abstract bool CanComputePortTypes { get; }

		public abstract void ComputePortTypes();

		public void PropagatePortWidths()
		{
			foreach (var port in Ports.Values.Where(port => port.ConnectedSignal != null))
			{
				port.ConnectedSignal.Type = port.SignalType;
				foreach (var otherSidePort in port.OtherSidePorts)
					otherSidePort.SignalType = port.SignalType;
			}
		}
	}
}
