using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL
{
	public class Environment
	{
		private readonly Dictionary<string, IReadableVariableBlock> m_readableVariables = new Dictionary<string, IReadableVariableBlock>();
		private readonly Dictionary<string, IWritableVariableBlock> m_writableVariables = new Dictionary<string, IWritableVariableBlock>();
		public Environment()
		{
			BlocksWithoutRung = new HashSet<BaseBlock>();
			Rungs = new List<Rung>();
		}

		public IReadOnlyDictionary<string, IReadableVariableBlock> ReadableVariables { get { return m_readableVariables; } }
		public IReadOnlyDictionary<string, IWritableVariableBlock> WritableVariables { get { return m_writableVariables; } }

		public void AddVariable(VariableStorageBlock newVariableBlock)
		{
			m_readableVariables.Add(newVariableBlock.VariableName, newVariableBlock);
			var value = newVariableBlock as IWritableVariableBlock;
			if(value != null)
				m_writableVariables.Add(newVariableBlock.VariableName, value);

			BlocksWithoutRung.Add(newVariableBlock);
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
