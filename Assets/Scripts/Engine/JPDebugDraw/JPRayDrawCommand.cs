using UnityEngine;

namespace JPDebugDraw
{
    public class JPRayDrawCommand : JPDrawCommand
    {
        public Vector3 start;
        public Vector3 dir;

        public JPRayDrawCommand(Vector3 start, Vector3 dir, Color? color = null)
        {
            this.start = start;
            this.dir = dir;
            this.color = color ?? Color.white;
        }

        public JPRayDrawCommand(Vector3 start, Vector3 dir, float len, Color? color = null) :
            this(start, dir.normalized * len, color)
        {
            
        }

        public override void Draw()
        {
            Gizmos.color = color;
            Gizmos.DrawRay(start, dir);
        }
    }
}