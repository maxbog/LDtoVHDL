using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Translation.Phases
{
	class P45_ConnectClockToVariables : IPhase
	{
		public int Priority { get { return 45; } }
		public void Go(Program program)
		{
			var clock = new ClockBlock("primary");
			program.BlocksWithoutRung.Add(clock);
			foreach (var variableStorageBlock in program.AllBlocks.OfType<IVariableStorageBlock>())
				variableStorageBlock.Load.Connect(clock.ClockOut);
		}
	}
}