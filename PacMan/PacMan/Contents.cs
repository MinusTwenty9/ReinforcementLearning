using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PacMan
{
    public static class Contents
    {
        // Textures
        public static Texture2D tex_cheese;
        public static Texture2D tex_ghost;
        public static Texture2D tex_ghost_blue;
        public static Texture2D tex_ghost_dead;
        public static Texture2D tex_ghost_killer;
        public static Texture2D tex_pacman;
        public static Texture2D tex_wall;

        // Fonts
        public static SpriteFont text;

        public static void Load_Contents(ContentManager content)
        {
            // Textures
            tex_cheese = content.Load<Texture2D>("Textures/cheese");
            tex_ghost = content.Load<Texture2D>("Textures/ghost_centerd");
            tex_ghost_blue = content.Load<Texture2D>("Textures/ghost_centerd_blue");
            tex_ghost_dead = content.Load<Texture2D>("Textures/ghost_centerd_dead");
            tex_pacman = content.Load<Texture2D>("Textures/pac_man_0");
            tex_wall = content.Load<Texture2D>("Textures/wall");
            tex_ghost_killer = content.Load<Texture2D>("Textures/ghost_killer");

            // Fonts
            text = content.Load<SpriteFont>("Fonts/text");
        }
    }
}
