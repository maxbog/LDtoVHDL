using System;
using System.Collections.Generic;
using System.Linq;
using LDtoVHDL.Blocks;

namespace LDtoVHDL
{
	public class Rung
	{
		public Rung()
		{
			Blocks = new HashSet<BaseBlock>();
		}

		public HashSet<BaseBlock> Blocks { get; private set; }

		public IEnumerable<Tuple<OutVariableBlock, Signal>> OutVariables
		{
			get
			{
				return Blocks
					.OfType<OutVariableBlock>()
					.Select(blk => Tuple.Create(blk, blk.Input.OtherSidePorts.Single().ParentBaseBlock.EnablePort.ConnectedSignal)); 
			}
		}
	}
}