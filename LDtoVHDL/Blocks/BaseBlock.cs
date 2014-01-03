using System;
using System.Collections.Generic;
using System.Globalization;

namespace LDtoVHDL.Blocks
{
	public class BaseBlock
	{
		public const String LEFT_RAIL = "leftPowerRail";
		public const String RIGHT_RAIL = "rightPowerRail";
		public const String CONTACT = "contact";
		public const String COIL = "coil";
		public const String ADD = "add";
		public const String POWER_OR = "power_or";

		public BaseBlock(int id, string type)
		{
			Type = type;
			Ports = new List<Port>();
			Id = id.ToString(CultureInfo.InvariantCulture);
		}

		public virtual string Id { get; private set; }
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

	class InternalBlock : BaseBlock
	{
		public static int _nextId;
		public InternalBlock(string type)
			: base(_nextId++, type)
		{
		}

		public override string Id
		{
			get { return "_" + base.Id; }
		}
	}
}
