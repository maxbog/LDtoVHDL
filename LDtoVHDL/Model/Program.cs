using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.Model
{
	public class Program
	{
		public string ProgramName { get; private set; }
		private readonly Dictionary<string, IVariableStorageBlock> m_readableVariables = new Dictionary<string, IVariableStorageBlock>();
		private readonly Dictionary<string, IWritableVariableStorageBlock> m_writableVariables = new Dictionary<string, IWritableVariableStorageBlock>();
		public Program(string programName)
		{
			ProgramName = programName;
			BlocksWithoutRung = new HashSet<BaseBlock>();
			Rungs = new List<Rung>();
		}

		public IReadOnlyDictionary<string, IVariableStorageBlock> ReadableVariables { get { return m_readableVariables; } }
		public IReadOnlyDictionary<string, IWritableVariableStorageBlock> WritableVariables { get { return m_writableVariables; } }

		public void AddVariable(VariableStorageBlock newVariableStorageBlock)
		{
			m_readableVariables.Add(newVariableStorageBlock.VariableName, newVariableStorageBlock);
			var value = newVariableStorageBlock as IWritableVariableStorageBlock;
			if(value != null)
				m_writableVariables.Add(newVariableStorageBlock.VariableName, value);

			BlocksWithoutRung.Add(newVariableStorageBlock);
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
