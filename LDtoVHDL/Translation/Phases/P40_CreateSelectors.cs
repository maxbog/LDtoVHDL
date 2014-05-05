using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Translation.Phases
{
	public class P40_CreateSelectors : IPhase
	{
		public int Priority { get { return 40; } }
		public void Go(Program program)
		{
			for (int i = 0; i < program.Rungs.Count; ++i)
			{
				var currentRungBlocks = program.Rungs[i].Blocks;
				var previousRungWritingBlocks = i != 0
					? program.Rungs[i - 1].WritingBlocks
					: new Dictionary<string, List<IOutVariableBlock>>();

				CreateSelectorsInRung(program, currentRungBlocks, previousRungWritingBlocks);
			}

			var lastRung = program.Rungs.Last();
			foreach (var memoryVariable in program.WritableVariables.Values)
			{
				if (lastRung.WritingBlocks.ContainsKey(memoryVariable.VariableName))
				{
					var selector = CreateSelector(lastRung.WritingBlocks[memoryVariable.VariableName], memoryVariable,
						program.BlocksWithoutRung);
					memoryVariable.Input.Connect(selector.Output);
				}
				else
					memoryVariable.Input.Connect(memoryVariable.Output);
			}
		}

		private static void CreateSelectorsInRung(Program program, HashSet<BaseBlock> currentRungBlocks, IReadOnlyDictionary<string, List<IOutVariableBlock>> previousRungWritingBlocks)
		{
			var createdSelectors = new Dictionary<string, VarSelector>();
			foreach (var inVarBlock in currentRungBlocks.OfType<IInVariableBlock>())
			{
				if (SelectorAlreadyCreated(createdSelectors, inVarBlock))
					ConnectSelectorToReader(inVarBlock, createdSelectors[inVarBlock.VariableName]);
				else if (VariableWrittenInPreviousRung(previousRungWritingBlocks, inVarBlock))
				{
					var selector = CreateSelectorForVariable(program, currentRungBlocks, previousRungWritingBlocks, inVarBlock,
						createdSelectors);
					ConnectSelectorToReader(inVarBlock, selector);
				}
				else
					ConnectVariableDirectly(program, inVarBlock);
			}
		}

		private static void ConnectVariableDirectly(Program env, IInVariableBlock inVarBlock)
		{
			inVarBlock.MemoryInput.Connect(env.ReadableVariables[inVarBlock.VariableName].Output);
		}

		private static void ConnectSelectorToReader(IInVariableBlock inVarBlock, VarSelector selector)
		{
			inVarBlock.MemoryInput.Connect(selector.Output);
		}

		private static VarSelector CreateSelectorForVariable(Program env, HashSet<BaseBlock> currentRungBlocks,
			IReadOnlyDictionary<string, List<IOutVariableBlock>> previousRungWritingBlocks, IInVariableBlock inVarBlock, Dictionary<string, VarSelector> createdSelectors)
		{
			var selector = CreateSelector(
				previousRungWritingBlocks[inVarBlock.VariableName],
				env.ReadableVariables[inVarBlock.VariableName],
				currentRungBlocks);
			createdSelectors.Add(inVarBlock.VariableName, selector);
			return selector;
		}

		private static bool VariableWrittenInPreviousRung(IReadOnlyDictionary<string, List<IOutVariableBlock>> previousRungWritingBlocks, IInVariableBlock inVarBlock)
		{
			return previousRungWritingBlocks.ContainsKey(inVarBlock.VariableName);
		}

		private static bool SelectorAlreadyCreated(Dictionary<string, VarSelector> createdSelectors, IInVariableBlock inVarBlock)
		{
			return createdSelectors.ContainsKey(inVarBlock.VariableName);
		}

		private static VarSelector CreateSelector(List<IOutVariableBlock> writingBlocks, IVariableStorageBlock memoryVariableStorage, HashSet<BaseBlock> destinationBlocksCollection)
		{
			var selector = new VarSelector();
			var signalBus = new BusCreator(writingBlocks.Select(blk => blk.MemoryOutput));
			var controlBus = new BusCreator(writingBlocks.Select(blk => blk.WriteCondition));
			selector.MemoryInput.Connect(memoryVariableStorage.Output);
			selector.Inputs.Connect(signalBus.Output);
			selector.Controls.Connect(controlBus.Output);
			destinationBlocksCollection.Add(selector);
			destinationBlocksCollection.Add(signalBus);
			destinationBlocksCollection.Add(controlBus);
			return selector;
		}
	}
}