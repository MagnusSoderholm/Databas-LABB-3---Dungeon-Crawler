using Databas_LABB_3___Dungeon_Crawler.LevelElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databas_LABB_3___Dungeon_Crawler.Enemy
{
    public class Rat : Enemy

    {
        public Rat(int x, int y) : base("Rat", 10, 'r', ConsoleColor.Red)
        {
            AttackDice = new Dice(1, 6, 3);
            DefenceDice = new Dice(1, 6, 1);
            X = x;
            Y = y;
        }

        public override void Update(Player player, LevelData levelData)
        {
            Random rand = new Random();
            int direction = rand.Next(4);
            int updateX = 0;
            int updateY = 0;

            switch (direction)
            {
                case 0: updateY = -1; break;
                case 1: updateY = 1; break;
                case 2: updateX = 1; break;
                case 3: updateX = -1; break;
            }
            Move(updateX, updateY, levelData);
        }
    }
}
