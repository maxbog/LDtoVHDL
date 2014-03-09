using System.Linq;
using LDtoVHDL.CompilerPhases;

namespace LDtoVHDL
{
	public class Compiler
	{
		private readonly ObjectCollection<IPhase> m_phases = ObjectCollection<IPhase>.FromExecutingAssembly();

		public void Transform(Environment env)
		{
			foreach (var phase in m_phases.OrderBy(phase => phase.Priority))
				phase.Go(env);
		}
	}
}