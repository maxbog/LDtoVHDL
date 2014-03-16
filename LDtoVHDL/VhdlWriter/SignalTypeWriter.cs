using System;
using System.Collections.Generic;
using LDtoVHDL.Model;

namespace LDtoVHDL.VhdlWriter
{
	public class SignalTypeWriter
	{
		private static readonly Dictionary<SignalType, string> VhdlNames = new Dictionary<SignalType, string>
		{
			{BuiltinType.Boolean, "STD_LOGIC"},
			{BuiltinType.SInt8, "SIGNED(7 to 0)"},
			{BuiltinType.SInt16, "SIGNED(15 to 0)"},
			{BuiltinType.SInt32, "SIGNED(31 to 0)"},
			{BuiltinType.UInt8, "UNSIGNED(7 to 0)"},
			{BuiltinType.UInt16, "UNSIGNED(15 to 0)"},
			{BuiltinType.UInt32, "UNSIGNED(31 to 0)"},
			{BuiltinType.TimerOn, "STD_LOGIC_VECTOR(31 to 0)"},
			{BuiltinType.Time, "STD_LOGIC_VECTOR(31 to 0)"}
		};

		public static string GetName(SignalType type)
		{
			var busType = type as BusType;
			if (busType != null)
				return string.Format("array({0} downto 0) of {1}", busType.SignalCount-1, GetName(busType.BaseType));

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