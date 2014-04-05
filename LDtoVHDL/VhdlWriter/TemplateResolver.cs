using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LDtoVHDL.VhdlWriter
{
	class TemplateResolver
	{
		private readonly Dictionary<string, string> m_templates = new Dictionary<string, string>();
		public TemplateResolver(string baseDirectory)
		{
			foreach (var file in Directory.EnumerateFiles(baseDirectory, "*.template", SearchOption.AllDirectories))
			{
				var templateName = MakeRelativePath(baseDirectory + Path.DirectorySeparatorChar, file);
				templateName = templateName.Replace(Path.DirectorySeparatorChar, '/');
				templateName = templateName.Remove(templateName.Length - ".template".Length);
				m_templates[templateName] = File.ReadAllText(file);
			}
		}

		public string GetWithReplacements(string templateName, IDictionary<string, string> replacements = null)
		{
			if (replacements == null)
				return m_templates[templateName];
			return replacements.Aggregate(m_templates[templateName], (current, kv) => current.Replace("#" + kv.Key + "#", kv.Value));
		}

		public static String MakeRelativePath(String fromPath, String toPath)
		{
			if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
			if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

			var fromUri = new Uri(fromPath);
			var toUri = new Uri(toPath);

			if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if (toUri.Scheme.ToUpperInvariant() == "FILE")
			{
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}

			return relativePath;
		}
	}
}
