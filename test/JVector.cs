#region
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
#endregion
namespace test
{
    class JVector
    {
        public static void DrawPentagram(Obj_AI_Base target, Color Color,float radius = 500, int tickness = 1)
        {
            Vector2 centre = target.Position.To2D();

            Vector2 Top = centre + new Vector2(0, radius);
            Vector2 Rightdown = centre + new Vector2((float)(Math.Cos(Deg2Rid(54)) * radius), -(float)(Math.Sin(Deg2Rid(54)) * radius));
            Vector2 Leftdown = centre + new Vector2((float)-(Math.Cos(Deg2Rid(54)) * radius), -(float)(Math.Sin(Deg2Rid(54)) * radius));
            Vector2 Righttop = centre + new Vector2((float)(Math.Cos(Deg2Rid(18)) * radius), (float)(Math.Sin(Deg2Rid(18)) * radius));
            Vector2 Lefttop = centre + new Vector2((float)-(Math.Cos(Deg2Rid(18)) * radius), (float)(Math.Sin(Deg2Rid(18)) * radius));

            Vector2 top = Drawing.WorldToScreen(Top.To3D());
            Vector2 rightdown = Drawing.WorldToScreen(Rightdown.To3D());
            Vector2 leftdown = Drawing.WorldToScreen(Leftdown.To3D());
            Vector2 righttop = Drawing.WorldToScreen(Righttop.To3D());
            Vector2 lefttop = Drawing.WorldToScreen(Lefttop.To3D());

            Drawing.DrawLine(top, leftdown,tickness, Color.White);
            Drawing.DrawLine(leftdown, righttop, tickness, Color.White);
            Drawing.DrawLine(righttop, lefttop, tickness, Color.White);
            Drawing.DrawLine(rightdown, lefttop, tickness, Color.White);
            Drawing.DrawLine(rightdown, top, tickness, Color.White);

            Utility.DrawCircle(target.Position, radius, Color.White,tickness,1);
        }

        public static Vector3 Front(Obj_AI_Base target, float Distance)
        {
            var pos = (target.Position.To2D() + Distance * target.Direction.To2D().Perpendicular());
            var pos_3D = new Vector3(pos.X, pos.Y, target.Position.Z);
            return pos_3D;
        }
        public static Vector3 Back(Obj_AI_Base target, float Distance)
        {
            var pos = (target.Position.To2D() + Distance * target.Direction.To2D().Perpendicular2());
            var pos_3D = new Vector3(pos.X, pos.Y, target.Position.Z);
            return pos_3D;
        }
        public static Vector3 Right(Obj_AI_Base target, float Distance)
        {
            var pos = (target.Position.To2D() + Distance * target.Direction.To2D());
            var pos_3D = new Vector3(pos.X, pos.Y, target.Position.Z);
            return pos_3D;
        }
        public static Vector3 Left(Obj_AI_Base target, float Distance)
        {
            var pos = (target.Position.To2D() - Distance * target.Direction.To2D());
            var pos_3D = new Vector3(pos.X, pos.Y, target.Position.Z);
            return pos_3D;
        }
        public static Vector3 ForCursor(Obj_AI_Base target, float Distance)
        {
            var pos = (target.Position.Extend(Game.CursorPos, Distance));
            return pos;
        }


        private static float Deg2Rid(float deg)
        {
            return (float)(deg * Math.PI / 180);        
        }
    }
}
