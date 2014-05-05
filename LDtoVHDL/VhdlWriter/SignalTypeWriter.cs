using System;
using System.Collections.Generic;
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
			{BuiltinType.Time, "time"},
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
			{BuiltinType.TimerOn, new TimeSpan(0)},
			{BuiltinType.TimerOff, new TimeSpan(0)},
			{BuiltinType.Time, new TimeSpan(0)},
			{BuiltinType.CounterUp, 0},
			{BuiltinType.CounterDown, 0}
		};

		public static string GetValueConstructor(SignalType type, object value)
		{
			if (value == null)
				value = DefaultValues[type];

			if (type == BuiltinType.Boolean)
				return (bool) value ? "'1'" : "'0'";

			if (type == BuiltinType.SInt8)
				return string.Format("to_signed({0},8)", (sbyte)Convert.ToInt64(value));
			if (type == BuiltinType.SInt16)
				return string.Format("to_signed({0},16)", (short)Convert.ToInt64(value));
			if (type == BuiltinType.SInt32)
				return string.Format("to_signed({0},32)", (int)Convert.ToInt64(value));
			if (type == BuiltinType.UInt8)
				return string.Format("to_unsigned({0},8)", (byte)Convert.ToInt64(value));
			if (type == BuiltinType.UInt16)
				return string.Format("to_unsigned({0},16)", (ushort)Convert.ToInt64(value));
			if (type == BuiltinType.UInt32)
				return string.Format("to_unsigned({0},32)", (uint)Convert.ToInt64(value));
			if (type == BuiltinType.CounterUp || type == BuiltinType.CounterDown)
				return string.Format("to_signed({0},16)", (short)Convert.ToInt64(value));

			if (type == BuiltinType.Time || type == BuiltinType.TimerOn || type == BuiltinType.TimerOff)
			{
				var ts = (TimeSpan)value;
				return string.Format("{0} us", (uint)(ts.Ticks/10L));
			}

			throw new InvalidOperationException("No such type");
		}
	}
}