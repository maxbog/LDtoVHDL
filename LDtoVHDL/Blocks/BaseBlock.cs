using System;
using System.Collections.Generic;

namespace LDtoVHDL.Blocks
{
	public class BaseBlock
	{
		public const String LEFT_RAIL = "leftPowerRail";
		public const String RIGHT_RAIL = "rightPowerRail";
		public const String CONTACT = "contact";
		public const String COIL = "coil";
		public const String ADD = "add";

		public BaseBlock(int id, string type)
		{
			Type = type;
			Ports = new List<Port>();
			Id = id;
		}

		public int Id { get; private set; }
		public List<Port> Ports { get; private set; }
		public string Type { get; private set; }

		public void AddPort(Port port)
		{
			Ports.Add(port);
			port.ParentBaseBlock = this;
		}

		public override string ToString()
		{
			return String.Format("[b.{0}]{1}", Id, Type);
		}
	}
}
