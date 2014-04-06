using System.Collections.Generic;

namespace LDtoVHDL.Model.Blocks
{
	class AddBlock : BaseBlock
	{
		public const string TYPE = "ADD";

		public AddBlock(string id) : base(id)
		{
		}

		public Port Input1 { get { return Ports["IN1"]; } }
		public Port Input2 { get { return Ports["IN2"]; } }
		public Port Output { get { return Ports["OUT"]; } }

		public override bool CanComputePortTypes
		{
			get { return Input1.SignalType != null || Input2.SignalType != null || Output.SignalType != null; }
		}

		public override void ComputePortTypes()
		{

			var variableWidth = Input1.SignalType ?? (Input2.SignalType ?? Output.SignalType);
			Input1.SignalType = Input2.SignalType = Output.SignalType = variableWidth;
			Enable.SignalType = EnableOut.SignalType = BuiltinType.Boolean;
		}

		public override List<ValidationMessage> Validate()
		{
			var errors = base.Validate();
			if (Output.SignalType == null || Input1.SignalType == null || Input2.SignalType == null) 
				return errors;

			if (Output.SignalType != Input2.SignalType || Output.SignalType != Input1.SignalType)
				errors.Add(ValidationMessage.Error("All ports must have the same type. A: {0}, B: {1}, Q: {2}", Input1.SignalType, Input2.SignalType, Output.SignalType));
			else if(!Output.SignalType.IsInteger)
				errors.Add(ValidationMessage.Error("ADD block can only operate on integer types"));

			return errors;
		}
	}
}