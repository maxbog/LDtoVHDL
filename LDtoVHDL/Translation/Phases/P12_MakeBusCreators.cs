using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Translation.Phases
{
	public class P12_MakeBusCreators : IPhase
	{
		public int Priority { get { return 12; } }

		public void Go(Program program)
		{
			foreach (var block in program.AllBlocks.OfType<IWithBuses>().ToList())
			{
				foreach (var spec in block.GetBusesSpecification())
				{
					var connectedPorts = spec.Item1.Select(port => port.OtherSidePorts.Single()).ToList();
					foreach (var port in spec.Item1)
						port.Disconnect();
					var busCreator = new BusCreator(connectedPorts);
					busCreator.Output.Connect(spec.Item2);
					program.BlocksWithoutRung.Add(busCreator);
				}
			}
		}
	}
}