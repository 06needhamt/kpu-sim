using KyleHughes.CIS2118.KPUSim.Peripherals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KyleHughes.CIS2118.KPUSim.Views
{
    public partial class TerminalView : Window
    {
        public TerminalPeripheral Peripheral { get; private set; }
        public TerminalView(TerminalPeripheral p)
        {
            InitializeComponent();
            if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue))
                return;

            this.Peripheral = p;
            InnerForm inner = new InnerForm(Peripheral, p.NumCols * p.CHAR_WIDTH, p.NumRows * p.CHAR_HEIGHT);
            formshost.Child = inner;
            SizeWindow();
        }
        public void SizeWindow()
        {
            formshost.Width = Peripheral.NumCols * Peripheral.CHAR_WIDTH * Peripheral.Scale;
            formshost.Height = Peripheral.NumRows * Peripheral.CHAR_HEIGHT * Peripheral.Scale;
        }
        public void SwapBuffer()
        {
            formshost.Child.Refresh();
        }
        public void DrawCharacter(int x, int y, char c)
        {
            (formshost.Child as InnerForm).DrawCharacter(x, y, c);
        }

        class InnerForm : System.Windows.Forms.Control
        {
            public TerminalPeripheral Peripheral { get; private set; }
            System.Drawing.Bitmap fontBitmap;
            System.Drawing.Rectangle[] fontRects = new System.Drawing.Rectangle[128];
            public InnerForm(TerminalPeripheral p, int w, int h)
            {
                this.bmp = new System.Drawing.Bitmap(w, h);
                this.gBmp = System.Drawing.Graphics.FromImage(bmp);
                this.Peripheral = p;
                this.fontBitmap = KyleHughes.CIS2118.KPUSim.Properties.Resources.Font;
                for (int i = 0; i < fontRects.Length; i++)
                    fontRects[i] = new System.Drawing.Rectangle((i * Peripheral.CHAR_WIDTH), 0, Peripheral.CHAR_WIDTH, Peripheral.CHAR_HEIGHT);

            }
            System.Drawing.Bitmap bmp;//
            System.Drawing.Graphics gBmp;//
            public void DrawCharacter(int x, int y, char c)
            {
                if ((int)c < 0 || (int)c > 127)
                    return;
                gBmp.DrawImage(fontBitmap, new System.Drawing.Rectangle(x * Peripheral.CHAR_WIDTH, y * Peripheral.CHAR_HEIGHT, Peripheral.CHAR_WIDTH, Peripheral.CHAR_HEIGHT), fontRects[(int)c], System.Drawing.GraphicsUnit.Pixel);
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.ScaleTransform(Peripheral.Scale, Peripheral.Scale);
                e.Graphics.DrawImage(bmp, 0, 0);

            }
        }
    }

}
