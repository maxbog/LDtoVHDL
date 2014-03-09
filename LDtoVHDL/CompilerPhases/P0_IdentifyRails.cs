using System;
using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.CompilerPhases
{
	class P0_IdentifyRails : IPhase
	{
		public int Priority { get { return 0; } }
		public void Go(Environment env)
		{
			if (env.AllBlocks.OfType<LeftRailBlock>().Count() > 1)
				throw new InvalidOperationException("Only one left rail is allowed");

			if (env.AllBlocks.OfType<RightRailBlock>().Count() > 1)
				throw new InvalidOperationException("Only one right rail is allowed");

			env.LeftRail = env.AllBlocks.OfType<RightRailBlock>().First();
			env.RightRail = env.AllBlocks.OfType<RightRailBlock>().First();
		}
	}
}
