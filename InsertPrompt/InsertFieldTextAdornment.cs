using InsertPrompt.Parser;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace InsertPrompt
{
    /// <summary>
    /// InsertFieldTextAdornment places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class InsertFieldTextAdornment
    {
        private string _text;
        private InsertVisitor _visitor;
        private List<Element> _elements;
        private List<SnapshotSpan> _spans;
        /// <summary>
        /// The layer of the adornment.
        /// </summary>
        private readonly IAdornmentLayer layer;

        /// <summary>
        /// Text view where the adornment is created.
        /// </summary>
        private readonly IWpfTextView view;

        /// <summary>
        /// Adornment brush.
        /// </summary>
        private readonly Brush brush;

        /// <summary>
        /// Adornment pen.
        /// </summary>
        private readonly Pen pen;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertFieldTextAdornment"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public InsertFieldTextAdornment(IWpfTextView view)
        {
            _elements = new List<Element>();
            _spans = new List<SnapshotSpan>();
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            this.layer = view.GetAdornmentLayer("InsertFieldTextAdornment");

            this.view = view;
            this.view.LayoutChanged += this.OnLayoutChanged;
            this.view.Caret.PositionChanged += Caret_PositionChanged;
            // Create the pen and brush to color the box behind the a's
            this.brush = new SolidColorBrush(Colors.Purple);
            this.brush.Freeze();
            var penBrush = new SolidColorBrush(Colors.Red);
            penBrush.Freeze();
            this.pen = new Pen(penBrush, 1);
            this.pen.Freeze();
        }

        private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            _elements.Clear();
            
            foreach (var insert in _visitor.Inserts)
            {
                foreach (var column in insert.ColumnMappings)
                {
                    var position = e.NewPosition.BufferPosition.Position;
                    if (position >= column.Field.Start && position <= column.Field.Start + column.Field.Lenght
                        ||
                        column.Values.Any(f=>position>=f.Start&&position<=f.Start+f.Lenght))
                    {
                        _elements.Add(column.Field);
                        foreach(var value in column.Values)
                        {
                            _elements.Add(value);
                        }
                    }
                }
            }
            Redraw();
        }

        internal void Redraw()
        {
            foreach (var span in _spans)
                this.layer.RemoveAdornmentsByVisualSpan(span);

            _spans.Clear();

            foreach (var element in _elements)
            {
                _spans.Add(CreateVisuals(element));
            }
        }
        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            var text = view.TextSnapshot.GetText();
            if (!string.IsNullOrEmpty(text) && text != _text)
            {
                var parser = new Microsoft.SqlServer.TransactSql.ScriptDom.TSql160Parser(true);
                var result = parser.Parse(new StringReader(text), out var errors);
                if (errors != null)
                {
                    _visitor = new InsertVisitor(text);
                    result.Accept(_visitor);
                    _text = text;
                }
            }

            Redraw();
        }

        /// <summary>
        /// Adds the scarlet box behind the 'a' characters within the given line
        /// </summary>
        /// <param name="line">Line to add the adornments</param>
        private SnapshotSpan CreateVisuals(Element element)
        {
            int start = element.Start; 
            int length = element.Lenght;

            IWpfTextViewLineCollection textViewLines = this.view.TextViewLines;
            SnapshotSpan span = new SnapshotSpan(this.view.TextSnapshot, Microsoft.VisualStudio.Text.Span.FromBounds(start, start + length));
             //textViewLines.GetMarkerGeometry(span);
            Geometry geometry =textViewLines.GetMarkerGeometry(span);
            if (geometry != null)
            {
                var drawing = new GeometryDrawing(this.brush, this.pen, geometry);
                drawing.Freeze();

                var drawingImage = new DrawingImage(drawing);
                drawingImage.Freeze();

                var image = new System.Windows.Controls.Image
                {
                    Source = drawingImage,
                };

                // Align the image with the top of the bounds of the text geometry
                Canvas.SetLeft(image, geometry.Bounds.Left);
                Canvas.SetTop(image, geometry.Bounds.Top);

                this.layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);   
            }
            return span;
        }
    }
}
