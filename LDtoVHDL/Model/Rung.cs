using System.Collections.Generic;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Model
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
	}
}