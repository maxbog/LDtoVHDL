using System.Collections.Generic;
using System.IO;
using LDtoVHDL.Model;

namespace LDtoVHDL.VhdlWriter
{
	[WriterFor(typeof(SignalType))]
	public class SignalTypeWriter
	{
		protected readonly TextWriter Writer;

		public SignalTypeWriter(TextWriter writer)
		{
			Writer = writer;
		}

		public virtual string GetName(SignalType type)
		{
			if (type.Width == 0)
				return "!!!ERROR!!!";
			if (type.Width == 1)
				return "STD_LOGIC";
			return string.Format("STD_LOGIC_VECTOR({0} downto 1)", type.Width);
		}
	}

	[WriterFor(typeof(BuiltinType))]
	class BuiltinTypeWriter : SignalTypeWriter
	{
		public BuiltinTypeWriter(TextWriter writer) : base(writer)
		{
		}

		private static readonly Dictionary<BuiltinType, string> VhdlNames = new Dictionary<BuiltinType, string>
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

		public override string GetName(SignalType type)
		{
			return VhdlNames[(BuiltinType) type];
		}
	}
}