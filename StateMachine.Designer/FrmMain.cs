using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Graph;
using Graph.Items;

namespace StateMachine.Designer
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            Node node = new Node("FTP Uploader");
            node.AddItem(new NodeLabelItem("Text goes here"));
            node.AddItem(new NodeSliderItem("Text goes here", 2, 2, 0, 100, 50,true, true));

            graphControl1.AddNode(node);
        }
    }
}
