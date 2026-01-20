using System;
using System.Collections.Generic;
using UnityEngine;

namespace JPDebugDraw
{
    public class JPDebugDrawer : MonoBehaviour
    {
        private static Stack<JPDrawCommand> DrawCommands = new();

        public static void AddCommand(JPDrawCommand command)
        {
            DrawCommands.Push(command);
        }

        private void OnDrawGizmos()
        {
            while (DrawCommands.TryPop(out JPDrawCommand command))
                command.Draw();
        }
    }
}