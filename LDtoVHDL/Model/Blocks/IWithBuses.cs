using System;
using System.Collections.Generic;

namespace LDtoVHDL.Model.Blocks
{
	public interface IWithBuses
	{
		IEnumerable<Tuple<IEnumerable<Port>, Port>> GetBusesSpecification();
	}
}