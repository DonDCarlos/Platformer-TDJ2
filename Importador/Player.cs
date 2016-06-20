using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Importador
{
    class Player : Sprite
    {
        float jumpTime = 0f;
        bool onGround = false;

        float accelerationY = 10f;//Froça da gravidade
        float velocityY = 0f;
            
        public Player(ContentManager content,
            string imagename, Vector2 position) :
            base(content, imagename, position)
        {
            this.position.X += 1.5f;
        }

        public void Update(GameTime gameTime)
        {
            // Gravidade
            velocityY += accelerationY *
                (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 target = position;
            target.Y += velocityY;
            updateBoundingBox(target);

            List<Sprite> colSprites = collides();
            List<Sprite> colEnemies = collidee();
            List<Sprite> colSpikes = collidesp();

            if (colSprites.Count == 0)
            {
                // nenhuma colisao
                position = target;
                onGround = false;
            }
            else
            {
                // colidimos!!!
                onGround = true;
                jumpTime = 0f;
                // Posicao passa a ser baseada na colisao mais acima (com topo minimo)
                position.Y = colSprites.Min(sprite => sprite.bbox.Top) - bbox.Height  ;
                velocityY = 0f;
            }

            if(colEnemies.Count != 0)
            {
                Game1.gameover = true;
            }

            if(colSpikes.Count != 0)
            {
                Game1.gameover = true;
            }

            KeyboardState keyboard = Keyboard.GetState();
            target = position;
            if (keyboard.IsKeyDown(Keys.Left)) // Left
                target.X -= 225f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboard.IsKeyDown(Keys.Right)) // Right
                target.X += 225f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Jump!!!!
            if (onGround && keyboard.IsKeyDown(Keys.Z))
            {
                onGround = false;
                jumpTime = 0.5f;
            }
            if (jumpTime > 0f)
            {
                target.Y -= 40f * jumpTime;
                jumpTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (target != position) // if we move
            {
                updateBoundingBox(target);
                if (collides().Count == 0) // if we do not collide
                    position = target;
                else
                    jumpTime = 0f;
            }
        }

        public Vector2 GetPosition()
        {
            return position;
        }
    }
}
