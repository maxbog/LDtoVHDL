using System;
using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Translation.Phases
{
	class P0_IdentifyRails : IPhase
	{
		public int Priority { get { return 0; } }
		public void Go(Program program)
		{
			if (program.AllBlocks.OfType<LeftRailBlock>().Count() > 1)
				throw new InvalidOperationException("Only one left rail is allowed");

			if (program.AllBlocks.OfType<RightRailBlock>().Count() > 1)
				throw new InvalidOperationException("Only one right rail is allowed");

			program.LeftRail = program.AllBlocks.OfType<RightRailBlock>().First();
			program.RightRail = program.AllBlocks.OfType<RightRailBlock>().First();
		}
	}
}
