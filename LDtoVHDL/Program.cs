using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LDtoVHDL.BlockFactories;
using LDtoVHDL.Blocks;

namespace LDtoVHDL
{
	public class Program
	{
		static readonly XNamespace ns = XNamespace.Get(@"http://www.plcopen.org/xml/tc6.xsd");
		public static void Main(string[] args)
		{
			var xdoc = XDocument.Load(File.OpenRead(@"d:\Dokumenty\studia\praca magisterska\test_ber\plc.xml"));

			var blockBuilder = new BlockBuilder();

			CreateBlocksAndPorts(xdoc, blockBuilder);
			ConnectPorts();

			_allBlocks = new HashSet<BaseBlock>(_blocks.Values);

			IdentifyRails();

			foreach (var compositeSignal in Signal.CompositeSignals)
			{
				BaseBlock orBlock = new InternalBlock("power_or");
				var outputPort = new Port(PortDirection.Output);
				orBlock.AddPort(outputPort);
				foreach (var orredSignal in compositeSignal.Value.OrredSignals)
				{
					var port = new Port(PortDirection.Input);
					port.Connect(orredSignal.InputPort);
					orBlock.AddPort(port);
				}
				foreach (var signalOutputPort in compositeSignal.Value.OutputPorts)
				{
					signalOutputPort.Disconnect();
					outputPort.Connect(signalOutputPort);
				}
				_allBlocks.Add(orBlock);
			}

			DivideBlocksIntoRungs();

			Signal.CompositeSignals.Clear();

			foreach (var rung in _rungs)
			{
				Console.WriteLine("Rung {0}:", _rungs.IndexOf(rung));
				foreach (var block in rung)
				{
					Console.WriteLine(block);
					foreach (var port in block.Ports)
					{
						Console.WriteLine("    {0}", port);
						foreach (var otherSide in port.OtherSidePorts)
						{
							Console.WriteLine("        --- {0}", otherSide.ParentBaseBlock);
						}
					}
				}
			}
			Console.ReadKey();
		}

		private static void DivideBlocksIntoRungs()
		{
			foreach (var otherSideBlock in _leftRail.Ports.Select(p => p.OtherSidePorts).SelectMany(op => op.Select(p => p.ParentBaseBlock)))
			{
				if (otherSideBlock.Type == BaseBlock.RIGHT_RAIL)
					continue;
				if (otherSideBlock.Type == BaseBlock.LEFT_RAIL)
					continue;
				if (_rungs.Any(set => set.Contains(otherSideBlock)))
					continue;
				var currentRung = new HashSet<BaseBlock>();
				_rungs.Add(currentRung);
				var blocksToProcess = new Queue<BaseBlock>();
				blocksToProcess.Enqueue(otherSideBlock);
				while (blocksToProcess.Count > 0)
				{
					var currentBlock = blocksToProcess.Dequeue();
					if (currentBlock.Type == BaseBlock.RIGHT_RAIL)
						continue;
					if (currentBlock.Type == BaseBlock.LEFT_RAIL)
						continue;
					currentRung.Add(currentBlock);
					foreach (var block in currentBlock.Ports
						.Select(p => p.OtherSidePorts)
						.SelectMany(op => op.Select(p => p.ParentBaseBlock)))
					{
						if (_rungs.Any(set => set.Contains(block)))
							continue;
						blocksToProcess.Enqueue(block);
					}
				}
			}
		}

		private static void IdentifyRails()
		{
			if (_blocks.Values.Count(block => block.Type == BaseBlock.LEFT_RAIL) > 1)
			{
				throw new InvalidOperationException("Only one left rail is allowed");
			}

			if (_blocks.Values.Count(block => block.Type == BaseBlock.RIGHT_RAIL) > 1)
			{
				throw new InvalidOperationException("Only one right rail is allowed");
			}

			_leftRail = _blocks.Values.First(block => block.Type == BaseBlock.LEFT_RAIL);
			_rightRail = _blocks.Values.First(block => block.Type == BaseBlock.RIGHT_RAIL);
		}

		private static void ConnectPorts()
		{
			foreach (var port in _ports)
			{
				foreach (var otherSideXPort in GetOtherSideXPorts(port.Key))
				{
					port.Value.Connect(_ports[otherSideXPort]);
				}
			}
		}

		private static void CreateBlocksAndPorts(XDocument xdoc, BlockBuilder blockBuilder)
		{
			var blocks = GetAllBlocks(xdoc.Root)
				.Select(xBlock =>
					new
					{
						XBlock = xBlock,
						Position = GetBlockPosition(xBlock),
						Block = blockBuilder.CreateBlock(xBlock)
					});

			foreach (var block in blocks)
			{
				var ports = GetAllPorts(block.XBlock)
					.Select(xPort =>
						new
						{
							XPort = xPort,
							Offset = GetPortOffset(xPort),
							Port = new Port(xPort.Name.LocalName == "connectionPointIn" ? PortDirection.Input : PortDirection.Output)
							{
								Name = GetPortName(xPort)
							}
						})
					.OrderBy(port => port.Offset.Y);

				foreach (var port in ports)
				{
					_ports.Add(port.XPort, port.Port);
					_xPorts.Add(block.Position + port.Offset, port.XPort);
					block.Block.AddPort(port.Port);
				}
				_blocks.Add(block.XBlock, block.Block);
			}
		}

		private static string GetPortName(XElement xPort)
		{
			if (xPort.Parent.Name != ns + "variable")
				return null;
			var formalParameter = xPort.Parent.Attribute("formalParameter");
			if (formalParameter == null)
				return null;
			return (string)formalParameter;
		}

		private static IEnumerable<XElement> GetOtherSideXPorts(XElement xPort)
		{
			if (!HasConnection(xPort))
				return Enumerable.Empty<XElement>();
			return GetOtherSidePositions(xPort).Select(p => _xPorts[p]);
		}

		private static bool HasConnection(XElement xPort)
		{
			return xPort.Elements(ns + "connection").Any();
		}

		private static IEnumerable<Point> GetOtherSidePositions(XElement xPort)
		{
			var xPositions = xPort.Elements(ns + "connection").Select(conn => conn.Elements(ns + "position").Last());
			return xPositions.Select(xPosition => new Point((int)xPosition.Attribute("x"), (int)xPosition.Attribute("y")));

		}

		private static Point GetBlockPosition(XElement xBlock)
		{
			var xPosition = xBlock.Element(ns + "position");
			return new Point((int)xPosition.Attribute("x"), (int)xPosition.Attribute("y"));
		}

		private static Offset GetPortOffset(XElement xPort)
		{
			var xOffset = xPort.Element(ns + "relPosition");
			return new Offset((int)xOffset.Attribute("x"), (int)xOffset.Attribute("y"));
		}

		private static IEnumerable<XElement> GetAllBlocks(XElement root)
		{
			return root.Descendants(ns + "LD").First().Elements();
		}

		private static IEnumerable<XElement> GetAllPorts(XElement root)
		{
			return root.Descendants(ns + "connectionPointIn").Concat(root.Descendants(ns + "connectionPointOut"));
		}

		private static readonly Dictionary<XElement, BaseBlock> _blocks = new Dictionary<XElement, BaseBlock>();

		private static readonly Dictionary<XElement, Port> _ports = new Dictionary<XElement, Port>();

		private static readonly Dictionary<Point, XElement> _xPorts = new Dictionary<Point, XElement>();

		private static readonly List<HashSet<BaseBlock>> _rungs = new List<HashSet<BaseBlock>>();

		private static HashSet<BaseBlock> _allBlocks;
		private static BaseBlock _leftRail;
		private static BaseBlock _rightRail;

	}
}
