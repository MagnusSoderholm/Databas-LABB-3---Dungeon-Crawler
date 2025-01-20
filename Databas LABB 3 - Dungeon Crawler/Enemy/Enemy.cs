using Databas_LABB_3___Dungeon_Crawler.LevelElement;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databas_LABB_3___Dungeon_Crawler.Enemy
{
    public abstract class Enemy : LevelElement.LevelElement
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public Dice AttackDice { get; set; }
        public Dice DefenceDice { get; set; }
        public bool IsAlive = true;
        public bool ShouldDraw = true;
        public bool IsMovingTowardsPlayer { get; set; }

        public Enemy(string name, int health, char symbol, ConsoleColor color)
        {
            Name = name;
            Health = health;
            Symbol = symbol;
            Color = color;
        }

        public abstract void Update(Player player, LevelData levelData);

        public void MoveTowards(Player player)
        {
            if (DistanceTo(player) > 1)
            {
                IsMovingTowardsPlayer = true;
            }
            else
            {
                IsMovingTowardsPlayer = false;
            }
        }

        public int DistanceTo(Player player)
        {
            return Math.Abs(this.X - player.X) + Math.Abs(this.Y - player.Y);
        }
    }
}
