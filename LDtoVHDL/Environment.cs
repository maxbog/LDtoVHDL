using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL
{
	public class Environment
	{
		private readonly Dictionary<string, IReadableVariable> m_readableVariables = new Dictionary<string, IReadableVariable>();
		private readonly Dictionary<string, IWritableVariable> m_writableVariables = new Dictionary<string, IWritableVariable>();
		public Environment()
		{
			BlocksWithoutRung = new HashSet<BaseBlock>();
			Rungs = new List<Rung>();
		}

		public IReadOnlyDictionary<string, IReadableVariable> ReadableVariables { get { return m_readableVariables; } }
		public IReadOnlyDictionary<string, IWritableVariable> WritableVariables { get { return m_writableVariables; } }

		public void AddVariable(MemoryVariable newVariable)
		{
			m_readableVariables.Add(newVariable.VariableName, newVariable);
			var value = newVariable as IWritableVariable;
			if(value != null)
				m_writableVariables.Add(newVariable.VariableName, value);

			BlocksWithoutRung.Add(newVariable);
		}

		public IEnumerable<BaseBlock> AllBlocks
		{
			get { return BlocksWithoutRung.Concat(Rungs.SelectMany(rung => rung.Blocks)); }
		}
		public HashSet<BaseBlock> BlocksWithoutRung { get; private set; }

		public BaseBlock LeftRail;
		public BaseBlock RightRail;

		public IEnumerable<Signal> AllCompositeSignals
		{
			get
			{
				return AllBlocks.SelectMany(blk => blk.Ports.Values)
					.Select(port => port.ConnectedSignal)
					.Where(sig => sig != null && sig.IsComposite);
			}
		}

		public List<Rung> Rungs { get; private set; }

		
		public IEnumerable<Signal> AllSignals
		{
			get
			{
				return AllBlocks.SelectMany(blk => blk.Ports.Values).Select(port => port.ConnectedSignal).Where(sig => sig != null).Distinct();
			}
		}
	}
}
