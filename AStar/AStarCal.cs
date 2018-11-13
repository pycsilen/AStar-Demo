using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AStar
{
    public partial class AStarCal : Form
    {
        public AStarCal()
        {
            InitializeComponent();
        }

        int height = 0, width = 0, wadx = 10, qezc = 14;
        private void button1_Click(object sender, EventArgs e)
        {
            if (!Int32.TryParse(this.txt_Height.Text, out height))
            {
                MessageBox.Show("请输入正确高度");
                return;
            }
            if (!Int32.TryParse(this.txt_width.Text, out width))
            {
                MessageBox.Show("请输入正确宽度");
                return;
            }
            if (!Int32.TryParse(this.txt_wadx.Text, out wadx))
            {
                MessageBox.Show("请输入正确权重");
                return;
            }
            if (!Int32.TryParse(this.txt_qezc.Text, out qezc))
            {
                MessageBox.Show("请输入正确权重");
                return;
            }
            if (this.MapPannel.Controls.Count != 0)
            {
                foreach (Control i in this.MapPannel.Controls)
                {
                    i.Dispose();
                }
                this.MapPannel.Controls.Clear();
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    MyBtn _btn = new MyBtn();
                    _btn.Tag = new Point(x, y);
                    _btn.BtnPoint = new Point(x, y);
                    _btn.Location = new Point(x * 25, y * 25);
                    _btn.Enabled = true;
                    _btn.Visible = true;
                    this.MapPannel.Controls.Add(_btn);
                }
            }
            this.Size = new Size(240 + width * 25, Math.Max(340, height * 25 + 50));
            this.button2.Enabled = true;
            Console.WriteLine();
        }
        CubeState[,] State;

        List<MyBtn> BtnList = new List<MyBtn>();
        private void button2_Click(object sender, EventArgs e)
        {
            if (MyBtn.Btn_start == null)
            {
                MessageBox.Show("请设置起点");
                return;
            }
            if (MyBtn.Btn_end == null)
            {
                MessageBox.Show("请设置终点");
                return;
            }
            State = new CubeState[width, height];
            foreach (Control i in MapPannel.Controls)
            {
                Point p = (Point)i.Tag;
                State[p.X, p.Y] = ((MyBtn)i).thisCubeState;
                BtnList.Add(i as MyBtn);
            }
            String str = "";
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    str += Enum.GetName(typeof(CubeState), State[x, y]) + "\t";
                }
                str += "\r\n";
            }
            List<MyBtn> OpenList = new List<MyBtn>();
            List<MyBtn> CloseList = new List<MyBtn>();
            CloseList.Add(MyBtn.Btn_start);
            OpenList = (from btn in BtnList
                        where (
                            Math.Abs(btn.BtnPoint.X - MyBtn.Btn_start.BtnPoint.X) <= 1
                            &&
                            Math.Abs(btn.BtnPoint.Y - MyBtn.Btn_start.BtnPoint.Y) <= 1
                            &&
                            ((Point)btn.Tag) != ((Point)MyBtn.Btn_start.Tag)
                            )
                        select btn).ToList();

            CaculatePath(OpenList, CloseList, MyBtn.Btn_start);
        }
        List<MyBtn> Path = new List<MyBtn>();
        private void FindPath(MyBtn Node)
        {
            if (Node == MyBtn.Btn_start)
            {
                Path.Insert(0, Node);
                for (int i = 1; i < Path.Count - 1; i++)
                {
                    Path[i].thisCubeState = CubeState.Path;
                    Path[i].SetColor();
                }
                return;
            }
            Path.Insert(0, Node);
            FindPath(Node.ParentNode);
        }
        private void CaculatePath(List<MyBtn> OpenList, List<MyBtn> CloseList, MyBtn Node)
        {
            Point nodepoint = Node.BtnPoint;
            Point endPoint = MyBtn.Btn_end.BtnPoint;
            CloseList.Add(Node);

            List<MyBtn> BlockList = OpenList.Where(btn => btn.thisCubeState == CubeState.Block).ToList();
            OpenList.RemoveAll(btn => BlockList.Contains(btn) || CloseList.Contains(btn)
                ||
                BlockList.Where(a =>
                    Math.Pow(a.BtnPoint.X - btn.BtnPoint.X, 2) + Math.Pow(a.BtnPoint.Y - btn.BtnPoint.Y, 2) == 1
                    &&
                    Math.Pow(Node.BtnPoint.X - a.BtnPoint.X, 2) + Math.Pow(Node.BtnPoint.Y - a.BtnPoint.Y, 2) == 1
                    )
                    .ToList().Count != 0);

            if (OpenList.Contains(MyBtn.Btn_end))
            {
                MyBtn.Btn_end.ParentNode = Node;
                FindPath(MyBtn.Btn_end);
                return;
            }

            if (OpenList.Count == 0)
            {
                if (Node == MyBtn.Btn_start)
                {
                    return;
                }

                OpenList = (from btn in BtnList
                            where (
                                Math.Abs(btn.BtnPoint.X - Node.ParentNode.BtnPoint.X) <= 1
                                &&
                                Math.Abs(btn.BtnPoint.Y - Node.ParentNode.BtnPoint.Y) <= 1
                                &&
                                ((Point)btn.Tag) != ((Point)Node.ParentNode.Tag)
                                )
                            select btn).ToList();
                CaculatePath(OpenList, CloseList, Node.ParentNode);
                return;
            }

            foreach (MyBtn btn in OpenList)
            {
                Point btnPoint = btn.BtnPoint;
                if (btn.H == 0)
                    btn.H = (Math.Abs(endPoint.Y - btnPoint.Y) + Math.Abs(endPoint.X - btnPoint.X)) * wadx;
                if (btn.ParentNode == null)
                    btn.ParentNode = Node;

                Point parentNodePoint = btn.ParentNode.BtnPoint;

                if (btn.G == 0)
                {
                    if (Math.Pow((parentNodePoint.X - btnPoint.X), 2) + Math.Pow((parentNodePoint.Y - btnPoint.Y), 2) == 2)
                        btn.G = qezc + btn.ParentNode.G;
                    else
                        btn.G = wadx + btn.ParentNode.G;
                }
                else
                {
                    if (btn.ParentNode != Node)
                    {
                        int G = 0;
                        if (Math.Pow((nodepoint.X - btnPoint.X), 2) + Math.Pow((nodepoint.Y - btnPoint.Y), 2) == 2)
                            G = qezc + Node.G;
                        else
                            G = wadx + Node.G;
                        if (G < btn.G)
                        {
                            btn.G = G;
                            btn.ParentNode = Node;
                        }
                    }
                }
            }
            int MinF = OpenList.Min(a => a.F);
            MyBtn newNode = OpenList.FindLast(btn => btn.F == MinF);

            OpenList = (from btn in BtnList
                        where (
                            Math.Abs(btn.BtnPoint.X - newNode.BtnPoint.X) <= 1
                            &&
                            Math.Abs(btn.BtnPoint.Y - newNode.BtnPoint.Y) <= 1
                            &&
                            ((Point)btn.Tag) != ((Point)newNode.Tag)
                            )
                        select btn).ToList();
            CaculatePath(OpenList, CloseList, newNode);

        }

    }
}
