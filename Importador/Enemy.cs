using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importador
{
    class Enemy : Sprite
    {
        float accelerationY = 50f;
        float velocityY = 0f;
        Player eplayer;

        public Enemy(ContentManager content,
            string imagename, Vector2 position, Player player) :
            base(content, imagename, position)
        {
            eplayer = player;
        }

        public void Update(GameTime gameTime)
        {
            velocityY += accelerationY * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 targete = position;
            targete.Y += velocityY;
            Vector2 targetp;
            updateBoundingBox(targete);

            List<Sprite> colSprites = collides();
            List<Sprite> colEnemies = collidee();

            if (colSprites.Count == 0)
            {
                position = targete;
            }
            else
            {
                position.Y = colSprites.Min(sprite => sprite.bbox.Top) - bbox.Height;
                velocityY = 0f;
            }

            targete = position;
            targetp = eplayer.GetPosition();

            if (targetp.X > targete.X)
            {
                targete.X += 100f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
                targete.X -= 100f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (targete != position) // if we move
            {
                updateBoundingBox(targete);
                if (collides().Count == 0) // if we do not collide
                    position = targete;
            }
        }

        public Vector2 GetPosition()
        {
            return position;
        }
    }
}
