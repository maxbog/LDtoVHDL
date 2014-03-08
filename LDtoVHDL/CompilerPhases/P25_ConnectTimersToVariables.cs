using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.CompilerPhases
{
	public class P25_ConnectTimersToVariables : IPhase
	{
		public int Priority { get { return 25; } }
		public void Go(Environment env)
		{
		}
	}
}