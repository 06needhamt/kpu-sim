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
    public partial class StorageView : Window
    {
        public StoragePeripheral Peripheral { get; private set; }
        public System.Windows.Forms.Control Form { get; private set; }
        public StorageView(StoragePeripheral p)
        {
            InitializeComponent();
            if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue))
                return;

            this.Peripheral = p;
            this.Form = new InnerForm(Peripheral, 256, 256);
            formshost.Child = Form;
            SizeWindow();
        }
        public void SizeWindow()
        {
            formshost.Width = 256;
            formshost.Height = 256;
        }

        class InnerForm : System.Windows.Forms.Control
        {
            public StoragePeripheral Peripheral { get; private set; }

            public InnerForm(StoragePeripheral p, int w, int h)
            {
                this.Peripheral = p;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                for (var i = 0; i < 65536; i++)
                {
                    var rgb = 0;
                    try{
                        rgb = this.Peripheral.Values[i];
                    }catch (Exception){}
                    rgb *= 255;
                    e.Graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb((rgb >> 16) & 0xff, (rgb >> 8) & 0xff, (rgb >> 0) & 0xff)), i % 256, i / 256, 1, 1);
                }

            }
        }
    }

}
