using UnityEngine;

namespace JPDebugDraw
{
    public class JPProjectedBoxDrawCommand : JPBoxDrawCommand
    {
        public JPProjectedBoxDrawCommand(Vector3 pos, Vector3 extents, Color? color = null) : 
            base(JPProjection.projectPoint(pos), JPProjection.projectPoint(extents), color)
        {
            
        }
    }
}