using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LDtoVHDL
{
	public class Signal
	{
		public string Name { get; internal set; }

		public int Width
		{
			get { return m_width; }
			set
			{
				if (m_width != 0 && value != m_width)
					throw new InvalidOperationException("Incompatible port widths");
				m_width = value;
			}
		}

		private static int _nextId;
		private readonly IEnumerable<Signal> m_orredSignals;
		private int m_width;

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
			return string.Format("[s.{0}/{1}]",Hash,Width);
		}

		public string VhdlName { get { return string.Format("signal_{0}", Hash); }}

		public string VhdlType
		{
			get
			{
				if (Width == 0)
					return "!!! ERROR !!!";
				if (Width == 1)
					return "STD_LOGIC";
				return string.Format("STD_LOGIC_VECTOR({0} downto {1})", Width-1, 0);
			}
		}

		public string VhdlDeclaration
		{
			get { return string.Format("signal {0} : {1};", VhdlName, VhdlType); }
		}
	}
}