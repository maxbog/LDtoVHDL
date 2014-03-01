﻿using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL
{
	public class Rung
	{
		public Rung()
		{
			Blocks = new HashSet<BaseBlock>();
			WritingBlocks = new Dictionary<string, List<IOutVariableBlock>>();
		}

		public HashSet<BaseBlock> Blocks { get; private set; }
		public Dictionary<string, List<IOutVariableBlock>> WritingBlocks { get; private set; }

		public void AddThisRungWriters()
		{
			foreach (var currentRungVar in Blocks.OfType<IOutVariableBlock>())
			{
				var variableName = currentRungVar.VariableName;
				if (WritingBlocks.ContainsKey(variableName))
					WritingBlocks[variableName].Add(currentRungVar);
				else
					WritingBlocks.Add(variableName, new List<IOutVariableBlock> { currentRungVar });
			}
		}
	}
}