using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AStar
{
    public enum CubeState
    {
        Null = 0,
        Block,
        Start,
        End,
        Path
    }
    public partial class MyBtn : UserControl
    {
        public static MyBtn Btn_start;
        public static MyBtn Btn_end;
        public MyBtn()
        {
            InitializeComponent();
        }
        public CubeState thisCubeState = CubeState.Null;

        public Point BtnPoint { get; set; }
        public MyBtn ParentNode { get; set; }//父节点
        public int G { get; set; }//到达父节点权重
        public int H { get; set; }//到达终点权重
        public int F { get { return G + H; } }//综合权重：F=G+H
        private void MyBtn_MouseClick(object sender, MouseEventArgs e)
        {
            int i = (int)thisCubeState;
            if (e.Button == MouseButtons.Left)
            {
                if (i < 3)
                    i++;
            }
            if (e.Button == MouseButtons.Right)
            {
                if (i > 0)
                    i--;
            }
            thisCubeState = (CubeState)i;
            SetColor();
        }
        public void SetColor()
        {
            switch (thisCubeState)
            {
                case CubeState.Start:
                    this.BackColor = Color.Blue;
                    if (Btn_start != null)
                    {
                        Btn_start.thisCubeState = CubeState.Null;
                        Btn_start.SetColor();
                        Btn_start = this;
                    }
                    else
                    {
                        Btn_start = this;
                    }
                    return;
                case CubeState.End:
                    if (Btn_end != null)
                    {
                        Btn_end.thisCubeState = CubeState.Null;
                        Btn_end.SetColor();
                        Btn_end = this;
                    }
                    else
                    {
                        if (Btn_start == this)
                            Btn_start = null;
                        Btn_end = this;
                    }
                    this.BackColor = Color.Red;
                    return;
                case CubeState.Null:
                    this.BackColor = Color.Gray;
                    break;
                case CubeState.Block:
                    this.BackColor = Color.Black;
                    break;
                case CubeState.Path:
                    this.BackColor = Color.White;
                    break;
            }
            if (Btn_start == this && thisCubeState != CubeState.Start)
            {
                Btn_start = null;
            }
            if (Btn_end == this && thisCubeState != CubeState.End)
            {
                Btn_end = null;
            }
        }

    }
}
