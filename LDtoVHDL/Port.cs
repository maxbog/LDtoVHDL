using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LDtoVHDL.Blocks;

namespace LDtoVHDL
{
	public enum PortDirection
	{
		Input,
		Output
	}

	public class Port
	{
		private static int _nextId = 0;
		public Port()
		{
			Id = _nextId++;
			OtherSidePorts = new List<Port>();
		}

		public int Id { get; private set; }
		public List<Port> OtherSidePorts { get; private set; }
		public PortDirection Direction { get; set; }
		public BaseBlock ParentBaseBlock { get; set; }
		public string Name { get; set; }

		public void Connect(Port otherPort)
		{
			if (OtherSidePorts.Contains(otherPort))
				return;
			OtherSidePorts.Add(otherPort);
			otherPort.Connect(this);
		}

		public override string ToString()
		{
			return String.Format("[p.{0}]{1} ({2})", Id, Name, Direction);
		}
	}
}
