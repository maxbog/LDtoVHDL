namespace LDtoVHDL.CompilerPhases
{
	public interface IPhase
	{
		int Priority { get; }
		void Go(Environment env);
	}
}