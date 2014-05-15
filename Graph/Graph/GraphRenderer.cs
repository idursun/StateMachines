﻿#region License
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
	    public void RenderNode(Graphics graphics, Node node)
	    {
	        var size = node.bounds.Size;
	        var position = node.bounds.Location;

	        const int cornerSize = GraphConstants.CornerSize*2;
	        const int connectorSize = GraphConstants.ConnectorSize;
	        int halfConnectorSize = (int) Math.Ceiling(connectorSize/2.0f);
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

	            using (SolidBrush brush = new SolidBrush(node.BackColor))
	            {
	                graphics.FillPath(brush, path);
	            }

                Color borderColor = Color.FromArgb(64, 64, 64);
                if (node.state.HasFlag(RenderState.Focus) && node.state.HasFlag(RenderState.Dragging))
	                borderColor = Color.Yellow;
	            else if ((node.state & RenderState.Hover) == RenderState.Hover)
	                borderColor = Color.Orange;

	            using (Pen pen = new Pen(borderColor, 2))
                {
                    graphics.DrawPath(pen, path);
                }
	        }
	     
	        var itemPosition = position;
	        itemPosition.X += connectorSize + (int) GraphConstants.HorizontalSpacing;
	        node.inputBounds = Rectangle.Empty;
	        node.outputBounds = Rectangle.Empty;

	        var minimumItemSize = new SizeF(node.bounds.Width - GraphConstants.NodeExtraWidth, 0);
	        foreach (var item in GraphUtils.EnumerateNodeItems(node))
	        {
                item.Render(graphics, minimumItemSize, itemPosition);
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


	    public void RenderLabel(Graphics graphics, NodeConnection connection, RenderState state)
		{
            var center = new PointF(0, 0);

			using (var path = new GraphicsPath(FillMode.Winding))
			{			
				const int cornerSize = GraphConstants.CornerSize * 2;
				const int connectorSize = GraphConstants.ConnectorSize;
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
			
			using (var path = GraphUtils.GetArrowLinePath(x1, y1, x, y, true, 0.0f))
			{
				using (var pen = new Pen(GetArrowLineColor(state), 4))
				{
					graphics.DrawPath(pen, path);
				}
			}
		}
		
		public void RenderInputConnection(Graphics graphics, NodeConnector input, float x, float y, RenderState state)
		{
			if (graphics == null || input == null)
				return;

		    RectangleF inputBounds = input.bounds;

			var x2 = (inputBounds.Left + inputBounds.Right) / 2.0f;
			var y2 = (inputBounds.Top + inputBounds.Bottom) / 2.0f;

		    using (var path = GraphUtils.GetArrowLinePath(x, y, x2, y2, true))
		    using (var pen = new Pen(GetArrowLineColor(state), 4))
		    {
		        graphics.DrawPath(pen, path);
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

	    public void RenderConnection(Graphics graphics, NodeConnection connection)
	    {
            var to = connection.To;
            var from = connection.From;
            RectangleF toBounds = to.bounds;
            RectangleF fromBounds = @from.bounds;

            var x1 = (fromBounds.Left + fromBounds.Right) / 2.0f;
            var y1 = (fromBounds.Top + fromBounds.Bottom) / 2.0f;
            var x2 = (toBounds.Left + toBounds.Right) / 2.0f;
            var y2 = (toBounds.Top + toBounds.Bottom) / 2.0f;

            using (var path = GraphUtils.GetArrowLinePath(x1, y1, x2, y2, false))
            {
                using (var brush = new SolidBrush(GetArrowLineColor(connection.state | RenderState.Connected)))
                {
                    graphics.DrawPath(new Pen(brush, 4), path);
                }
                connection.bounds = path.GetBounds();
            }
	    }
	}
}
