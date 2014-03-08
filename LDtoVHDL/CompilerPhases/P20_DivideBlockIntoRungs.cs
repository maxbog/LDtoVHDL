using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.CompilerPhases
{
	public class P20_DivideBlockIntoRungs : IPhase
	{
		public int Priority { get { return 20; } }
		public void Go(Environment env)
		{
			var startingBlocks = env.LeftRail.ConnectedBlocks.Where(blk => !(blk is RailBlock));
			foreach (var startingBlock in startingBlocks)
			{
				if (AlreadyInRung(env, startingBlock))
					continue;
				IdentifyBlocksForRung(env, startingBlock);
			}
		}

		private static bool AlreadyInRung(Environment env, BaseBlock startingBlock)
		{
			return env.Rungs.Any(rung => rung.Blocks.Contains(startingBlock));
		}

		private void IdentifyBlocksForRung(Environment env, BaseBlock startingBlocks)
		{
			var currentRung = new Rung();
			env.Rungs.Add(currentRung);
			var blocksToProcess = new Queue<BaseBlock>();
			blocksToProcess.Enqueue(startingBlocks);
			while (blocksToProcess.Count > 0)
			{
				var currentBlock = blocksToProcess.Dequeue();
				if (currentBlock is RailBlock)
					continue;
				currentRung.Blocks.Add(currentBlock);
				env.BlocksWithoutRung.Remove(currentBlock);
				foreach (var block in currentBlock.ConnectedBlocks)
				{
					if (env.Rungs.Any(rung => rung.Blocks.Contains(block)))
						continue;
					blocksToProcess.Enqueue(block);
				}
			}
		}
	}
}