using System;
using System.Collections.Generic;
using System.Reflection;

namespace LDtoVHDL.Blocks
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

		protected override string VhdlType
		{
			get
			{
				if (Output.SignalType.IsSigned)
					return "BLK_ADD_SIGNED";
				if (Output.SignalType.IsUnsigned)
					return "BLK_ADD_UNSIGNED";
				throw new InvalidOperationException("ADD block can only operate on integer types");
			}
		}

		public override List<ValidationMessage> Validate()
		{
			var errors = base.Validate();
			if (Output.SignalType == null || Input1.SignalType == null || Input2.SignalType == null) 
				return errors;

			if (Output.SignalType != Input2.SignalType || Output.SignalType != Input1.SignalType) 
				errors.Add(ValidationMessage.Error("All ports must have the same type. IN1: {0}, IN2: {1}, OUT: {2}", Input1.SignalType, Input2.SignalType, Output.SignalType));
			else if(!Output.SignalType.IsInteger)
				errors.Add(ValidationMessage.Error("ADD block can only operate on integer types"));

			return errors;
		}
	}
}