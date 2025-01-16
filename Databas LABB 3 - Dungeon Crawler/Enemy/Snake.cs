using Databas_LABB_3___Dungeon_Crawler.LevelElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databas_LABB_3___Dungeon_Crawler.Enemy
{
    public class Snake : Enemy
    {
        public Snake(int x, int y) : base("Snake", 25, 's', ConsoleColor.Green)
        {
            AttackDice = new Dice(3, 4, 2);
            DefenceDice = new Dice(1, 8, 5);
            X = x;
            Y = y;
        }
        public override void Update(Player player, LevelData levelData)
        {
            int distance = (int)Math.Sqrt(Math.Pow(player.X - X, 2) + Math.Pow(player.Y - Y, 2));

            if (distance <= 2)
            {
                int updateX = 0;
                int updateY = 0;

                if (player.X < X) updateX = 1;
                else if (player.X > X) updateX = -1;

                if (player.Y < Y) updateY = 1;
                else if (player.Y > Y) updateY = -1;

                Move(updateX, updateY, levelData);
            }
        }
    }
}
