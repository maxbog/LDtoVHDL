using System;
using System.Collections.Generic;
using System.Globalization;
using LDtoVHDL.Model;

namespace LDtoVHDL.VhdlWriter
{
	public class SignalTypeWriter
	{
		private static readonly Dictionary<SignalType, string> VhdlNames = new Dictionary<SignalType, string>
		{
			{BuiltinType.Boolean, "std_logic"},
			{BuiltinType.SInt8, "sint8"},
			{BuiltinType.SInt16, "sint16"},
			{BuiltinType.SInt32, "sint32"},
			{BuiltinType.UInt8, "uint8"},
			{BuiltinType.UInt16, "uint16"},
			{BuiltinType.UInt32, "uint32"},
			{BuiltinType.TimerOn, "timer_on"},
			{BuiltinType.TimerOff, "timer_off"},
			{BuiltinType.Time, "plc_time"},
			{BuiltinType.CounterUp, "counter_up"},
			{BuiltinType.CounterDown, "counter_down"}
		};

		public static string GetName(SignalType type)
		{
			var busType = type as BusType;
			if (busType != null) 
				return string.Format("{0}_vector({1} downto 0)", GetName(busType.BaseType), busType.SignalCount-1);

			return VhdlNames[type];
		}

		private static readonly Dictionary<SignalType, object> DefaultValues = new Dictionary<SignalType, object>
		{
			{BuiltinType.Boolean, false},
			{BuiltinType.SInt8, 0},
			{BuiltinType.SInt16, 0},
			{BuiltinType.SInt32, 0},
			{BuiltinType.UInt8, 0},
			{BuiltinType.UInt16, 0},
			{BuiltinType.UInt32, 0},
			{BuiltinType.TimerOn, 0},
			{BuiltinType.TimerOff, 0},
			{BuiltinType.Time, 0L},
			{BuiltinType.CounterUp, 0},
			{BuiltinType.CounterDown, 0},
			{BuiltinType.Integer, 0}
		};

		public static string GetValueConstructor(SignalType type, object value)
		{
			if (value == null)
				value = DefaultValues[type];

			if (type == BuiltinType.Boolean)
				return (bool) value ? "'1'" : "'0'";

			if (type == BuiltinType.SInt8)
				return string.Format("x\"{0:x2}\"", (sbyte)Convert.ToInt64(value));
			if (type == BuiltinType.SInt16)
				return string.Format("x\"{0:x4}\"", (short)Convert.ToInt64(value));
			if (type == BuiltinType.SInt32)
				return string.Format("x\"{0:x8}\"", (int)Convert.ToInt64(value));
			if (type == BuiltinType.UInt8)
				return string.Format("x\"{0:x2}\"", (byte)Convert.ToInt64(value));
			if (type == BuiltinType.UInt16)
				return string.Format("x\"{0:x4}\"", (ushort)Convert.ToInt64(value));
			if (type == BuiltinType.UInt32)
				return string.Format("x\"{0:x8}\"", (uint)Convert.ToInt64(value));
			if (type == BuiltinType.CounterUp || type == BuiltinType.CounterDown)
				return string.Format("x\"{0:x4}\"", (short)Convert.ToInt64(value));
			if (type == BuiltinType.Time || type == BuiltinType.TimerOn || type == BuiltinType.TimerOff)
				return string.Format("x\"{0:x16}\"", Convert.ToInt64(value));

			if (type == BuiltinType.Integer)
				return Convert.ToInt64(value).ToString(CultureInfo.InvariantCulture);

			throw new InvalidOperationException("No such type");
		}
	}
}