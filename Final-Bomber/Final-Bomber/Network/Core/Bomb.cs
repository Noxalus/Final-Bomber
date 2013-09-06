using Final_Bomber.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_Bomber.Network.Core
{
    /*
    class Bomb : MapItem
    {
        public Bomb(Vector2 pos, float tickSpeed, bool enemy)
            : base(LayerStatus.BehindPlayer)
        {
            tickSpeed = 1000 / tickSpeed;
            DrawOrder = 3.9f;
            //E2D_Texture2D texture = E2D_Engine.TextureManager.GetTextureOrLoad("Entities\\BombAnim");
            this.MapPosition = pos;
            Initialize(texture, (int)tickSpeed, 5, 1);
            SetAnimationLoop(0, 4);
            if (enemy)
                ColorModulation = new Color(255, 100, 100, 255);
        }

        bool stopAnimation = false;
        public override void Draw()
        {
            if (CurrentFrame == 4 && !stopAnimation)
            {
                SetAnimationLoop(4, 4);
                stopAnimation = true;
            }
            base.Draw();
        }
    }

    class BombCollection : List<Bomb>
    {
        public Bomb GetBombByPos(float xPos, float yPos)
        {
            foreach (E2D_Entity bomb in this)
            {
                if (bomb.MapPosition.X == xPos && bomb.MapPosition.Y == yPos)
                    return (Bomb)bomb;
            }
            return null;
        }
    }
    */
}
