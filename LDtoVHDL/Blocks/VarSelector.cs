namespace LDtoVHDL.Blocks
{
	class VarSelector : InternalBlock
	{
		public VarSelector()
		{
			CreateOutputPort("OUT");
			CreateInputPort("IN");
			CreateInputPort("CONTROL");
			CreateInputPort("MEMORY_IN");
		}

		public Port Output { get { return Ports["OUT"]; } }
		public Port Controls { get { return Ports["CONTROL"]; } }
		public Port Inputs { get { return Ports["IN"]; } }
		public Port MemoryInput { get { return Ports["MEMORY_IN"]; } }
		public const string TYPE = "_var_selector";

		public override bool CanComputePortWidths
		{
			get
			{
				if (Controls.Width != 0 && Inputs.Width != 0)
					return true;

				return (Controls.Width != 0 || Inputs.Width != 0) && (MemoryInput.Width != 0 || Output.Width != 0);
			}
		}

		public override void ComputePortWidths()
		{
			if (Controls.Width != 0 && Inputs.Width != 0)
			{
				var variableWidth = Controls.Width/Inputs.Width;
				MemoryInput.Width = Output.Width = variableWidth;
			}
			else
			{
				var variableWidth = MemoryInput.Width != 0 ? MemoryInput.Width : Output.Width;
				MemoryInput.Width = Output.Width = variableWidth;
				if (Controls.Width == 0)
				{
					Controls.Width = Inputs.Width/variableWidth;

				}
				else
				{
					Inputs.Width = Controls.Width * variableWidth;
				}
			}
		}
	}
}