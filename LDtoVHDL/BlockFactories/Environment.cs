using System;
using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.BlockFactories
{
	public class Environment
	{
		public Environment()
		{
			Variables = new Dictionary<string, VariableBlock>();
			AllBlocks = new HashSet<BaseBlock>();
			Rungs = new List<Rung>();
		}

		public Dictionary<string, VariableBlock> Variables { get; private set; }
		public HashSet<BaseBlock> AllBlocks { get; private set; }

		public BaseBlock LeftRail;
		public BaseBlock RightRail;

		public void IdentifyRails()
		{
			if (AllBlocks.Count(block => block.Type == BaseBlock.LEFT_RAIL) > 1)
			{
				throw new InvalidOperationException("Only one left rail is allowed");
			}

			if (AllBlocks.Count(block => block.Type == BaseBlock.RIGHT_RAIL) > 1)
			{
				throw new InvalidOperationException("Only one right rail is allowed");
			}

			LeftRail = AllBlocks.First(block => block.Type == BaseBlock.LEFT_RAIL);
			RightRail = AllBlocks.First(block => block.Type == BaseBlock.RIGHT_RAIL);
		}

		public IEnumerable<Signal> AllCompositeSignals
		{
			get
			{
				return AllBlocks.SelectMany(blk => blk.Ports.Values)
					.Select(port => port.ConnectedSignal)
					.Where(sig => sig != null && sig.IsComposite);
			}
		}

		public void ReplaceCompositeSignalsWithOrs()
		{
			foreach (var compositeSignal in AllCompositeSignals.ToList())
			{            
				var orBlock = new PowerOrBlock();
				foreach (var orredSignal in compositeSignal.OrredSignals.Select((sig, idx) => new {Signal = sig, Index = idx}))
				{
					var port = new Port(PortDirection.Input, string.Format("IN{0}", orredSignal.Index));
					port.Connect(orredSignal.Signal.InputPort);
					orBlock.AddPort(port);
				}

				foreach (var signalOutputPort in compositeSignal.OutputPorts)
				{
					signalOutputPort.Disconnect();
					orBlock.Output.Connect(signalOutputPort);
				}
				AllBlocks.Add(orBlock);
			}
		}

		public List<Rung> Rungs { get; private set; }

		public void DivideBlocksIntoRungs()
		{
			foreach (var startingBlocks in LeftRail.ConnectedBlocks.Where(blk => blk.Type != BaseBlock.RIGHT_RAIL && blk.Type != BaseBlock.LEFT_RAIL))
			{
				if (Rungs.Any(rung => rung.Blocks.Contains(startingBlocks)))
					continue;
				IdentifyBlockForRung(startingBlocks);
			}
		}

		private void IdentifyBlockForRung(BaseBlock startingBlocks)
		{
			var currentRung = new Rung();
			Rungs.Add(currentRung);
			var blocksToProcess = new Queue<BaseBlock>();
			blocksToProcess.Enqueue(startingBlocks);
			while (blocksToProcess.Count > 0)
			{
				var currentBlock = blocksToProcess.Dequeue();
				if (currentBlock.Type == BaseBlock.RIGHT_RAIL)
					continue;
				if (currentBlock.Type == BaseBlock.LEFT_RAIL)
					continue;
				currentRung.Blocks.Add(currentBlock);
				foreach (var block in currentBlock.ConnectedBlocks)
				{
					if (Rungs.Any(rung => rung.Blocks.Contains(block)))
						continue;
					blocksToProcess.Enqueue(block);
				}
			}
		}

		public IEnumerable<Signal> AllSignals
		{
			get
			{
				return AllBlocks.SelectMany(blk => blk.Ports.Values).Select(port => port.ConnectedSignal).Where(sig => sig != null).Distinct();
			}
		}
	}
}
