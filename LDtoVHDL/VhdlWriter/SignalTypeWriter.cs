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
	}
}