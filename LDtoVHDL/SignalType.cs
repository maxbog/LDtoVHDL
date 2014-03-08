using System;

namespace LDtoVHDL
{
	public abstract class SignalType
	{
		public virtual string VhdlName
		{
			get
			{
				if (Width == 0)
					return "!!!ERROR!!!";
				if (Width == 1)
					return "STD_LOGIC";
				return string.Format("STD_LOGIC_VECTOR({0} downto 1)", Width);
			}
		}
		public abstract int Width { get; }

		protected bool Equals(SignalType other)
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((SignalType) obj);
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(SignalType lhs, SignalType rhs)
		{
			return Equals(lhs, rhs);
		}

		public static bool operator !=(SignalType lhs, SignalType rhs)
		{
			return !(lhs == rhs);
		}

		public bool IsSigned { get { return this == BuiltinType.SInt8 || this == BuiltinType.SInt16 || this == BuiltinType.SInt32; } }
		public bool IsUnsigned { get { return this == BuiltinType.UInt8 || this == BuiltinType.UInt16 || this == BuiltinType.UInt32; } }
		public bool IsInteger { get { return IsSigned || IsUnsigned; } }
	}

	public class BuiltinType : SignalType
	{
		public static readonly SignalType Boolean = new BuiltinType("Boolean", 1, "STD_LOGIC");
		public static readonly SignalType SInt8 = new BuiltinType("SInt8", 8, "SIGNED(7 to 0)");
		public static readonly SignalType SInt16 = new BuiltinType("SInt16", 16, "SIGNED(15 to 0)");
		public static readonly SignalType SInt32 = new BuiltinType("SInt32", 32, "SIGNED(31 to 0)");
		public static readonly SignalType UInt8 = new BuiltinType("UInt8", 8, "UNSIGNED(7 to 0)");
		public static readonly SignalType UInt16 = new BuiltinType("UInt16", 16, "UNSIGNED(15 to 0)");
		public static readonly SignalType UInt32 = new BuiltinType("UInt32", 32, "UNSIGNED(31 to 0)");
		public static readonly SignalType TimerOn = new BuiltinType("TON", 32, "STD_LOGIC_VECTOR(7 to 0)");
		public static readonly SignalType Time = new BuiltinType("Time", 32, "STD_LOGIC_VECTOR(7 to 0)");
		private readonly int m_width;
		private readonly string m_vhdlName;

		public override string VhdlName { get { return m_vhdlName; } }

		private BuiltinType(string name, int width, string vhdlName)
		{
			m_width = width;
			m_vhdlName = vhdlName;
			Name = name;
		}

		public override int Width
		{
			get { return m_width; }
		}

		public string Name { get; private set; }

		protected bool Equals(BuiltinType other)
		{
			return m_width == other.m_width && string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((BuiltinType) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = base.GetHashCode();
				hashCode = (hashCode*397) ^ m_width;
				hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static bool operator ==(BuiltinType lhs, BuiltinType rhs)
		{
			return Equals(lhs, rhs);
		}

		public static bool operator !=(BuiltinType lhs, BuiltinType rhs)
		{
			return !(lhs == rhs);
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public class BusType : SignalType
	{
		public SignalType BaseType { get; private set; }
		private readonly int m_busWidth;

		public BusType(SignalType baseType, int busWidth)
		{
			BaseType = baseType;
			m_busWidth = busWidth;
		}

		public override int Width
		{
			get { return BaseType.Width*m_busWidth; }
		}

		protected bool Equals(BusType other)
		{
			return m_busWidth == other.m_busWidth && Equals(BaseType, other.BaseType);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((BusType) obj);
		}

		public override int GetHashCode()
		{
			var hashCode = base.GetHashCode();
			hashCode = (hashCode*397) ^ m_busWidth;
			hashCode = (hashCode*397) ^ (BaseType != null ? BaseType.GetHashCode() : 0);
			return hashCode;
		}

		public static bool operator ==(BusType lhs, BusType rhs)
		{
			return Equals(lhs, rhs);
		}

		public static bool operator !=(BusType lhs, BusType rhs)
		{
			return !(lhs == rhs);
		}

		public override string ToString()
		{
			return string.Format("{0}[{1}]", BaseType, Width);
		}
	}
}