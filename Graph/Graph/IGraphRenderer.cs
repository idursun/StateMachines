using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Graph
{
    public interface IGraphRenderer
    {
        void PerformLayout(Graphics graphics, IEnumerable<Node> nodes);
        void PerformLayout(Graphics graphics, Node node);
        void RenderNode(Graphics graphics, Node node);
        void RenderConnector(Graphics graphics, NodeConnector nodeConnector, RenderState renderState);
        void RenderOutputConnection(Graphics graphics, NodeConnector output, float x, float y, RenderState state);
        void RenderInputConnection(Graphics graphics, NodeConnector input, float x, float y, RenderState state);
        void RenderLabel(Graphics graphics, NodeConnection connection, PointF center, RenderState state);
        Color GetArrowLineColor(RenderState state);
    }
}