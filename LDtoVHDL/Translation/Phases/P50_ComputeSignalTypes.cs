using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Translation.Phases
{
	public class P50_ComputeSignalTypes : IPhase
	{
		public int Priority { get { return 50; } }
		public void Go(Program program)
		{
			var toProcess = new HashSet<BaseBlock>(program.AllBlocks);
			while (ComputeTypes(toProcess))
			{ }
		}

		private static bool ComputeTypes(ICollection<BaseBlock> toProcess)
		{
			var changed = false;
			foreach (var block in toProcess.ToList().Where(block => block.CanComputePortTypes))
			{
				changed = true;
				block.ComputePortTypes();
				PropagatePortTypes(block);
				toProcess.Remove(block);
			}
			return changed;
		}

		private static void PropagatePortTypes(BaseBlock block)
		{
			foreach (var port in block.Ports.Values.Where(port => port.ConnectedSignal != null))
			{
				port.ConnectedSignal.Type = port.SignalType;
				foreach (var otherSidePort in port.OtherSidePorts)
					otherSidePort.SignalType = port.SignalType;
			}
		}
	}
}