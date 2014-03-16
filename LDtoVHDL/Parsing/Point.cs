using System.Diagnostics;

namespace LDtoVHDL.Parsing
{
	[DebuggerDisplay("({X},{Y})")]
	public struct Point
	{
		public readonly int X;
		public readonly int Y;

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		public static Point operator+(Point point, Offset offset)
		{
			return new Point (point.X + offset.X, point.Y + offset.Y);
		}

		public static Offset operator -(Point point1, Point point2)
		{
			return new Offset (point1.X - point2.X, point1.Y - point2.Y);
		}

		public bool Equals(Point other)
		{
			return X == other.X && Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Point && Equals((Point) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X*397) ^ Y;
			}
		}

		public static bool operator ==(Point lhs, Point rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Point lhs, Point rhs)
		{
			return !(lhs == rhs);
		}

		public override string ToString()
		{
			return string.Format("({0},{1})", X, Y);
		}
	}
}
