﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LDtoVHDL.Model;
using LDtoVHDL.Model.Blocks;

namespace LDtoVHDL.VhdlWriter
{
	public abstract class BaseBlockWriter
	{
		protected readonly TemplateResolver TemplateResolver;
		protected BaseBlockWriter(TemplateResolver templateResolver)
		{
			TemplateResolver = templateResolver;
		}

		public virtual void WriteDeclaration(TextWriter writer, BaseBlock block)
		{

		}
		
		public virtual void WriteCode(TextWriter writer, BaseBlock block)
		{
			writer.WriteLine("    {0}: {1} {2}port map ({3});", GetName(block), GetVhdlType(block), GetGenericMappingString(block), GetPortMappingStiring(block));
		}

		public virtual string GetDefinition(BaseBlock block)
		{
			return null;
		}

		private string GetPortMappingStiring(BaseBlock block)
		{
			return string.Join(", ", GetPortMapping(block).Select(GetSingleMappingString));
		}

		private string GetGenericMappingString(BaseBlock block)
		{
			var genericMapping = string.Join(", ", GetGenericMapping(block).Select(GetSingleMappingString));
			return genericMapping != "" ? string.Format("generic map({0}) ", genericMapping) : "";
		}

		private static string GetSingleMappingString(Tuple<string, string> mapping)
		{
			return string.Format("{0} => {1}", mapping.Item1, mapping.Item2 ?? "open");
		}

		public abstract string GetVhdlType(BaseBlock block);

		protected virtual string GetName(BaseBlock block)
		{
			if(block is InternalBlock)
				return string.Format("internal_block_{0}", block.Id);
			return string.Format("block_{0}", block.Id);
		}

		protected virtual IEnumerable<Tuple<string, string>> GetGenericMapping(BaseBlock block)
		{
			return Enumerable.Empty<Tuple<string,string>>();
		}

		protected virtual IEnumerable<Tuple<string, string>> GetPortMapping(BaseBlock block)
		{
			return block.Ports.Select(port => Tuple.Create(port.Key, GetSignalName(port.Value)));
		}

		public virtual string GetComponentReference(BaseBlock block)
		{
			return null;
		}

		public static string PrepareTemplateForOutput(string inputTemplate)
		{
			var trimmedTemplate = inputTemplate.Trim().Replace("\r\n", "\n");
			return PrependIndentation(trimmedTemplate);
		}

		private static string PrependIndentation(string trimmedTemplate)
		{
			return "    " + trimmedTemplate.Replace("\n", "\n    ");
		}

		protected static string MakeTypedName(string untypedName, SignalType signalType)
		{
			return untypedName + "_" + SignalTypeWriter.GetName(signalType);
		}

		protected static string GetSignalName(Port input1)
		{
			return input1.ConnectedSignal == null ? null : ProgramWriter.GetSignalName(input1.ConnectedSignal);
		}
	}
}