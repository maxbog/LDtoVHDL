using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.BlockFactories;
using LDtoVHDL.Blocks;
using Environment = LDtoVHDL.BlockFactories.Environment;

namespace LDtoVHDL
{
	public class PlcOpenParser
	{
		private readonly XDocument m_xdoc;

		public PlcOpenParser(XDocument xdoc)
		{
			m_xdoc = xdoc;
		}

		public Environment Parse()
		{
			var environment = new Environment();
			CreateVariableBlocks(environment);
			CreateBlocksAndPorts(environment);
			ConnectPorts();
			return environment;
		}

		private static readonly Dictionary<string, int> VarWidths = new Dictionary<string, int>
		{
			{"BOOL", 1},
			{"SINT", 8},
			{"INT", 16},
			{"DINT", 32},
			{"USINT", 8},
			{"UINT", 16},
			{"UDINT", 32}
		};

		private readonly Dictionary<XElement, Port> m_ports = new Dictionary<XElement, Port>();
		private readonly Dictionary<Point, XElement> m_xPorts = new Dictionary<Point, XElement>();
		private readonly BlockBuilder m_blockBuilder = new BlockBuilder();


		public void CreateVariableBlocks(Environment env)
		{
			foreach (var variable in GetAllVariables())
			{
				if (variable.Item1 == "localVars")
				{
					var varName = GetVariableName(variable.Item2);
					var varBlock = new MemoryVariable("_local_" + varName, varName, GetVariableWidth(variable.Item2));
					env.Variables.Add(varName, varBlock);
					env.BlocksWithoutRung.Add(varBlock);
				}
			}
		}

		private static int GetVariableWidth(XElement varElem)
		{
			var typeName = varElem.Descendants("type".XName()).First().Descendants().First().Name.LocalName;
			return VarWidths[typeName];
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

		public void CreateBlocksAndPorts(Environment env)
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
							Port = new Port(xPort.Name.LocalName == "connectionPointIn" ? PortDirection.Input : PortDirection.Output, GetPortName(xPort))
						})
					.OrderBy(port => port.Offset.Y);

				foreach (var port in ports)
				{
					m_ports.Add(port.XPort, port.Port);
					m_xPorts.Add(block.Position + port.Offset, port.XPort);
					block.Block.AddPort(port.Port);
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