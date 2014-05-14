﻿﻿#region License
// Copyright (c) 2009 Sander van Rossen, 2013 Oliver Salzburg
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Graph
{
	public class GraphRenderer : IGraphRenderer
	{
	    public SizeF Measure(Graphics context, Node node)
		{
			if (node == null)
				return SizeF.Empty;

			SizeF size = Size.Empty;
			size.Height = //(int)NodeConstants.TopHeight + 
				(int)GraphConstants.BottomHeight;
			foreach (var item in GraphUtils.EnumerateNodeItems(node))
			{
				var itemSize = item.Measure(context);
				size.Width = Math.Max(size.Width, itemSize.Width);
				size.Height += GraphConstants.ItemSpacing + itemSize.Height;
			}
			
			size.Width += GraphConstants.NodeExtraWidth;
			return size;
		}

		static SizeF PreRenderItem(Graphics graphics, NodeItem item, PointF position)
		{
			var itemSize = (SizeF)item.Measure(graphics);
			item.bounds = new RectangleF(position, itemSize);
			return itemSize;
		}

		static void RenderItem(Graphics graphics, SizeF minimumSize, NodeItem item, PointF position)
		{
			item.Render(graphics, minimumSize, position);
		}

		private static Pen BorderPen = new Pen(Color.FromArgb(64, 64, 64));

		public void RenderConnector(Graphics graphics, NodeConnector nodeConnector, RenderState renderState)
		{
            if (nodeConnector.ConnectorType == NodeConnectorType.Exec)
                RenderExecConnector(graphics, nodeConnector, renderState);
            //else if (nodeConnector.ConnectorType == NodeConnectorType.Array)
            //    RenderArrayConnector(graphics, nodeConnector, renderState);
            else if (nodeConnector.ConnectorType == NodeConnectorType.Value)
                RenderValueConnector(graphics, nodeConnector, renderState);
		}

        private void RenderValueConnector(Graphics graphics, NodeConnector nodeConnector, RenderState renderState)
        {
            RectangleF bounds = nodeConnector.bounds;
            var state = renderState;
            using (var brush = new SolidBrush(GetArrowLineColor(state)))
            {
                graphics.FillEllipse(brush, bounds);
            }

            if (state == RenderState.None)
            {
                graphics.DrawEllipse(Pens.Black, bounds);
            }
            else
                // When we're compatible, but not dragging from this node we render a highlight
                if ((state & (RenderState.Compatible | RenderState.Dragging)) == RenderState.Compatible)
                {
                    // First draw the normal black border
                    graphics.DrawEllipse(Pens.Black, bounds);

                    // Draw an additional highlight around the connector
                    RectangleF highlightBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                    highlightBounds.Width += 10;
                    highlightBounds.Height += 10;
                    highlightBounds.X -= 5;
                    highlightBounds.Y -= 5;
                    graphics.DrawEllipse(Pens.OrangeRed, highlightBounds);
                }
                else
                {
                    graphics.DrawArc(Pens.Black, bounds, 90, 180);
                    using (var pen = new Pen(GetArrowLineColor(state)))
                    {
                        graphics.DrawArc(pen, bounds, 270, 180);
                    }
                }			

        }

        private void RenderExecConnector(Graphics graphics, NodeConnector nodeConnector, RenderState renderState)
        {
            RectangleF bounds = nodeConnector.bounds;
            var state = renderState;
            using (var brush = new SolidBrush(GetArrowLineColor(state)))
            {
                graphics.FillRectangle(brush, bounds);
            }

            if (state == RenderState.None)
            {
                graphics.DrawRectangle(Pens.Black, Rectangle.Round(bounds));
            }
            else
                // When we're compatible, but not dragging from this node we render a highlight
                if ((state & (RenderState.Compatible | RenderState.Dragging)) == RenderState.Compatible)
                {
                    // First draw the normal black border
                    graphics.DrawRectangle(Pens.Black, Rectangle.Round(bounds));

                    // Draw an additional highlight around the connector
                    RectangleF highlightBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                    highlightBounds.Width += 10;
                    highlightBounds.Height += 10;
                    highlightBounds.X -= 5;
                    highlightBounds.Y -= 5;
                    graphics.DrawRectangle(Pens.OrangeRed, Rectangle.Round(highlightBounds));
                }
                else
                {
                    graphics.DrawRectangle(Pens.Black, Rectangle.Round(bounds));
                    //using (var pen = new Pen(GetArrowLineColor(state)))
                    //{
                    //    graphics.DrawArc(pen, bounds, 270, 180);
                    //}
                }
        }

		void RenderArrow(Graphics graphics, RectangleF bounds, RenderState connectionState)
		{
			var x = (bounds.Left + bounds.Right) / 2.0f;
			var y = (bounds.Top + bounds.Bottom) / 2.0f;
			using (var brush = new SolidBrush(GetArrowLineColor(connectionState | RenderState.Connected)))
			{
				graphics.FillPolygon(brush, GraphUtils.GetArrowPoints(x,y), FillMode.Winding);
			}
		}

		public void PerformLayout(Graphics graphics, IEnumerable<Node> nodes)
		{
			foreach (var node in nodes.Reverse<Node>())
			{
				PerformLayout(graphics, node);
			}
		}


		public void PerformLayout(Graphics graphics, Node node)
		{
		    if (node == null)
		        return;
		    var size = Measure(graphics, node);
		    var position = node.Location;
		    node.bounds = new RectangleF(position, size);

		    var path = new GraphicsPath(FillMode.Winding);
		    int connectorSize = (int) GraphConstants.ConnectorSize;
		    int halfConnectorSize = (int) Math.Ceiling(connectorSize/2.0f);
		    var connectorOffset = (int) Math.Floor((GraphConstants.MinimumItemHeight - GraphConstants.ConnectorSize)/2.0f);
		    var left = position.X + halfConnectorSize;
		    var top = position.Y;
		    var right = position.X + size.Width - halfConnectorSize;
		    var bottom = position.Y + size.Height;

		    node.inputConnectors.Clear();
		    node.outputConnectors.Clear();
		    //node.connections.Clear();

		    var itemPosition = position;
		    itemPosition.X += connectorSize + (int) GraphConstants.HorizontalSpacing;
		    node.inputBounds = Rectangle.Empty;
		    node.outputBounds = Rectangle.Empty;

		    foreach (var item in GraphUtils.EnumerateNodeItems(node))
		    {
		        var itemSize = PreRenderItem(graphics, item, itemPosition);
		        var realHeight = itemSize.Height;
		        var inputConnector = item.Input;
		        if (inputConnector != null && inputConnector.Enabled)
		        {
		            if (itemSize.IsEmpty)
		            {
		                inputConnector.bounds = Rectangle.Empty;
		            }
		            else
		            {
		                inputConnector.bounds = new RectangleF(left - (GraphConstants.ConnectorSize/2),
		                                                       itemPosition.Y + connectorOffset,
		                                                       GraphConstants.ConnectorSize,
		                                                       GraphConstants.ConnectorSize);
		            }
		            node.inputConnectors.Add(inputConnector);
		        }
		        var outputConnector = item.Output;
		        if (outputConnector != null && outputConnector.Enabled)
		        {
		            if (itemSize.IsEmpty)
		            {
		                outputConnector.bounds = Rectangle.Empty;
		            }
		            else
		            {
		                outputConnector.bounds = new RectangleF(right - (GraphConstants.ConnectorSize/2),
		                                                        itemPosition.Y + realHeight - (connectorOffset + GraphConstants.ConnectorSize),
		                                                        GraphConstants.ConnectorSize,
		                                                        GraphConstants.ConnectorSize);
		            }
		            node.outputConnectors.Add(outputConnector);
		        }
		        itemPosition.Y += itemSize.Height + GraphConstants.ItemSpacing;
		    }
		    node.itemsBounds = new RectangleF(left, top, right - left, bottom - top);
		}

	    public void RenderNode(Graphics graphics, Node node)
	    {
	        var size = node.bounds.Size;
	        var position = node.bounds.Location;

	        int cornerSize = (int) GraphConstants.CornerSize*2;
	        int connectorSize = (int) GraphConstants.ConnectorSize;
	        int halfConnectorSize = (int) Math.Ceiling(connectorSize/2.0f);
	        var connectorOffset = (int) Math.Floor((GraphConstants.MinimumItemHeight - GraphConstants.ConnectorSize)/2.0f);
	        var left = position.X + halfConnectorSize;
	        var top = position.Y;
	        var right = position.X + size.Width - halfConnectorSize;
	        var bottom = position.Y + size.Height;
	        using (var path = new GraphicsPath(FillMode.Winding))
	        {
	            path.AddArc(left, top, cornerSize, cornerSize, 180, 90);
	            path.AddArc(right - cornerSize, top, cornerSize, cornerSize, 270, 90);

	            path.AddArc(right - cornerSize, bottom - cornerSize, cornerSize, cornerSize, 0, 90);
	            path.AddArc(left, bottom - cornerSize, cornerSize, cornerSize, 90, 90);
	            path.CloseFigure();

	            //if ((node.state & (RenderState.Dragging | RenderState.Focus)) != 0)
	            //{
	            //    graphics.FillPath(Brushes.DarkOrange, path);
	            //} else
	            //if ((node.state & RenderState.Hover) != 0)
	            //{
	            //    graphics.FillPath(Brushes.LightSteelBlue, path);
	            //} else

	            using (SolidBrush brush = new SolidBrush(node.BackColor))
	            {
	                graphics.FillPath(brush, path);
	            }

                if ((node.state & RenderState.Hover) == RenderState.Hover)
                {
                    using (Pen pen = new Pen(Color.Orange, 2))
                    {
                        graphics.DrawPath(pen, path);    
                    }
                }
                else
                {
                    graphics.DrawPath(BorderPen, path);
                }
	        }
	        /*
			if (!node.Collapsed)
				graphics.DrawLine(Pens.Black, 
					left  + GraphConstants.ConnectorSize, node.titleItem.bounds.Bottom - GraphConstants.ItemSpacing, 
					right - GraphConstants.ConnectorSize, node.titleItem.bounds.Bottom - GraphConstants.ItemSpacing);
			*/
	        var itemPosition = position;
	        itemPosition.X += connectorSize + (int) GraphConstants.HorizontalSpacing;
	        node.inputBounds = Rectangle.Empty;
	        node.outputBounds = Rectangle.Empty;

	        var minimumItemSize = new SizeF(node.bounds.Width - GraphConstants.NodeExtraWidth, 0);
	        foreach (var item in GraphUtils.EnumerateNodeItems(node))
	        {
	            RenderItem(graphics, minimumItemSize, item, itemPosition);
	            var inputConnector = item.Input;
	            if (inputConnector != null && inputConnector.Enabled)
	            {
	                if (!inputConnector.bounds.IsEmpty)
	                {
	                    var state = RenderState.None;
	                    var connected = false;
	                    foreach (var connection in node.connections)
	                    {
	                        if (connection.To == inputConnector)
	                        {
	                            state |= connection.state;
	                            connected = true;
	                        }
	                    }

	                    RenderConnector(graphics, inputConnector, state);

	                    if (connected)
	                        RenderArrow(graphics, inputConnector.bounds, state);
	                }
	            }
	            var outputConnector = item.Output;
	            if (outputConnector != null && outputConnector.Enabled)
	            {
	                if (!outputConnector.bounds.IsEmpty)
	                {
	                    var state = outputConnector.state;
	                    foreach (var connection in node.connections)
	                    {
	                        if (connection.From == outputConnector)
	                            state |= connection.state | RenderState.Connected;
	                    }
	                    RenderConnector(graphics, outputConnector, state);
	                }
	            }
	            itemPosition.Y += item.bounds.Height + GraphConstants.ItemSpacing;
	        }
	    }


	    public void RenderLabel(Graphics graphics, NodeConnection connection, PointF center, RenderState state)
		{
			using (var path = new GraphicsPath(FillMode.Winding))
			{			
				int cornerSize			= (int)GraphConstants.CornerSize * 2;
				int connectorSize		= (int)GraphConstants.ConnectorSize;
				int halfConnectorSize	= (int)Math.Ceiling(connectorSize / 2.0f);

				SizeF size;
				PointF position;
				var text		= connection.Name;
				if (connection.textBounds.IsEmpty ||
					connection.textBounds.Location != center)
				{
					size		= graphics.MeasureString(text, SystemFonts.StatusFont, center, GraphConstants.CenterTextStringFormat);
					position	= new PointF(center.X - (size.Width / 2.0f) - halfConnectorSize, center.Y - (size.Height / 2.0f));
					size.Width	+= connectorSize;
					connection.textBounds = new RectangleF(position, size);
				} else
				{
					size		= connection.textBounds.Size;
					position	= connection.textBounds.Location;
				}

				var left				= position.X;
				var top					= position.Y;
				var right				= position.X + size.Width;
				var bottom				= position.Y + size.Height;
				path.AddArc(left, top, cornerSize, cornerSize, 180, 90);
				path.AddArc(right - cornerSize, top, cornerSize, cornerSize, 270, 90);

				path.AddArc(right - cornerSize, bottom - cornerSize, cornerSize, cornerSize, 0, 90);
				path.AddArc(left, bottom - cornerSize, cornerSize, cornerSize, 90, 90);
				path.CloseFigure();

				using (var brush = new SolidBrush(GetArrowLineColor(state)))
				{
					graphics.FillPath(brush, path);
				}

				graphics.DrawString(text, SystemFonts.StatusFont, Brushes.Black, center, GraphConstants.CenterTextStringFormat);

				if (state == RenderState.None)
					graphics.DrawPath(Pens.Black, path);
			}
		}

	    public void RenderOutputConnection(Graphics graphics, NodeConnector output, float x, float y, RenderState state)
		{
			if (graphics == null ||
				output == null)
				return;

		    RectangleF outputBounds = output.bounds;

			var x1 = (outputBounds.Left + outputBounds.Right) / 2.0f;
			var y1 = (outputBounds.Top + outputBounds.Bottom) / 2.0f;
			
			float centerX;
			float centerY;
			using (var path = GraphUtils.GetArrowLinePath(x1, y1, x, y, out centerX, out centerY, true, 0.0f))
			{
				using (var pen = new Pen(GetArrowLineColor(state), 4))
				{
					graphics.DrawPath(pen, path);
				}
			}
		}
		
		public void RenderInputConnection(Graphics graphics, NodeConnector input, float x, float y, RenderState state)
		{
			if (graphics == null || 
				input == null)
				return;

		    RectangleF inputBounds = input.bounds;

			var x2 = (inputBounds.Left + inputBounds.Right) / 2.0f;
			var y2 = (inputBounds.Top + inputBounds.Bottom) / 2.0f;

			float centerX;
			float centerY;
			using (var path = GraphUtils.GetArrowLinePath(x, y, x2, y2, out centerX, out centerY, true, 0.0f))
			{
				using (var pen = new Pen(GetArrowLineColor(state), 4))
				{
					graphics.DrawPath(pen, path);
				}
			}
		}


	    public Color GetArrowLineColor(RenderState state)
        {
            if ((state & (RenderState.Hover | RenderState.Dragging)) != 0)
            {
                if ((state & RenderState.Incompatible) != 0)
                {
                    return Color.Red;
                }
                else
                    if ((state & RenderState.Compatible) != 0)
                    {
                        return Color.DarkOrange;
                    }
                    else
                        if ((state & RenderState.Dragging) != 0)
                            return Color.SteelBlue;
                        else
                            return Color.DarkOrange;
            }
            else
                if ((state & RenderState.Incompatible) != 0)
                {
                    return Color.Gray;
                }
                else
                    if ((state & RenderState.Compatible) != 0)
                    {
                        return Color.White;
                    }
                    else
                        if ((state & RenderState.Connected) != 0)
                        {
                            return Color.Black;
                        }
                        else
                            return Color.LightGray;
        }
	}
}
