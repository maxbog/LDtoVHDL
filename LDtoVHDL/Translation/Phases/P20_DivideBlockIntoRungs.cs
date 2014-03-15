using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Translation.Phases
{
	public class P20_DivideBlockIntoRungs : IPhase
	{
		public int Priority { get { return 20; } }
		public void Go(Program program)
		{
			var startingBlocks = program.LeftRail.ConnectedBlocks.Where(blk => !(blk is RailBlock));
			foreach (var startingBlock in startingBlocks)
				ProcessStartingBlock(program, startingBlock);
		}

		private void ProcessStartingBlock(Program program, BaseBlock startingBlock)
		{
			if (AlreadyInRung(program, startingBlock))
				return;
			IdentifyBlocksForRung(program, startingBlock);
		}

		private static bool AlreadyInRung(Program program, BaseBlock startingBlock)
		{
			return program.Rungs.Any(rung => rung.Blocks.Contains(startingBlock));
		}

		private void IdentifyBlocksForRung(Program program, BaseBlock startingBlocks)
		{
			var currentRung = new Rung();
			program.Rungs.Add(currentRung);
			var blocksToProcess = new Queue<BaseBlock>();
			blocksToProcess.Enqueue(startingBlocks);
			while (blocksToProcess.Count > 0)
			{
				var currentBlock = blocksToProcess.Dequeue();
				if (currentBlock is RailBlock)
					continue;
				currentRung.Blocks.Add(currentBlock);
				program.BlocksWithoutRung.Remove(currentBlock);
				EnqueueConnectedBlocks(program, currentBlock, blocksToProcess);
			}
		}

		private void EnqueueConnectedBlocks(Program program, BaseBlock currentBlock, Queue<BaseBlock> blocksToProcess)
		{
			foreach (var block in GetUnprocessedBlocks(program, currentBlock))
				blocksToProcess.Enqueue(block);
		}

		private IEnumerable<BaseBlock> GetUnprocessedBlocks(Program program, BaseBlock currentBlock)
		{
			return currentBlock.ConnectedBlocks.Where(block => program.Rungs.All(rung => !rung.Blocks.Contains(block)));
		}
	}
}