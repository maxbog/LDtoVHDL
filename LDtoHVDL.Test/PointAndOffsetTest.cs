using LDtoVHDL.Parsing;
using NUnit.Framework;

namespace LDtoHVDL.Test
{
	[TestFixture]
	public class PointAndOffsetTest
	{
		[TestCase(0,0)]
		[TestCase(1,1)]
		[TestCase(10,32)]
		[TestCase(4378,321)]
		public void Point_TwoInstancesWithSameCoordinates_AreEqual(int x, int y)
		{
			var instance1 = new Point(x,y);
			var instance2 = new Point(x,y);
			Assert.That(instance1, Is.EqualTo(instance2));
		}

		[TestCase(0, 0)]
		[TestCase(1, 1)]
		[TestCase(10, 32)]
		[TestCase(4378, 321)]
		public void Offset_TwoInstancesWithSameCoordinates_AreEqual(int x, int y)
		{
			var instance1 = new Offset(x, y);
			var instance2 = new Offset(x, y);
			Assert.That(instance1, Is.EqualTo(instance2));
		}

		[TestCase(0, 0, 0, 0, 0, 0)]
		[TestCase(0, 0, 1, 3, 1, 3)]
		[TestCase(1, 3, 0, 0, 1, 3)]
		[TestCase(4, 9, 6, 2, 10, 11)]
		public void Point_AddingOffset_AddsCoordinates(int px, int py, int ox, int oy, int sx, int sy)
		{
			var instance1 = new Point(px, py);
			var instance2 = new Offset(ox, oy);
			var actualSum = instance1 + instance2;

			var expectedSum = new Point(sx,sy);

			Assert.That(actualSum,Is.EqualTo(expectedSum));

		}

		[TestCase(0, 0, 0, 0, 0, 0)]
		[TestCase(0, 0, 1, 3, -1, -3)]
		[TestCase(1, 3, 0, 0, 1, 3)]
		[TestCase(4, 9, 6, 2, -2, 7)]
		public void Point_SubstractingPoint_SubstractsCoordinates(int px, int py, int ox, int oy, int dx, int dy)
		{
			var instance1 = new Point(px, py);
			var instance2 = new Point(ox, oy);
			var actualDiff = instance1 - instance2;

			var expectedDiff = new Offset(dx, dy);

			Assert.That(actualDiff, Is.EqualTo(expectedDiff));

		}
	}
}