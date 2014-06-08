using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Translation.Phases
{
	class P42_RemoveInOutVariableBlocks : IPhase
	{
		public int Priority { get { return 42; } }
		public void Go(Program program)
		{
			foreach (var inVariableBlock in program.AllBlocks.OfType<InVariableBlock>().Where(blk => blk.GetType() == typeof(InVariableBlock)))
			{
				var outputConnectedPorts = inVariableBlock.Output.OtherSidePorts.ToList();
				var inputConnectedPort = inVariableBlock.MemoryInput.OtherSidePorts.Single();
				inVariableBlock.Output.Disconnect();
				inVariableBlock.MemoryInput.Disconnect();
				foreach (var outputConnectedPort in outputConnectedPorts)
				{
					outputConnectedPort.Connect(inputConnectedPort);
				}
			}

			foreach (var outVariableBlock in program.AllBlocks.OfType<OutVariableBlock>().Where(blk => blk.GetType() == typeof(OutVariableBlock)))
			{
				outVariableBlock.ComputeWriteCondition();
				var inputConnectedPort = outVariableBlock.Input.OtherSidePorts.Single();
				var outputConnectedPort = outVariableBlock.MemoryOutput.OtherSidePorts.Single();
				outputConnectedPort.Disconnect();
				outVariableBlock.Input.Disconnect();
				outVariableBlock.MemoryOutput.Disconnect();
				inputConnectedPort.Connect(outputConnectedPort);
			}
		}
	}
}