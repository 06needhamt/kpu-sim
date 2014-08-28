using KyleHughes.CIS2118.KPUSim.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KyleHughes.CIS2118.KPUSim.Peripherals
{
    /// <summary>
    /// not proud of this it's all one massive hack
    /// </summary>
    [Peripheral("I/O Terminal", "Provides ways of displaying data on the screen.\n\n"
        + "Interrupts:\n"
        + "0: Turns on the display\n"
        + "1: Sets the X coordinate to IOD\n"
        + "2: Sets the Y coordinate to IOD\n"
        + "3: Draws the character in IOD at X/Y and increments the cursor by 1\n"
        + "4: Draws the character in IOD at X/Y without incrementing the cursor the cursor\n"
        + "5: Swaps the display buffers"
        , true)]
    public class TerminalPeripheral : PeripheralBase
    {
        /// <summary>
        /// the width of the bitmap font
        /// </summary>
        [JsonIgnore]
        public readonly int CHAR_WIDTH = 8;
        /// <summary>
        /// the height of the bitmap font
        /// </summary>
        [JsonIgnore]
        public readonly int CHAR_HEIGHT = 8;

        private int _numRows = 20, _numCols = 20;
        private float _scale = 3;
        /// <summary>
        /// the number of rows this display has 
        /// </summary>
        public int NumRows
        {
            get
            {
                return _numRows;
            }
            set
            {
                _numRows = value;
                if (Window != null)
                    Window.SizeWindow();
                Notify();
            }
        }
        /// <summary>
        /// the number of columns this display has
        /// </summary>
        public int NumCols
        {
            get
            {
                return _numCols;
            }
            set
            {
                _numCols = value;
                if (Window != null)
                    Window.SizeWindow();
                Notify();
            }
        }
        /// <summary>
        /// the scale this display uses
        /// </summary>
        public float Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                if (Window != null)
                    Window.SizeWindow();
                Notify();
            }
        }
        /// <summary>
        /// the cursor's x pos
        /// </summary>
        [JsonIgnore]
        public int xPos;
        /// <summary>
        /// the cursor's y pos
        /// </summary>
        [JsonIgnore]
        public int yPos;
        /// <summary>
        /// the window this peripheral uses
        /// </summary>
        [JsonIgnore]
        public TerminalView Window { get; set; }
        /// <summary>
        /// constructs a new terminal with the given id
        /// </summary>
        /// <param name="id">id</param>
        public TerminalPeripheral(ushort id)
            : base(id)
        {
            NumCols = 20;
            NumRows = 20;
            Scale = 3;
            this.Window = new TerminalView(this);
        }
        /// <summary>
        /// handles an interrupt with command, IOD and IOM
        /// </summary>
        /// <param name="command">command</param>
        /// <param name="data">IOD</param>
        /// <param name="extra">IOM</param>
        public override void HandleInterrupt(ushort command, ushort data, ushort extra)
        {
            switch (command)
            {
                case 0: //turn on!
                    if (this.Window == null || this.Window.IsLoaded == false)
                        this.Window = new TerminalView(this);
                    this.Window.Show();
                    break;
                case 1: //set x
                    SetCursor(data, yPos);
                    break;
                case 2: //set y
                    SetCursor(xPos, data);
                    break;
                case 3: //draw a char
                    if (this.Window != null && this.Window.IsLoaded)
                        Window.DrawCharacter(xPos, yPos, (char)data);
                    IncrementCursor(); //increment cursor
                    break;
                case 4: //draw a char
                    if (this.Window != null && this.Window.IsLoaded)
                        Window.DrawCharacter(xPos, yPos, (char)data);
                    break;
                case 5: //swap buffers
                    if (this.Window != null && this.Window.IsLoaded)
                        this.Window.SwapBuffer();
                    break;
            }
        }
        /// <summary>
        /// Sets the cursor positon
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        public void SetCursor(int x, int y)
        {
            //wrap around!
            xPos = (int)(x % (NumCols));
            yPos = (int)(y % (NumRows));
        }
        /// <summary>
        /// increments the cursor's positon by 1
        /// </summary>
        public void IncrementCursor()
        {
            int newx = xPos + 1;
            int newy = yPos;
            if (newx >= NumCols) //if it's too wide add 1 to y
                newy++;

            SetCursor(newx, newy);
        }
        /// <summary>
        /// on configure, show dialog
        /// </summary>
        public override void Configure()
        {
            ConfigureTerminalView v = new ConfigureTerminalView();
            v.DataContext = this;
            v.ShowDialog();
        }
        /// <summary>
        /// close the window if it's open
        /// </summary>
        public override void CleanUp()
        {
            if (this.Window.IsLoaded)
                this.Window.Close();
        }
    }
}
