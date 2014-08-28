using System;
using System.Windows;
using System.Windows.Media;

namespace KyleHughes.CIS2118.KPUSim.Views
{
    /// <summary>
    /// base control for drawing visual elements on screen
    /// </summary>
    public class DrawingControl : FrameworkElement
    {
        private readonly DrawingVisual _visual;
        private readonly VisualCollection _visuals;

        public DrawingControl()
        {
            _visual = new DrawingVisual();
            _visuals = new VisualCollection(this) {_visual};
        }
        /// <summary>
        /// gets the number of children elements
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return _visuals.Count; }
        }
        /// <summary>
        /// gets the graphics context
        /// </summary>
        /// <returns>graphics context</returns>
        public DrawingContext GetContext()
        {
            return _visual.RenderOpen();
        }
        /// <summary>
        /// gets the child at index
        /// </summary>
        /// <param name="index">child to get</param>
        /// <returns>child at given index</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _visuals.Count)
                throw new ArgumentOutOfRangeException();
            return _visuals[index];
        }
    }
}