using UnityEngine;

namespace JPDebugDraw
{
    public class JPBoxDrawCommand : JPDrawCommand
    {
        public Vector3 position;
        public Vector3 extents;

        public JPBoxDrawCommand(Vector3 position, Vector3 extents, Color? color = null)
        {
            this.position = position;
            this.extents = extents;
            this.color = color ?? Color.white;
        }

        public override void Draw()
        {
            Gizmos.color = color;
            Gizmos.DrawCube(position, extents * 2);
        }
    }
}