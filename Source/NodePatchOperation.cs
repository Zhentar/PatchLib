using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Verse;

namespace PatchLib
{
	public abstract class NodePatchOperationPathed : PatchOperationPathed, INodePatchOperation
	{
		private enum Success
		{
			Normal,
			Invert,
			Always,
			Never
		}

		//private static readonly Func<PatchOperation, Success> successGet = Utils.GetFieldAccessor<PatchOperation, Success>("success");
		private Success success { get { return /*successGet(this)*/ Success.Normal; } }


		private static readonly Func<PatchOperation, bool> neverSucceededGet = Utils.GetFieldAccessor<PatchOperation, bool>("neverSucceeded");
		private static readonly FieldInfo neverSucceededInfo = typeof(PatchOperation).GetField("neverSucceeded", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
		private bool neverSucceeded { get { return neverSucceededGet(this); } set { neverSucceededInfo.SetValue(this, value); } }


		protected sealed override bool ApplyWorker(XmlDocument xml)
		{
			return this.ApplyWorker((XmlNode)xml);
		}

		public bool Apply(XmlNode xml)
		{
			bool flag = this.ApplyWorker(xml);
			if (this.success == Success.Always)
			{
				flag = true;
			}
			else if (this.success == Success.Never)
			{
				flag = false;
			}
			else if (this.success == Success.Invert)
			{
				flag = !flag;
			}
			if (flag && neverSucceeded)
			{
				this.neverSucceeded = false;
			}
			return flag;
		}

		protected abstract bool ApplyWorker(XmlNode xml);
	}

	class NodePatchOperationAdd : NodePatchOperationPathed
	{
		private enum Order
		{
			Append,
			Prepend
		}

		private XmlContainer value;

		private Order order;

		protected override bool ApplyWorker(XmlNode xml)
		{
			XmlNode node = value.node;
			bool result = false;
			foreach (XmlNode xmlNode in xml.SelectNodes(xpath))
			{
				result = true;
				if (order == Order.Append)
				{
					for (int i = 0; i < node.ChildNodes.Count; i++)
					{
						xmlNode.AppendChild(xmlNode.OwnerDocument.ImportNode(node.ChildNodes[i], true));
					}
				}
				else if (order == Order.Prepend)
				{
					for (int j = node.ChildNodes.Count - 1; j >= 0; j--)
					{
						xmlNode.PrependChild(xmlNode.OwnerDocument.ImportNode(node.ChildNodes[j], true));
					}
				}
			}
			return result;
		}
	}

	class NodePatchOperationInsert : NodePatchOperationPathed
	{
		private enum Order
		{
			Append,
			Prepend
		}

		private XmlContainer value;

		private Order order;

		protected override bool ApplyWorker(XmlNode xml)
		{
			XmlNode node = value.node;
			bool result = false;
			foreach (XmlNode xmlNode in xml.SelectNodes(xpath))
			{
				result = true;
				XmlNode parentNode = xmlNode.ParentNode;
				if (order == Order.Append)
				{
					for (var i = 0; i < node.ChildNodes.Count; i++)
					{
						parentNode.InsertAfter(parentNode.OwnerDocument.ImportNode(node.ChildNodes[i], true), xmlNode);
					}
				}
				else if (order == Order.Prepend)
				{
					for (int j = node.ChildNodes.Count - 1; j >= 0; j--)
					{
						parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(node.ChildNodes[j], true), xmlNode);
					}
				}
			}
			return result;
		}
	}

	public class NodePatchOperationRemove : NodePatchOperationPathed
	{
		protected override bool ApplyWorker(XmlNode xml)
		{
			bool result = false;
			foreach (XmlNode xmlNode in xml.SelectNodes(xpath))
			{
				result = true;
				xmlNode.ParentNode.RemoveChild(xmlNode);
			}
			return result;
		}
	}

	public class NodePatchOperationReplace : NodePatchOperationPathed
	{
		private XmlContainer value;

		protected override bool ApplyWorker(XmlNode xml)
		{
			XmlNode node = value.node;
			bool result = false;
			foreach (XmlNode xmlNode in xml.SelectNodes(xpath))
			{
				result = true;
				XmlNode parentNode = xmlNode.ParentNode;
				for (int i = 0; i < node.ChildNodes.Count; i++)
				{
					parentNode.InsertBefore(parentNode.OwnerDocument.ImportNode(node.ChildNodes[i], true), xmlNode);
				}
				parentNode.RemoveChild(xmlNode);
			}
			return result;
		}
	}
}
