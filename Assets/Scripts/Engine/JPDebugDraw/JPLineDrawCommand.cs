using UnityEngine;

namespace JPDebugDraw
{
    public class JPLineDrawCommand : JPDrawCommand
    {
        public Vector3 start;
        public Vector3 end;

        public JPLineDrawCommand(Vector3 start, Vector3 end, Color? color = null)
        {
            this.start = start;
            this.end = end;
            this.color = color ?? Color.white;
        }

        public override void Draw()
        {
            Gizmos.color = color;
            Gizmos.DrawLine(start, end);
        }
    }
}