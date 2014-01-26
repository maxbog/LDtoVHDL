using System.Globalization;

namespace LDtoVHDL.Blocks
{
	public abstract class InternalBlock : BaseBlock
	{
		public static int NextId;

		protected InternalBlock()
			: base(GetNextId())
		{
		}

		public static string GetNextId()
		{
			return (NextId++).ToString(CultureInfo.InvariantCulture);
		}
	}
}