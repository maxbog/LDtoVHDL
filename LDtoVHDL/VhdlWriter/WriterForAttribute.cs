using System;

namespace LDtoVHDL.VhdlWriter
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class WriterForAttribute : Attribute
	{
		public Type FormattedType { get; private set; }

		public WriterForAttribute(Type formattedType)
		{
			FormattedType = formattedType;
		}
	}
}