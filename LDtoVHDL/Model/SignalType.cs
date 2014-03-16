namespace LDtoVHDL.Model
{
	public abstract class SignalType
	{
		public bool IsSigned { get { return this == BuiltinType.SInt8 || this == BuiltinType.SInt16 || this == BuiltinType.SInt32; } }
		public bool IsUnsigned { get { return this == BuiltinType.UInt8 || this == BuiltinType.UInt16 || this == BuiltinType.UInt32; } }
		public bool IsInteger { get { return IsSigned || IsUnsigned; } }

		protected bool Equals(SignalType other)
		{
			return false;
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
			return 0;
		}

		public static bool operator ==(SignalType left, SignalType right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SignalType left, SignalType right)
		{
			return !Equals(left, right);
		}
	}

	public class BuiltinType : SignalType
	{
		public static readonly BuiltinType Boolean = new BuiltinType("Boolean");
		public static readonly BuiltinType SInt8 = new BuiltinType("SInt8");
		public static readonly BuiltinType SInt16 = new BuiltinType("SInt16");
		public static readonly BuiltinType SInt32 = new BuiltinType("SInt32");
		public static readonly BuiltinType UInt8 = new BuiltinType("UInt8");
		public static readonly BuiltinType UInt16 = new BuiltinType("UInt16");
		public static readonly BuiltinType UInt32 = new BuiltinType("UInt32");
		public static readonly BuiltinType TimerOn = new BuiltinType("TON");
		public static readonly BuiltinType Time = new BuiltinType("Time");

		private BuiltinType(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }

		protected bool Equals(BuiltinType other)
		{
			return string.Equals(Name, other.Name);
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
				return Name != null ? Name.GetHashCode() : 0;
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

		public int SignalCount { get { return m_busWidth; } }

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
			var hashCode = m_busWidth;
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
			return string.Format("{0}[{1}]", BaseType, SignalCount);
		}
	}
}