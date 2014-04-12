using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;

namespace xmlViewer
{
    public partial class Form1 : Form
    {
        XmlDocument document = new XmlDocument();
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();
            opd.Filter = "XML File *.xml|*.xml|HTML File *.html|*.html";
            try
            {
                if (opd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (treeView1.Nodes.Count > 0)
                    {
                        treeView1.Nodes.RemoveAt(0);
                        textBox1.Text = string.Empty;
                    }
                    toolStripStatusLabelPath.Text = "Путь: " + opd.FileName;
                    document.Load(opd.FileName);
                    // Upload root element and his childs in treeList
                    XmlNode xn = document.DocumentElement;

                    int counter = 0;
                    TreeNode node = new TreeNode(xn.LocalName);
                    treeView1.Nodes.Add(node);

                    XmlNodeList childs = xn.ChildNodes;
                    foreach (XmlNode child in childs)
                    {
                        treeView1.Nodes[counter].Nodes.Add(new TreeNode(child.LocalName));
                    }
                    counter++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка при открытии файла");
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //if (((TreeView)sender).SelectedNode.Nodes.Count != 0 /*e.Node.Nodes.Count != 0*/
            //    && ((TreeView)sender).SelectedNode.Nodes[0].Nodes.Count == 0)
            if (treeView1.SelectedNode.Nodes.Count != 0
                && treeView1.SelectedNode.Nodes[0].Nodes.Count == 0)
            {
                try
                {
                    Dictionary<string, int> foundedElems = new Dictionary<string, int>();
                    
                    string tagName = string.Empty;
                    TreeNode chxn = new TreeNode();
                    int c = 0;
                    for (int indTreeElem = 0; indTreeElem < treeView1.SelectedNode.Nodes.Count; indTreeElem++)
                    {
                        c = 0;
                        chxn = treeView1.SelectedNode.Nodes[indTreeElem];
                        tagName = getTagName(new TreeViewEventArgs(chxn));
                        try
                        {
                            foundedElems.Contains(new KeyValuePair<string, int>(tagName, foundedElems[tagName]));
                        }
                        catch (KeyNotFoundException)
                        {
                            foundedElems.Add(tagName, 0);
                        }

                        XmlNodeList xnl = getElemInDoc(sender, new TreeViewEventArgs(chxn), true).ChildNodes;
                        
                        foreach (XmlNode xn in xnl)
                        {
                            if (xn.NodeType == XmlNodeType.Element)
                            {
                                treeView1.SelectedNode.Nodes[indTreeElem].Nodes.Add(xn.LocalName);
                                c++;
                            }
                        }
                        if (treeView1.SelectedNode.Nodes[indTreeElem].Nodes.Count > c)
                            while (treeView1.SelectedNode.Nodes[indTreeElem].Nodes.Count != c)
                                treeView1.SelectedNode.Nodes[indTreeElem].Nodes.RemoveAt(treeView1.SelectedNode.Nodes[indTreeElem].Nodes.Count - 1);
                    }
                    
                    /*System.Timers.Timer t = new System.Timers.Timer(3000);
                    t.Elapsed += (object senderT, System.Timers.ElapsedEventArgs eT) => 
                    {
                        System.Timers.Timer theTimer = senderT as System.Timers.Timer;
                        MessageBox.Show(string.Format("00000"));
                        theTimer.Dispose();
                    };
                    t.Start();*/
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            string tagName = getTagName(e);

            XmlNode xnode = getElemInDoc(sender, e);

            string attrs = string.Empty;
            if (xnode.Attributes != null)
            {
                foreach (XmlAttribute xa in xnode.Attributes)
                {
                    attrs += string.Format("{0}=\"{1}\"\t", xa.Name, xa.Value);
                }

                textBoxName.Text = string.Format(xnode.Name);
                textBoxPath.Text = string.Format(treeView1.SelectedNode.FullPath);
                textBox1.Text = string.Format("Атрибуты: {0}", attrs);
            }
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeNodeMouseClickEventArgs ev = new TreeNodeMouseClickEventArgs(e.Node,
                new System.Windows.Forms.MouseButtons(), 0, 0, 0);
            ((TreeView)sender).SelectedNode = e.Node;
            treeView1_NodeMouseDoubleClick(sender, ev);
        }

        private void toolStripButtonShowContent_Click(object sender, EventArgs e)
        {
            try
            {
                XmlNode xnode = getElemInDoc(sender, new TreeViewEventArgs(treeView1.SelectedNode));
                textBox1.Text += string.Format("\r\nСодержимое: {0}", xnode.InnerText);
            }
            catch
            {
                MessageBox.Show("Для того, чтобы показать содержимое элемента, пожалуйста, выберите его", "Элемент не выбран");
            }
        }

        private void toolStripButtonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
