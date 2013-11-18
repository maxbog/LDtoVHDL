namespace LDtoVHDL
{
	public struct Offset
	{
		public bool Equals(Offset other)
		{
			return X == other.X && Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Offset && Equals((Offset) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X*397) ^ Y;
			}
		}

		public readonly int X;
		public readonly int Y;

		public Offset(int x, int y) : this()
		{
			X = x;
			Y = y;
		}

		public static bool operator ==(Offset offset1, Offset offset2)
		{
			return offset1.X == offset2.X && offset1.Y == offset2.Y;
		}

		public static bool operator !=(Offset offset1, Offset offset2)
		{
			return !(offset1 == offset2);
		}
	}
}
