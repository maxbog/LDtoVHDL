using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LDtoVHDL
{
	public class Signal
	{
		public string Name { get; internal set; }

		public SignalType Type
		{
			get { return m_type; }
			set
			{
				if (m_type != null && value != m_type)
					throw new InvalidOperationException("Incompatible port widths");
				m_type = value;
			}
		}

		private static int _nextId;
		private readonly IEnumerable<Signal> m_orredSignals;
		private SignalType m_type;

		public Signal()
		{
			Hash = _nextId.ToString(CultureInfo.InvariantCulture);
			++_nextId;
			IsComposite = false;
			OutputPorts = new List<Port>();
		}

		public Signal(IEnumerable<Signal> orredSignals)
		{
			m_orredSignals = orredSignals;
			Hash = ComputeHash(m_orredSignals);
			IsComposite = true;
			OutputPorts = new List<Port>();
		}

		private static string ComputeHash(IEnumerable<Signal> orredSignals)
		{
			return string.Join(":", orredSignals.Select(s => s.Hash));
		}

		public bool IsComposite { get; private set; }

		public IEnumerable<Signal> OrredSignals
		{
			get
			{
				if (!IsComposite)
					throw new InvalidOperationException("cannot get orred signals on non-composite signal");
				return m_orredSignals;
			}
		}

		public Port InputPort { get; set; }
		public IList<Port> OutputPorts { get; private set; }

		public string Hash { get; private set; }

		public IEnumerable<Port> ConnectedPorts
		{
			get
			{
				return OutputPorts.Concat(IsComposite ? OrredSignals.Select(s => s.InputPort) : Enumerable.Repeat(InputPort, 1));
			}
		}

		public override string ToString()
		{
			return string.Format("[s.{0}/{1}]",Hash,Type);
		}

		public string VhdlName { get { return string.Format("signal_{0}", Hash); }}

		public string VhdlDeclaration
		{
			get { return string.Format("signal {0} : {1};", VhdlName, Type.VhdlName); }
		}
	}
}