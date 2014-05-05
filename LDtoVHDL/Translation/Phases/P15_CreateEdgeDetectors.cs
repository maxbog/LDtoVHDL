using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Translation.Phases
{
	public class P15_CreateEdgeDetectors : IPhase
	{
		public int Priority { get { return 15; } }
		public void Go(Program program)
		{
			foreach (var pcBlock in program.AllBlocks.OfType<PcBlock>().ToList())
			{
				var outPorts = pcBlock.EnableOut.OtherSidePorts.ToArray();
				pcBlock.EnableOut.Disconnect();
				program.AddVariable(new LocalVariableStorageBlock("ped_" + pcBlock.VariableName, BuiltinType.Boolean, false));
				var edgeDetector = new PositiveEdgeDetectorBlock("ped_" + pcBlock.VariableName);
				program.BlocksWithoutRung.Add(edgeDetector);
				edgeDetector.Input.Connect(pcBlock.EnableOut);
				foreach (var outPort in outPorts)
					outPort.Connect(edgeDetector.Output);
			}

			foreach (var ncBlock in program.AllBlocks.OfType<NcBlock>().ToList())
			{
				var outPorts = ncBlock.EnableOut.OtherSidePorts.ToArray();
				ncBlock.EnableOut.Disconnect();
				program.AddVariable(new LocalVariableStorageBlock("ned_" + ncBlock.VariableName, BuiltinType.Boolean, false));
				var edgeDetector = new NegativeEdgeDetectorBlock("ned_" + ncBlock.VariableName);
				program.BlocksWithoutRung.Add(edgeDetector);
				edgeDetector.Input.Connect(ncBlock.EnableOut);
				foreach (var outPort in outPorts)
					outPort.Connect(edgeDetector.Output);
			}
		}
	}
}