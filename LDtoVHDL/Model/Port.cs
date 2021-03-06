﻿using System;
using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Model
{
	public enum PortDirection
	{
		Input,
		Output
	}

	public class Port
	{
		private static int _nextId;
		private SignalType m_signalType;

		public Port(PortDirection direction, string name)
		{
			Direction = direction;
			Id = _nextId++;
			Name = string.IsNullOrEmpty(name) ? string.Format("port_{0}", Id) : name;
			SignalType = null;

		}

		public int Id { get; private set; }
		public Signal ConnectedSignal { get; private set; }
		public PortDirection Direction { get; private set; }
		public BaseBlock ParentBlock { get; set; }
		
		public IEnumerable<Port> OtherSidePorts
		{
			get
			{
				if (ConnectedSignal == null)
					return Enumerable.Empty<Port>();
				if (ConnectedSignal.IsComposite)
					return ConnectedSignal.OrredSignals.Select(s => s.InputPort);
				if (Direction == PortDirection.Input)
					return Enumerable.Repeat(ConnectedSignal.InputPort, 1);
				return ConnectedSignal.OutputPorts;
			}
		}
		public string Name { get; internal set; }

		public SignalType SignalType
		{
			get { return m_signalType; }
			set
			{
				if(m_signalType != null && value != m_signalType)
					throw new InvalidOperationException("Incompatible port types");
				m_signalType = value;
			}
		}

		public void Connect(Port otherPort)
		{
			if (otherPort.Direction == Direction)
				throw new ArgumentException("cannot connect to a port with the same direction", "otherPort");

			if (Direction != PortDirection.Input)
			{
				otherPort.Connect(this);
				return;
			}
			if (otherPort.ConnectedSignal == null)
				otherPort.ConnectedSignal = new Signal { InputPort = otherPort };

			if (ConnectedSignal == null)
			{
				ConnectedSignal = otherPort.ConnectedSignal;
				otherPort.ConnectedSignal.OutputPorts.Add(this);
			}
			else if (ConnectedSignal.IsComposite)
			{
				ConnectedSignal = new Signal(ConnectedSignal.OrredSignals.Concat(Enumerable.Repeat(otherPort.ConnectedSignal, 1)));
				if(ConnectedSignal.OutputPorts.IndexOf(this) == -1)
					ConnectedSignal.OutputPorts.Add(this);
				foreach (var orredSignal in ConnectedSignal.OrredSignals)
					orredSignal.OutputPorts.Remove(this);
			}
			else
			{
				ConnectedSignal = new Signal(new[] { ConnectedSignal, otherPort.ConnectedSignal });

				if (ConnectedSignal.OutputPorts.IndexOf(this) == -1)
					ConnectedSignal.OutputPorts.Add(this);
				foreach (var orredSignal in ConnectedSignal.OrredSignals)
					orredSignal.OutputPorts.Remove(this);
			}
		}

		public override string ToString()
		{
			return String.Format("[p.{0}]{1} ({2}): {3}", Id, Name, Direction, ConnectedSignal);
		}

		public void Disconnect()
		{
			if (ConnectedSignal != null)
			{
				ConnectedSignal.Disconnect(this);
				ConnectedSignal = null;
			}
		}
	}
}
