using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databas_LABB_3___Dungeon_Crawler.LevelElement
{
    public class Wall : LevelElement
    {
        public bool IsDiscovered { get; set; }
        public Wall(int x, int y)
        {
            Symbol = '#';

            Color = ConsoleColor.Gray;

            X = x;
            Y = y;
            IsDiscovered = false;
        }
    }
}
