using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LDtoVHDL.Model.Blocks
{
	class AddBlock : BaseBlock, IWithBuses
	{
		public AddBlock(string id) : base(id)
		{
			CreateInputPort("BUS_IN");
		}

		public Port InputBus { get { return Ports["BUS_IN"]; } }
		public Port Output { get { return Ports["OUT"]; } }
		public int InputsCount { get { return ArithmeticInputPorts.Count(); } }

		public override bool CanComputePortTypes
		{
			get { return InputBus.SignalType != null || Output.SignalType != null; }
		}

		public override void ComputePortTypes()
		{
			if (InputBus.SignalType != null)
			{
				var busType = InputBus.SignalType as BusType;
				Debug.Assert(busType != null, "busType != null");
				Output.SignalType = busType.BaseType;
			}
			else if (Output.SignalType != null)
			{
				InputBus.SignalType = new BusType(Output.SignalType, InputsCount);
			}

			Enable.SignalType = EnableOut.SignalType = BuiltinType.Boolean;
		}

		public override List<ValidationMessage> Validate()
		{
			var errors = base.Validate();
			if (Output.SignalType == null || Output.SignalType == null) 
				return errors;

			var busType = InputBus.SignalType as BusType;
			if (busType == null)
				errors.Add(ValidationMessage.Error("Input bus must be of bus type. Is: {0}", InputBus.SignalType));
			else if (Output.SignalType != busType.BaseType)
				errors.Add(ValidationMessage.Error("Output type and input bust base type must be the same. InputBus: {0}, Output: {1}", InputBus.SignalType, Output.SignalType));
			else if(!Output.SignalType.IsInteger)
				errors.Add(ValidationMessage.Error("ADD block can only operate on integer types"));

			return errors;
		}

		public IEnumerable<Port> ArithmeticInputPorts
		{
			get { return Ports.Where(port => port.Key.StartsWith("IN")).Select(port => port.Value); }
		}
		
		public IEnumerable<Tuple<IEnumerable<Port>, Port>> GetBusesSpecification()
		{
			yield return Tuple.Create(ArithmeticInputPorts, InputBus);
		}
	}
}