using System;

namespace LDtoVHDL.BlockFactories
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class FactoryForAttribute : Attribute
	{
		public string CreatedType { get; set; }

		public FactoryForAttribute(string createdType)
		{
			CreatedType = createdType;
		}
	}
}