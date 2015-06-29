using System.Collections.Generic;
using System.Drawing;

namespace Graph
{
    public interface IGraphRenderer
    {
        void RenderNode(Graphics graphics, Node node);
        void RenderConnector(Graphics graphics, NodeConnector nodeConnector, RenderState renderState);
        void RenderOutputConnection(Graphics graphics, NodeConnector output, float x, float y, RenderState state);
        void RenderInputConnection(Graphics graphics, NodeConnector input, float x, float y, RenderState state);
        void RenderLabel(Graphics graphics, NodeConnection connection, RenderState state);
        void RenderConnection(Graphics graphics, NodeConnection connection);
    }
}