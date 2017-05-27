using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Verse;

namespace PatchLib
{
	public class PatchOperationSetContext : NodePatchOperationPathed
	{
		protected List<INodePatchOperation> operations;

		protected override bool ApplyWorker(XmlNode xml)
		{
			bool result = true;
			bool anyMatch = false;
			foreach (XmlNode xmlNode in xml.SelectNodes(xpath))
			{
				anyMatch = true;
				foreach (var current in operations) { result = current.Apply(xmlNode) && result; }
			}
			return anyMatch && result;
		}
	}
}
