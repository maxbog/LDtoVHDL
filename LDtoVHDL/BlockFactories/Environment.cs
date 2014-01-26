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
			Variables = new Dictionary<string, MemoryVariable>();
			BlocksWithoutRung = new HashSet<BaseBlock>();
			Rungs = new List<Rung>();
		}

		public Dictionary<string, MemoryVariable> Variables { get; private set; }

		public IEnumerable<BaseBlock> AllBlocks
		{
			get { return BlocksWithoutRung.Concat(Rungs.SelectMany(rung => rung.Blocks)); }
		}
		public HashSet<BaseBlock> BlocksWithoutRung { get; private set; }

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
				BlocksWithoutRung.Add(orBlock);
			}
		}

		public List<Rung> Rungs { get; private set; }

		public void DivideBlocksIntoRungs()
		{
			foreach (var startingBlocks in LeftRail.ConnectedBlocks.Where(blk => blk.Type != BaseBlock.RIGHT_RAIL && blk.Type != BaseBlock.LEFT_RAIL))
			{
				if (Rungs.Any(rung => rung.Blocks.Contains(startingBlocks)))
					continue;
				IdentifyBlocksForRung(startingBlocks);
			}
		}

		private void IdentifyBlocksForRung(BaseBlock startingBlocks)
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
				BlocksWithoutRung.Remove(currentBlock);
				foreach (var block in currentBlock.ConnectedBlocks)
				{
					if (Rungs.Any(rung => rung.Blocks.Contains(block)))
						continue;
					blocksToProcess.Enqueue(block);
				}
			}
		}

		public void AccumulateOutVariables()
		{
			for (int i = 0; i < Rungs.Count; ++i)
			{
				if (i != 0)
				{
					foreach (var previousRungWriters in Rungs[i - 1].WritingBlocks)
						Rungs[i].WritingBlocks.Add(previousRungWriters.Key, new List<OutVariableBlock>(previousRungWriters.Value));
				}
				Rungs[i].AddThisRungWriters();
			}
		}

		public IEnumerable<Signal> AllSignals
		{
			get
			{
				return AllBlocks.SelectMany(blk => blk.Ports.Values).Select(port => port.ConnectedSignal).Where(sig => sig != null).Distinct();
			}
		}

		public void CreateSelectors()
		{
			for (int i = 0; i < Rungs.Count; ++i)
			{
				var blocks = Rungs[i].Blocks;
				var writingBlocks = i != 0
					? Rungs[i - 1].WritingBlocks
					: new Dictionary<string, List<OutVariableBlock>>();
				var createdSelectors = new Dictionary<string, VarSelector>();

				foreach (var inVarBlock in blocks.OfType<InVariableBlock>())
				{
					if(createdSelectors.ContainsKey(inVarBlock.VariableName))
						inVarBlock.MemoryInput.Connect(createdSelectors[inVarBlock.VariableName].Output);
					else if (writingBlocks.ContainsKey(inVarBlock.VariableName))
					{
						var selector = CreateSelector(
							writingBlocks[inVarBlock.VariableName], 
							Variables[inVarBlock.VariableName],
							blocks);
						createdSelectors.Add(inVarBlock.VariableName, selector);
						inVarBlock.MemoryInput.Connect(selector.Output);
					}
					else
					{
						inVarBlock.MemoryInput.Connect(Variables[inVarBlock.VariableName].Output);
					}
				}
			}

			var lastRung = Rungs.Last();
			foreach (var memoryVariable in Variables.Values)
			{
				if (lastRung.WritingBlocks.ContainsKey(memoryVariable.VariableName))
				{
					var selector = CreateSelector(lastRung.WritingBlocks[memoryVariable.VariableName], memoryVariable, BlocksWithoutRung);
					memoryVariable.Input.Connect(selector.Output);
				}
				else
				{
					memoryVariable.Input.Connect(memoryVariable.Output);
				}
			}
		}

		private static VarSelector CreateSelector(List<OutVariableBlock> writingBlocks, MemoryVariable memoryVariable,
			HashSet<BaseBlock> destinationBlocksCollection)
		{
			var selector = new VarSelector();
			var signalBus = new BusCreator(writingBlocks.Select(blk => blk.MemoryOutput));
			var controlBus = new BusCreator(writingBlocks.Select(blk => blk.WriteCondition.InputPort));
			selector.MemoryInput.Connect(memoryVariable.Output);
			selector.Inputs.Connect(controlBus.Output);
			selector.Controls.Connect(controlBus.Output);
			destinationBlocksCollection.Add(selector);
			destinationBlocksCollection.Add(signalBus);
			destinationBlocksCollection.Add(controlBus);
			return selector;
		}
	}
}
