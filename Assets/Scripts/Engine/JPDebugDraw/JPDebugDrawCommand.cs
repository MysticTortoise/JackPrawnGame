using UnityEngine;

namespace JPDebugDraw
{
    public abstract class JPDrawCommand
    {
        public Color color;

        public abstract void Draw();
    }
}