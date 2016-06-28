using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Importador
{
    public class Sprite
    {
        static Dictionary<string, TextureData> cache =
            new Dictionary<string, TextureData>();

        Texture2D image;
        protected Vector2 position;
        public Rectangle bbox;
        public string name;

        public Sprite(ContentManager content, string imageName, Vector2 pos)
        {
            name = imageName;
            image = content.Load<Texture2D>(imageName);
            position = new Vector2();
            position.X = pos.X * Game1.unitSize;
            position.Y = -image.Height - pos.Y * Game1.unitSize;

            bbox = new Rectangle(position.ToPoint(),
                new Point(image.Width, image.Height));

            cacheSprite(name, image);
        }

        static void cacheSprite(string name, Texture2D image)
        {
            if (!cache.ContainsKey(name))
                cache.Add(name, new TextureData(image));
        }

        static TextureData Get(string name)
        {
            return cache[name]; // CUIDADO SE O NAME NAO EXISTIR!!!!!!!!! /!\  
        }

        protected void updateBoundingBox(Vector2 pos)
        {
            // estamos a pressupor que o tamanho da imagem
            // nunca muda, portanto, é só atualizar as
            // coordenadas da bounding box.
            bbox.Location = pos.ToPoint();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, position, Color.White);
        }

        // Verifica se sprite atual colide com algum objeto da cena
        // Se colidir, retorna a lista de bounding boxes dos objetos que colidem
        // Se nao colidir retorna lista vazia
        protected List<Sprite> collides()
        {
            // Where: definida no System.LINQ;
            return Game1.scene.Where(el => Intersects(el)).ToList();
        }

        protected List<Sprite> collidee()
        {
            return Game1.enemies.Where(el => Intersects(el)).ToList();
        }

        protected List<Sprite> collidesp()
        { 
            return Game1.spikes.Where(el => Intersects(el)).ToList();
        }

        protected List<Sprite> collidesign()
        {
            return Game1.placa.Where(el => Intersects(el)).ToList();
        }

        protected List<Sprite> collidef()
        {
            return Game1.fim.Where(el => Intersects(el)).ToList();
        }

        // Intersects bounding boxes?
        protected bool Intersects(Sprite other)
        {
            if ( bbox.Intersects(other.bbox) )
            {
                // pixeis colidem??
                if (bbox.Width * bbox.Height < other.bbox.Width * other.bbox.Height)
                {
                    // eu sou mais pequeno...
                    return this.pixelIntersects(other);
                }
                else
                {
                    // other é mais pequeno....
                    return other.pixelIntersects(this);
                }
            }
            else
            {
                return false;
            }
        }

        bool pixelIntersects(Sprite other)
        {
            TextureData our = Sprite.Get(name);
            TextureData yours = Sprite.Get(other.name);

            for (int x = 0; x < our.width; x++)
            {
                for (int y = 0; y < our.height; y++)
                {
                    if (our.At(x,y).A != 0) // ----------------------
                    {
                        // Não é transparente em our.
                        int xl = (int) ( x + bbox.X - other.bbox.X );
                        int yl = (int) ( y + bbox.Y - other.bbox.Y );

                        if (xl >= 0 && yl >= 0 && xl < yours.width && yl < yours.height)
                        {
                            if (yours.At(xl, yl).A != 0) // ----------------------
                            {
                                return true;
                            }                               
                        }
                    }
                }
            } // fim do for
            return false;
        }
    }
}
