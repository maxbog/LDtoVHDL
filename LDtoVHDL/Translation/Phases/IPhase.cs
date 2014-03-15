using LDtoVHDL.Model;

namespace LDtoVHDL.Translation.Phases
{
	public interface IPhase
	{
		int Priority { get; }
		void Go(Program program);
	}
}