using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace Importador
{
    public class Game1 : Game
    {
        public static float scale = 0.75f;
        public static float unitSize = 80f; //128 - default for now 80 works
        // esta lista so tem as sprites com as quais ha colisoes
        public static List<Sprite> scene;
        // esta lista tem elementos decorativos com os quais nao ha colisoes
        List<Sprite> background;
        // Lista com os enimigos em que ha colisoes
        public static List<Sprite> enemies;
        public static List<Sprite> spikes;

        public static int nlvl;

        public static bool gameover = false;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        Texture2D pixel;
        SpriteFont arialBlack20;
        Enemy enemy;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.Black });
            gameover = false;
            nlvl = 1;

            loadScene("MainScene" + nlvl + ".json");
            base.Initialize();
        }

        void loadScene(string filename)
        {
            filename = @"Content\" + filename;
            string sceneContents = File.ReadAllText(filename);
            JObject json = JObject.Parse(sceneContents);
            JArray images = (JArray) json["composite"]["sImages"];

            scene = new List<Sprite>();
            background = new List<Sprite>();
            enemies = new List<Sprite>();
            spikes = new List<Sprite>();

            foreach (JObject image in images)
            {
                string imageName = (string)image["imageName"];
                string layerName = (string)image["layerName"];

                float x, y;
                JToken t;
                if (image.TryGetValue("x", out t))
                    x = (float)t;
                else
                    x = 0f;
                if (image.TryGetValue("y", out t))
                    y = (float)t;
                else
                    y = 0f;

                if (imageName == "player")
                    player = new Player(Content, imageName, new Vector2(x, y));
                else if (imageName == "enemy")
                    enemies.Add(enemy = new Enemy(Content, imageName, new Vector2(x, y), player));
                else if (imageName == "3spikes")
                    spikes.Add(new Sprite(Content, imageName, new Vector2(x, y)));
                else if (layerName == "background")
                    background.Add(new Sprite(Content, imageName, new Vector2(x, y)));
                else
                    scene.Add(new Sprite(Content, imageName, new Vector2(x, y)));
            }
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            arialBlack20 = Content.Load<SpriteFont>("ArialBlack_20");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime);
            foreach (Enemy enemy in enemies)
            {
                Vector2 posenemy = enemy.GetPosition();
                Vector2 posplaye = player.GetPosition();
                if(posenemy.X < GraphicsDevice.Viewport.Width + posplaye.X)
                {
                    enemy.Update(gameTime);
                }
            }
            base.Update(gameTime);

            KeyboardState keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.R))
            {
                Initialize();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Vector2 posPlayer = player.GetPosition()
                + new Vector2(player.bbox.Width, player.bbox.Height) / 2;

            spriteBatch.Begin(transformMatrix:
                Matrix.CreateTranslation(-posPlayer.X, -posPlayer.Y, 0)
                *
                Matrix.CreateScale(scale)
                *
                Matrix.CreateTranslation(GraphicsDevice.Viewport.Width/2,
                                         GraphicsDevice.Viewport.Height/2, 0)   
            );

            // Desenhar objetos decorativos
            foreach (Sprite sprite in background)
                sprite.Draw(spriteBatch);
            
            // Desenha objetos com colisoes
            foreach (Sprite sprite in scene)            
                sprite.Draw(spriteBatch);

            // Desenha inimigos
            foreach (Enemy enemy in enemies)
                enemy.Draw(spriteBatch);

            // Desenha spikes
            foreach (Sprite spikes in spikes)
                spikes.Draw(spriteBatch);
           
            player.Draw(spriteBatch);
            spriteBatch.End();

            if (gameover)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(pixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(Color.Black, 1f));

                Vector2 strSize = arialBlack20.MeasureString("YOU LOSE!!");
                spriteBatch.DrawString(arialBlack20, "YOU LOSE!!", (new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) - strSize) * 0.5f, Color.DarkGreen);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
