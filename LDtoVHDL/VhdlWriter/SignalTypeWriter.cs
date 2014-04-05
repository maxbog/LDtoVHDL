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
			{BuiltinType.Time, "time"}
		};

		public static string GetName(SignalType type)
		{
			var busType = type as BusType;
			if (busType != null) 
				return string.Format("{0}_vector({1} downto 0)", GetName(busType.BaseType), busType.SignalCount-1);

			return VhdlNames[type];
		}

		public static string GetValueConstructor(SignalType type, object value)
		{
			if (type == BuiltinType.Boolean)
				return (bool) value ? "1" : "0";

			if (type == BuiltinType.SInt8)
				return string.Format("to_signed({0},8)", (sbyte)(long)value);
			if (type == BuiltinType.SInt16)
				return string.Format("to_signed({0},16)", (short)(long)value);
			if (type == BuiltinType.SInt32)
				return string.Format("to_signed({0},32)", (int)(long)value);
			if (type == BuiltinType.UInt8)
				return string.Format("to_unsigned({0},8)", (byte)(long)value);
			if (type == BuiltinType.UInt16)
				return string.Format("to_unsigned({0},16)", (ushort)(long)value);
			if (type == BuiltinType.UInt32)
				return string.Format("to_unsigned({0},32)", (uint)(long)value);

			if (type == BuiltinType.Time)
			{
				var ts = (TimeSpan)value;
				return string.Format("to_unsigned({0},32)", (uint)(ts.Ticks/10L));
			}

			throw new InvalidOperationException("No such type");
		}
	}
}