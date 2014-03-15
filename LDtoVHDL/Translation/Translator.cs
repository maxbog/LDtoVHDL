using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Translation.Phases;
using LDtoVHDL.TypeFinder;

namespace LDtoVHDL.Translation
{
	public class Translator
	{
		private readonly ObjectCollection<IPhase> m_phases = ObjectCollection<IPhase>.FromExecutingAssembly();

		public void Translate(Program program)
		{
			foreach (var phase in m_phases.OrderBy(phase => phase.Priority))
				phase.Go(program);
		}
	}
}