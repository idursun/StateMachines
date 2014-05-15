using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Graph;

static internal class GraphUtils
{
    public static IEnumerable<NodeItem> EnumerateNodeItems(Node node)
    {
        if (node == null)
            yield break;

        yield return node.titleItem;

        foreach (var item in node.Items)
            yield return item;
    }


    public static Region GetConnectionRegion(NodeConnection connection)
    {
        var to		= connection.To;
        var from	= connection.From;
        RectangleF toBounds = to.bounds;
        RectangleF fromBounds = @from.bounds;

        var x1 = (fromBounds.Left + fromBounds.Right) / 2.0f;
        var y1 = (fromBounds.Top + fromBounds.Bottom) / 2.0f;
        var x2 = (toBounds.Left + toBounds.Right) / 2.0f;
        var y2 = (toBounds.Top + toBounds.Bottom) / 2.0f;

        Region region;
        using (var linePath = GetArrowLinePath(	x1, y1, x2, y2, true, 5.0f))
        {
            region = new Region(linePath);
        }
        return region;
    }

    public static GraphicsPath GetArrowLinePath(float x1, float y1, float x2, float y2,  bool include_arrow, float extra_thickness = 0)
    {
        var newPoints = GetArrowLinePoints(x1, y1, x2, y2, extra_thickness);

        var path = new GraphicsPath(FillMode.Winding);
        path.AddLines(newPoints.ToArray());
        //if (include_arrow)
        //    path.AddLines(GetArrowPoints(x2, y2, extra_thickness).ToArray());
        //path.CloseFigure();
        return path;
    }

    public static List<PointF> GetArrowLinePoints(float x1, float y1, float x2, float y2, float extra_thickness = 0)
    {
        var widthX	= (x2 - x1);
        var lengthX = Math.Max(60, Math.Abs(widthX / 2)) 
            //+ Math.Max(0, -widthX / 2)
            ;
        var lengthY = 0;// Math.Max(-170, Math.Min(-120.0f, widthX - 120.0f)) + 120.0f; 
        if (widthX < 120)
            lengthX = 60;
        var yB = ((y1 + y2) / 2) + lengthY;// (y2 + ((y1 - y2) / 2) * 0.75f) + lengthY;
        var yC = y2 + yB;
        var xC = (x1 + x2) / 2;
        var xA = x1 + lengthX;
        var xB = x2 - lengthX;

        /*
			if (widthX >= 120)
			{
				xA = xB = xC = x2 - 60;
			}
			//*/
			
        var points = new List<PointF>
        { 
            new PointF(x1, y1),
            new PointF(xA, y1),
            new PointF(xB, y2),
            new PointF(x2 - GraphConstants.ConnectorSize - extra_thickness, y2)
        };

        var t  = 1.0f;//Math.Min(1, Math.Max(0, (widthX - 30) / 60.0f));
        var yA = (yB * t) + (yC * (1 - t));

        if (widthX <= 120)
        {
            points.Insert(2, new PointF(xB, yA));
            points.Insert(2, new PointF(xC, yA));
            points.Insert(2, new PointF(xA, yA));
        }
        //*
        using (var tempPath = new GraphicsPath())
        {
            tempPath.AddBeziers(points.ToArray());
            tempPath.Flatten();
            points = tempPath.PathPoints.ToList();
        }
        return points;
    }

    public static PointF[] GetArrowPoints(float x, float y, float extra_thickness = 0)
    {
        return new PointF[]{
            new PointF(x - (GraphConstants.ConnectorSize + 1.0f) - extra_thickness, y + (GraphConstants.ConnectorSize / 1.5f) + extra_thickness),
            new PointF(x + 1.0f + extra_thickness, y),
            new PointF(x - (GraphConstants.ConnectorSize + 1.0f) - extra_thickness, y - (GraphConstants.ConnectorSize / 1.5f) - extra_thickness)};
    }

    public static GraphicsPath CreateRoundedRectangle(SizeF size, PointF location)
    {
        int cornerSize			= (int)GraphConstants.CornerSize * 2;

        var height				= size.Height;
        var width				= size.Width;
        var left				= location.X;
        var top					= location.Y;
        var right				= location.X + width;
        var bottom				= location.Y + height;

        var path = new GraphicsPath(FillMode.Winding);
        path.AddArc(left, top, cornerSize, cornerSize, 180, 90);
        path.AddArc(right - cornerSize, top, cornerSize, cornerSize, 270, 90);

        path.AddArc(right - cornerSize, bottom - cornerSize, cornerSize, cornerSize, 0, 90);
        path.AddArc(left, bottom - cornerSize, cornerSize, cornerSize, 90, 90);
        path.CloseFigure();
        return path;
    }
}