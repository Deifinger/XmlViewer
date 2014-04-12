using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace xmlViewer
{
    partial class Form1 : Form
    {
        string getTagName(TreeViewEventArgs e)
        {
            string tagName = string.Empty;
            if (document.DocumentElement.Prefix != string.Empty)
            {
                tagName = document.DocumentElement.Prefix + ":";
            }
            return tagName + e.Node.Text;
        }

        List<int> indexPathXml(object sender, TreeViewEventArgs e, bool isDblClk = false)
        {
            List<int> indexes = new List<int>();
            indexes.Add(isDblClk == true ? e.Node.Index : treeView1.SelectedNode.Index);

            TreeNode xnode = isDblClk == true ? e.Node : treeView1.SelectedNode;
            while (xnode.Parent != null)
            {
                indexes.Add(xnode.Parent.Index);
                xnode = xnode.Parent;
            }
            return indexes;
        }

        XmlNode getElemInDoc(object sender, TreeViewEventArgs e, bool isDblClk = false)
        {
            List<int> indxs = indexPathXml(sender, e, isDblClk);

            int c = indxs.Count > 2 ? 2 : indxs.Count;
            XmlNode xnode = document.DocumentElement.ChildNodes[indxs[indxs.Count - c]];
            int indElemType = 0;
            while (indxs.Count - 1 - c >= 0)
            {
                xnode = xnode.ChildNodes[0];
                while (true)
                {
                    if (xnode.NodeType == XmlNodeType.Element)
                        indElemType++;
                    if (indElemType == indxs[indxs.Count - 1 - c] + 1)
                    {
                        indElemType = 0;
                        break;
                    }
                    xnode = xnode.NextSibling;
                }
                c++;
            }
            return xnode;
        }
    }
}
