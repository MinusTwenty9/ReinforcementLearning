using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReinforcementLearning.QLearning.FunctionApproximation;

namespace PacMan
{
    public class Map
    {
        public float scale = 1.0f;

        public MapType map_type;
        public Vector2 map_size;
        public Entity[,] grid;
        public bool map_init
        {
            get { return grid != null; }
        }
        public int cheese_count;

        public List<Ghost> ghosts;
        public List<Pacman> pacmans;

        private EntityType[,] grid_reset;

        public Map()
        {
            ghosts = new List<Ghost>();
            pacmans = new List<Pacman>();
        }

        public void Draw(SpriteBatch sb)
        {
            for (int y = 0; y < map_size.Y; y++)
                for (int x = 0; x < map_size.X; x++)
                    if (grid[y, x] != null)
                        grid[y,x].Draw(sb,scale);

            for (int i = 0; i < pacmans.Count; i++)
                pacmans[i].Draw(sb, scale);
            for (int i = 0; i < ghosts.Count; i++)
                ghosts[i].Draw(sb,scale);
        }

        public void Update()
        {
            for (int y = 0; y < map_size.Y; y++)
                for (int x = 0; x < map_size.X; x++)
                    if (grid[y, x] != null)
                        grid[y, x].Update();
        }

        public void Ghost_Update()
        {
            for (int i = 0; i < ghosts.Count; i++)
                ghosts[i].Update();
        }

        public void Pacman_Update()
        {
            for (int i = 0; i < pacmans.Count; i++)
                pacmans[i].Update();
        }

        public void Set_Grid(Vector2 pos, EntityType entity_type)
        {
            if (entity_type == EntityType.Wall)
                add_wall(pos);
            else if (entity_type == EntityType.Empty)
                add_empty(pos);
            else if (entity_type == EntityType.Ghost)
                add_ghost(pos);
            else if (entity_type == EntityType.Cheese)
                add_cheese(pos);
            else if (entity_type == EntityType.Pacman)
                add_pacman(pos);
        }

        //  0 = Boarder and rest empty
        public void Create_Map(MapType map_type, Vector2 map_size)
        {
            this.map_size = map_size;
            this.map_type = map_type;
            this.grid = new Entity[(int)map_size.Y, (int)map_size.X];
            this.cheese_count = 0;

            if (map_type == MapType.Border_Empty)
                create_map_boarder_empty();
        }

        public void Grid_Reset()
        {
            ghosts.Clear();
            Create_Custome_Map(grid_reset, map_size);
        }

        #region Creation Functions

        #region Entities
        private void add_wall(Vector2 pos)
        {
            Wall wall = new Wall(pos);
            grid[(int)pos.Y, (int)pos.X] = wall;
        }

        private void add_empty(Vector2 pos)
        {
            Empty empty = new Empty(pos*32);
            grid[(int)pos.Y, (int)pos.X] = empty;
        }

        private void add_cheese(Vector2 pos)
        {
            Cheese cheese = new Cheese(pos);
            grid[(int)pos.Y, (int)pos.X] = cheese;
            cheese_count++;
        }

        private void add_ghost(Vector2 pos)
        {
            Ghost ghost = new Ghost(pos, this);
            ghosts.Add(ghost);
        }

        private void add_pacman(Vector2 pos)
        {
            Pacman pacman = new Pacman(pos,this);
            pacmans.Add(pacman);
        }
        #endregion

        #region Map
        private void create_map_boarder_empty()
        {
            for (int y = 0; y < map_size.Y; y++)
                for (int x = 0; x < map_size.X; x++)
                    if (x == 0 || y == 0 || x == map_size.X-1 || y == map_size.Y-1)
                        Set_Grid(new Vector2(x, y), EntityType.Wall);
                    else
                        Set_Grid(new Vector2(x, y), EntityType.Empty);
        }

        public void Create_Custome_Map(EntityType[,] grid, Vector2 map_size)
        {
            this.map_size = map_size;
            this.grid = new Entity[(int)map_size.Y, (int)map_size.X];
            this.cheese_count = 0;
            this.grid_reset = (EntityType[,])grid.Clone();

            for (int y =0;y < map_size.Y; y++)
                for (int x = 0; x < map_size.X; x++)
                {
                    if (grid[y, x] == EntityType.Ghost)
                        Set_Grid(new Vector2(x, y), EntityType.Cheese);
                    else if (grid[y, x] == EntityType.Pacman)
                    {
                        Set_Grid(new Vector2(x, y), EntityType.Empty);
                        grid_reset[y, x] = EntityType.Empty;
                    }

                    Set_Grid(new Vector2(x, y), grid[y, x]);
                }
        }
        #endregion

        #endregion
    }

    public enum EntityType
    {
        Empty,
        Wall,
        Pacman,
        Ghost,
        Ghost_Dead,
        Gohst_Blue,
        Ghost_Killer,
        Cheese
    }
    public enum MapType
    { 
        Border_Empty
    }
}
