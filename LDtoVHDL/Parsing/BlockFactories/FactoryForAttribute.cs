using System;

namespace LDtoVHDL.Parsing.BlockFactories
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class FactoryForAttribute : Attribute
	{
		public string CreatedType { get; private set; }

		public FactoryForAttribute(string createdType)
		{
			CreatedType = createdType;
		}
	}
}