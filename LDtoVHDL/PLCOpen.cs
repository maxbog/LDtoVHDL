using System.Xml.Linq;

namespace LDtoVHDL
{
	public static class PlcOpen
	{
		public static XNamespace Ns = XNamespace.Get(@"http://www.plcopen.org/xml/tc6.xsd");

		public static XName XName(this string @this)
		{
			return Ns + @this;
		}
	}
}