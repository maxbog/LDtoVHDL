using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.CompilerPhases
{
	public class P50_ComputeSignalTypes : IPhase
	{
		public int Priority { get { return 50; } }
		public void Go(Environment env)
		{
			var toProcess = new HashSet<BaseBlock>(env.AllBlocks);
			while (ComputeTypes(toProcess))
			{ }
		}

		private static bool ComputeTypes(HashSet<BaseBlock> toProcess)
		{
			bool changed = false;
			foreach (var block in toProcess.ToList())
			{
				if (block.CanComputePortTypes)
				{
					changed = true;
					block.ComputePortTypes();
					block.PropagatePortWidths();
					toProcess.Remove(block);
				}
			}
			return changed;
		}
	}
}