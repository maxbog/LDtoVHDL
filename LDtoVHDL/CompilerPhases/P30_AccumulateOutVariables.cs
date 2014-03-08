using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.CompilerPhases
{
	public class P30_AccumulateOutVariables : IPhase
	{
		public int Priority { get { return 30; } }
		public void Go(Environment env)
		{
			if (env.Rungs.Count < 1)
				return;

			AddRungWriters(env.Rungs[0]);

			for (int i = 1; i < env.Rungs.Count; ++i)
			{
				foreach (var previousRungWriters in env.Rungs[i - 1].WritingBlocks)
					env.Rungs[i].WritingBlocks.Add(previousRungWriters.Key, new List<IOutVariableBlock>(previousRungWriters.Value));
				AddRungWriters(env.Rungs[i]);
			}
		}

		public void AddRungWriters(Rung rung)
		{
			foreach (var currentRungVar in rung.Blocks.OfType<IOutVariableBlock>())
			{
				var variableName = currentRungVar.VariableName;
				if (rung.WritingBlocks.ContainsKey(variableName))
					rung.WritingBlocks[variableName].Add(currentRungVar);
				else
					rung.WritingBlocks.Add(variableName, new List<IOutVariableBlock> { currentRungVar });
			}
		}
	}
}