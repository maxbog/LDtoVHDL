using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL.CompilerPhases
{
	public class P10_ReplaceCompositeSignalsWithOrs : IPhase
	{
		public int Priority { get { return 10; } }
		public void Go(Environment env)
		{
			foreach (var compositeSignal in env.AllCompositeSignals.ToList())
				ReplaceCompositeSignal(env, compositeSignal);
		}

		private static void ReplaceCompositeSignal(Environment env, Signal compositeSignal)
		{
			var orBlock = new PowerOrBlock(compositeSignal.OrredSignals);

			foreach (var signalOutputPort in compositeSignal.OutputPorts)
			{
				signalOutputPort.Disconnect();
				orBlock.Output.Connect(signalOutputPort);
			}
			env.BlocksWithoutRung.Add(orBlock);
		}
	}
}