using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PatchLib
{
	public interface INodePatchOperation
	{
		bool Apply(XmlNode xml);
	}

	
}
