using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using JetBrains.Annotations;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;
using LDtoVHDL.Parsing.BlockFactories;

namespace LDtoVHDL.Parsing
{
	[Serializable]
	public class PlcOpenParserException : Exception
	{
		public PlcOpenParserException()
		{
		}

		public PlcOpenParserException(string message) : base(message)
		{
		}

		public PlcOpenParserException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected PlcOpenParserException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}

	public class PlcOpenParser
	{
		private readonly XElement m_pouRoot;

		public PlcOpenParser(XDocument xdoc)
		{
			m_pouRoot = xdoc.Descendants("pou".XName()).First();
		}

		public Program Parse()
		{
			var programName = GetEntityName();
			var environment = new Program(programName);
			CreateVariableBlocks(environment);
			CreateBlocksAndPorts(environment);
			ConnectPorts();
			return environment;
		}

		private string GetEntityName()
		{
			return (string)m_pouRoot.Attribute("name");
		}

		public static readonly Dictionary<string, SignalType> VarTypes = new Dictionary<string, SignalType>
		{
			{"BOOL", BuiltinType.Boolean},
			{"SINT", BuiltinType.SInt8},
			{"INT", BuiltinType.SInt16},
			{"DINT", BuiltinType.SInt32},
			{"USINT", BuiltinType.UInt8},
			{"UINT", BuiltinType.UInt16},
			{"UDINT", BuiltinType.UInt32},
			{"TON", BuiltinType.TimerOn},
			{"TOF", BuiltinType.TimerOff},
			{"TIME", BuiltinType.Time},
			{"CTU", BuiltinType.CounterUp},
			{"CTD", BuiltinType.CounterDown}
		};

		private readonly Dictionary<XElement, Port> m_ports = new Dictionary<XElement, Port>();
		private readonly Dictionary<Point, XElement> m_xPorts = new Dictionary<Point, XElement>();
		private readonly BlockBuilder m_blockBuilder = new BlockBuilder();


		public void CreateVariableBlocks(Program env)
		{
			foreach (var variable in GetAllVariables())
			{
				var varBlock = CreateMemoryVariableBlock(variable, GetVariableName(variable.Item2));
				env.AddVariable(varBlock);
			}
		}

		private static VariableStorageBlock CreateMemoryVariableBlock(Tuple<string, XElement> variable, string varName)
		{
			if (variable.Item1 == "localVars")
				return new LocalVariableStorageBlock(varName, GetVariableType(variable.Item2), GetInitialValue(variable.Item2));
			if (variable.Item1 == "inputVars")
				return new InputVariableStorageBlock(varName, GetVariableType(variable.Item2), GetInitialValue(variable.Item2));
			if (variable.Item1 == "outputVars")
				return new OutputVariableStorageBlock(varName, GetVariableType(variable.Item2), GetInitialValue(variable.Item2));
			throw new PlcOpenParserException("Unrecognized variable type: " + variable.Item1);
		}

		private static object GetInitialValue(XElement xVar)
		{
			var xInitialValue = xVar.Element("initialValue".XName());
			if (xInitialValue == null) 
				return null;
			var xSimpleValue = xInitialValue.Element("simpleValue".XName());
			if (xSimpleValue == null) 
				return null;

			return ParseExpression((string)xSimpleValue.Attribute("value")).Item2;
		}

		private static SignalType GetVariableType(XElement varElem)
		{
			var typeTag = varElem.Descendants("type".XName()).First().Descendants().First();
			var typeName = typeTag.Name.LocalName == "derived" ? typeTag.Attribute("name").Value : typeTag.Name.LocalName;
			return VarTypes[typeName];
		}

		private static string GetVariableName(XElement varElem)
		{
			return (string)varElem.Attribute("name");
		}

		private IEnumerable<Tuple<string, XElement>> GetAllVariables()
		{
			return m_pouRoot.Descendants("interface".XName()).First().Descendants("variable".XName()).Select(xelm =>
			{
				Debug.Assert(xelm.Parent != null, "xelm.Parent != null");
				return Tuple.Create(xelm.Parent.Name.LocalName, xelm);
			});
		}

		public void ConnectPorts()
		{
			foreach (var connection in m_ports.SelectMany(GetOtherSidePorts, (port, otherSidePort) => new {Port = port.Value, OtherSidePort = otherSidePort}))
			{
				connection.Port.Connect(connection.OtherSidePort);
			}
		}

		private IEnumerable<Port> GetOtherSidePorts(KeyValuePair<XElement, Port> port)
		{
			return GetOtherSideXPorts(port.Key).Select(pkey => m_ports[pkey]);
		}

		public void CreateBlocksAndPorts(Program env)
		{
			var blocks = GetAllBlocks()
				.Select(xBlock =>
					new
					{
						XBlock = xBlock,
						Position = GetBlockPosition(xBlock),
						Block = m_blockBuilder.CreateBlock(xBlock, env)
					});

			foreach (var block in blocks)
			{
				var ports = GetAllPorts(block.XBlock)
					.Select(xPort =>
						new
						{
							XPort = xPort,
							Offset = GetPortOffset(xPort),
							Direction = xPort.Name.LocalName == "connectionPointIn" ? PortDirection.Input : PortDirection.Output,
							Name = GetPortName(xPort)
						})
					.OrderBy(port => port.Offset.Y);

				foreach (var port in ports)
				{
					var newPort = port.Direction == PortDirection.Input 
						? block.Block.CreateInputPort(port.Name) 
						: block.Block.CreateOutputPort(port.Name);
					m_ports.Add(port.XPort, newPort);
					m_xPorts.Add(block.Position + port.Offset, port.XPort);
				}
				env.BlocksWithoutRung.Add(block.Block);
			}
		}

		private static string GetPortName(XElement xPort)
		{
			Debug.Assert(xPort.Parent != null, "xPort.Parent != null");
			if (xPort.Parent.Name != "variable".XName())
				return null;
			var formalParameter = xPort.Parent.Attribute("formalParameter");
			if (formalParameter == null)
				return null;
			return (string)formalParameter;
		}

		private IEnumerable<XElement> GetOtherSideXPorts(XElement xPort)
		{
			if (!HasConnection(xPort))
				return Enumerable.Empty<XElement>();
			return GetOtherSidePositions(xPort).Select(p => m_xPorts[p]);
		}

		private static bool HasConnection(XElement xPort)
		{
			return xPort.Elements("connection".XName()).Any();
		}

		private static IEnumerable<Point> GetOtherSidePositions(XElement xPort)
		{
			var xPositions = xPort.Elements("connection".XName()).Select(conn => conn.Elements("position".XName()).Last());
			return xPositions.Select(xPosition => new Point((int)xPosition.Attribute("x"), (int)xPosition.Attribute("y")));

		}

		private static Point GetBlockPosition(XElement xBlock)
		{
			var xPosition = xBlock.Element("position".XName());
			Debug.Assert(xPosition != null, "xPosition != null");
			return new Point((int)xPosition.Attribute("x"), (int)xPosition.Attribute("y"));
		}

		private static Offset GetPortOffset(XElement xPort)
		{
			var xOffset = xPort.Element("relPosition".XName());
			Debug.Assert(xOffset != null, "xOffset != null");
			return new Offset((int)xOffset.Attribute("x"), (int)xOffset.Attribute("y"));
		}

		private IEnumerable<XElement> GetAllBlocks()
		{
			return m_pouRoot.Descendants("LD".XName()).First().Elements();
		}

		private static IEnumerable<XElement> GetAllPorts(XElement xBlock)
		{
			return xBlock.Descendants("connectionPointIn".XName()).Concat(xBlock.Descendants("connectionPointOut".XName()));
		}

		public static Tuple<SignalType, object> ParseExpression(string expression)
		{
			bool boolValue;
			if (bool.TryParse(expression, out boolValue))
				return Tuple.Create<SignalType, object>(BuiltinType.Boolean, boolValue);

			if (expression.StartsWith("BOOL#", StringComparison.InvariantCultureIgnoreCase))
				return Tuple.Create<SignalType, object>(BuiltinType.Boolean, bool.Parse(expression.Substring(5)));

			if (expression.StartsWith("DINT#", StringComparison.InvariantCultureIgnoreCase) ||
				expression.StartsWith("INT#", StringComparison.InvariantCultureIgnoreCase) ||
				expression.StartsWith("SINT#", StringComparison.InvariantCultureIgnoreCase) ||
				expression.StartsWith("UDINT#", StringComparison.InvariantCultureIgnoreCase) ||
				expression.StartsWith("UINT#", StringComparison.InvariantCultureIgnoreCase) ||
				expression.StartsWith("USINT#", StringComparison.InvariantCultureIgnoreCase))
			{
				var type = expression.Substring(0, expression.IndexOf('#'));
				return ParseIntExpression(expression.Substring(expression.IndexOf('#') + 1), VarTypes[type]);
			}

			if (expression.StartsWith("T#", StringComparison.InvariantCultureIgnoreCase) ||
				expression.StartsWith("TIME#", StringComparison.InvariantCultureIgnoreCase))
				return ParseTimeExpression(expression.Substring(expression.IndexOf('#') + 1));

			return ParseIntExpression(expression.Substring(expression.IndexOf('#') + 1), null);
		}

		private static Tuple<SignalType, object> ParseIntExpression(string intValue, SignalType varType)
		{
			intValue = intValue.Replace("_", "");
			if (intValue.StartsWith("2#"))
				return Tuple.Create<SignalType, object>(varType, Convert.ToInt64(intValue, 2));
			if (intValue.StartsWith("8#"))
				return Tuple.Create<SignalType, object>(varType, Convert.ToInt64(intValue, 8));
			if (intValue.StartsWith("16#"))
				return Tuple.Create<SignalType, object>(varType, Convert.ToInt64(intValue, 16));

			return Tuple.Create<SignalType, object>(varType, Convert.ToInt64(intValue, 10));
		}

		private static Tuple<SignalType, object> ParseTimeExpression(string timeValue)
		{
			timeValue = timeValue.Replace("_", "");
			long nanoseconds = 0;
			int currentIndex = 0;
			while (currentIndex < timeValue.Length)
			{
				int unitIdx = timeValue.IndexOfAny("abcdefghijklmnopqrstuvwxyz".ToCharArray(), currentIndex + 1);
				var currentValue = double.Parse(timeValue.Substring(currentIndex, unitIdx));
				int nextValueIndex = timeValue.IndexOfAny("0123456789.".ToCharArray(), unitIdx + 1);
				var unit = nextValueIndex == -1 ? timeValue.Substring(unitIdx) : timeValue.Substring(unitIdx, nextValueIndex - 1);
				nanoseconds += (long)(currentValue * Multiplier[unit]);
				currentIndex = nextValueIndex == -1 ? timeValue.Length : nextValueIndex;
			}
			return Tuple.Create<SignalType, object>(BuiltinType.Time, nanoseconds);
		}

		private static readonly Dictionary<string, long> Multiplier = new Dictionary<string, long>
		{
			{"ns", 1000L},
			{"us", 1000L*1000},
			{"ms", 1000L*1000*1000},
			{"s", 1000L*1000*1000*1000},
			{"m", 1000L*1000*1000*1000*60},
			{"h", 1000L*1000*1000*1000*60*60},
			{"d", 1000L*1000*1000*1000*60*60*24}
		};

	}
}