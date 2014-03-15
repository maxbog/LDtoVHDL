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
		private readonly XDocument m_xdoc;

		public PlcOpenParser(XDocument xdoc)
		{
			m_xdoc = xdoc;
		}

		public Program Parse()
		{
			var environment = new Program();
			CreateVariableBlocks(environment);
			CreateBlocksAndPorts(environment);
			ConnectPorts();
			return environment;
		}

		private static readonly Dictionary<string, SignalType> VarTypes = new Dictionary<string, SignalType>
		{
			{"BOOL", BuiltinType.Boolean},
			{"SINT", BuiltinType.SInt8},
			{"INT", BuiltinType.SInt16},
			{"DINT", BuiltinType.SInt32},
			{"USINT", BuiltinType.UInt8},
			{"UINT", BuiltinType.UInt16},
			{"UDINT", BuiltinType.UInt32},
			{"TON", BuiltinType.TimerOn}
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
				return new LocalVariableStorageBlock(varName, GetVariableType(variable.Item2));
			if (variable.Item1 == "inputVars")
				return new InputVariableStorageBlock(varName, GetVariableType(variable.Item2));
			if (variable.Item1 == "outputVars")
				return new OutputVariableStorageBlock(varName, GetVariableType(variable.Item2));
			throw new PlcOpenParserException("Unrecognized variable type: " + variable.Item1);
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
			return m_xdoc.Descendants("interface".XName()).First().Descendants("variable".XName()).Select(xelm =>
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
			Debug.Assert(m_xdoc.Root != null, "m_xdoc.Root != null");
			return m_xdoc.Root.Descendants("LD".XName()).First().Elements();
		}

		private static IEnumerable<XElement> GetAllPorts(XElement xBlock)
		{
			return xBlock.Descendants("connectionPointIn".XName()).Concat(xBlock.Descendants("connectionPointOut".XName()));
		}
	}
}