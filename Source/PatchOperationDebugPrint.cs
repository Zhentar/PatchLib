using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Verse;

namespace PatchLib
{
	public class PatchOperationDebugPrint : NodePatchOperationPathed
	{
		protected string label;

		protected override bool ApplyWorker(XmlNode xml)
		{
			bool result = false;
			foreach (XmlNode xmlNode in xml.SelectNodes(xpath))
			{
				result = true;
				using (var sw = new StringWriter())
				using (var xmlw = new XmlTextWriter(sw) { Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1})
				{
					if (!string.IsNullOrEmpty(label)) { sw.WriteLine($"{label} (xpath: {xpath})"); }
					else
					{ sw.WriteLine($"Match for xpath selector: {xpath}"); }
					xmlNode.WriteTo(xmlw);
					Log.Message(sw.ToString());
				}
					
			}
			return result;
		}
	}
}
