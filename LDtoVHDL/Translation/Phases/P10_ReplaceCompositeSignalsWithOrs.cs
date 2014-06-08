using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Translation.Phases
{
	public class P10_ReplaceCompositeSignalsWithOrs : IPhase
	{
		public int Priority { get { return 10; } }
		public void Go(Program program)
		{
			foreach (var compositeSignal in program.AllCompositeSignals.ToList())
				ReplaceCompositeSignal(program, compositeSignal);
		}

		private static void ReplaceCompositeSignal(Program program, Signal compositeSignal)
		{
			var orBlock = new PowerOrBlock(compositeSignal.OrredSignals.Where(sig => sig.InputPort != null));

			foreach (var signalOutputPort in compositeSignal.OutputPorts.ToList())
			{
				signalOutputPort.Disconnect();
				orBlock.Output.Connect(signalOutputPort);
			}
			program.BlocksWithoutRung.Add(orBlock);
		}
	}
}