using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL
{
	public class Rung
	{
		public Rung()
		{
			Blocks = new HashSet<BaseBlock>();
			AccumulatedOutVariables = new Dictionary<string, List<OutVariableBlock>>();
		}

		public HashSet<BaseBlock> Blocks { get; private set; }
		public Dictionary<string, List<OutVariableBlock>> AccumulatedOutVariables { get; private set; }

		public void AccumulateVariables()
		{
			foreach (var currentRungVar in Blocks.OfType<OutVariableBlock>())
			{
				if (AccumulatedOutVariables.ContainsKey(currentRungVar.VariableName))
					AccumulatedOutVariables[currentRungVar.VariableName].Add(currentRungVar);
				else
					AccumulatedOutVariables.Add(currentRungVar.VariableName, new List<OutVariableBlock> {currentRungVar});
			}
		}
	}
}