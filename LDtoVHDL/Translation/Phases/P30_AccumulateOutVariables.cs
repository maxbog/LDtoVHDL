using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Translation.Phases
{
	public class P30_AccumulateOutVariables : IPhase
	{
		public int Priority { get { return 30; } }
		public void Go(Program program)
		{
			if (program.Rungs.Count < 1)
				return;

			AddRungWriters(program.Rungs[0]);

			for (int i = 1; i < program.Rungs.Count; ++i)
			{
				foreach (var previousRungWriters in program.Rungs[i - 1].WritingBlocks)
					program.Rungs[i].WritingBlocks.Add(previousRungWriters.Key, new List<IOutVariableBlock>(previousRungWriters.Value));
				AddRungWriters(program.Rungs[i]);
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