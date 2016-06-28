using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Importador
{
    class Player : Sprite
    {
        float jumpTime = 0f; //tempo do salto
        public float dashcooldown = 5f;
        bool onGround = false; //variavel para verificar se ele esta no chao

        public static bool isFinish = false;
        public float totaltimer;
        int directionfaced = 1;
        float accel = 0;
        float maxaccel = 225f;

        float accelerationY = 10f;//Froça da gravidade
        float velocityY = 0f;

        SoundEffect jumpSound; //som do salto

        public Player(ContentManager content,
            string imagename, Vector2 position) :
            base(content, imagename, position)
        {
            jumpSound = content.Load<SoundEffect>("Mario Jump - Sound Effect");
        }

        public void Update(GameTime gameTime)
        {
            // Gravidade
            velocityY += accelerationY *
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 target = position;
            target.Y += velocityY;
            updateBoundingBox(target);

            if (!Game1.win || Game1.gameover)
            {
                totaltimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            dashcooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;

            List<Sprite> colSprites = collides();
            List<Sprite> colEnemies = collidee();
            List<Sprite> colSpikes = collidesp();
            List<Sprite> colPlaca = collidesign();
            List<Sprite> colFim = collidef();

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
                position.Y = colSprites.Min(sprite => sprite.bbox.Top) - bbox.Height;
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
            if(position.Y > 100)
            {
                Game1.gameover = true;
            }
            if (colPlaca.Count != 0)
            {
                Game1.nlvl = 2;
            }
            if (colFim.Count != 0)
            {
                Game1.win = true;
            }

            KeyboardState keyboard = Keyboard.GetState();
            target = position;

            if (keyboard.IsKeyDown(Keys.Left)) // Left
            {
                if (directionfaced == 1)
                {
                    accel = 0f;
                }
                if (accel <= maxaccel)
                {
                    accel += 10;
                    target.X -= accel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    directionfaced = 0;
                    if (accel >= maxaccel)
                    {
                        accel = maxaccel;
                    }
                }
            }
            else if (keyboard.IsKeyDown(Keys.Right)) // Right
            {
                if (directionfaced == 0)
                {
                    accel = 0f;
                }
                if (accel <= maxaccel)
                {
                    accel += 10;
                    target.X += accel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    directionfaced = 1;
                    if (accel >= maxaccel)
                    {
                        accel = maxaccel;
                    }
                }
            }
            else
            {
                if (accel >= 10)
                    accel -= 10;
                if (accel == 10)
                    accel = 0;

                if (directionfaced == 1 && accel != 0)
                {
                    target.X += accel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if(directionfaced == 0 && accel != 0)
                {
                    target.X -= accel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }           

            if (dashcooldown >= 5f && keyboard.IsKeyDown(Keys.X))
            {
                if (directionfaced == 1) // 1 - right
                {
                    dashcooldown = 0;
                    target.X += 300f;
                }
                else if(directionfaced == 0) // 0 - left
                {
                    dashcooldown = 0;
                    target.X -= 300f;
                }
            }

            if (onGround && keyboard.IsKeyDown(Keys.Z))
            {
                onGround = false;
                jumpTime = 0.5f;
                jumpSound.Play();
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
