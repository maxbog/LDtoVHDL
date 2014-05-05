using System.Diagnostics;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	abstract public class VariableBlockWriter : BaseBlockWriter
	{
		protected VariableBlockWriter(TemplateResolver templateResolver) : base(templateResolver)
		{
		}

		protected override string GetName(BaseBlock block)
		{
			var variableBlock = block as IVariableBlock;
			Debug.Assert(variableBlock != null, "variableBlock != null");
			return base.GetName(block) + "_" + variableBlock.VariableName;
		}
	}
}