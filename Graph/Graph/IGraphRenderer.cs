using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Graph
{
    public interface IGraphRenderer
    {
        void PerformLayout(Graphics graphics, IEnumerable<Node> nodes);
        void PerformLayout(Graphics graphics, Node node);
        void RenderConnections(Graphics graphics, Node node, HashSet<NodeConnection> skipConnections, bool showLabels);
        Region GetConnectionRegion(NodeConnection connection);
        void RenderOutputConnection(Graphics graphics, NodeConnector output, float x, float y, RenderState state);
        void RenderInputConnection(Graphics graphics, NodeConnector input, float x, float y, RenderState state);
        void Render(Graphics graphics, IEnumerable<Node> nodes, bool showLabels);
    }
}