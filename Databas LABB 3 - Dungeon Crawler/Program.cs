using Databas_LABB_3___Dungeon_Crawler.Enemy;
using Databas_LABB_3___Dungeon_Crawler.LevelElement;

class Program
{

    static void Main(string[] args)
    {
        Game game = new Game();
        game.Run();
    }
    public class Game
    {
        private string battleText = string.Empty;
        private LevelData levelData = new LevelData();
        private bool _isRunning = true;
        private MongoDatabaseHandler databaseHandler = new MongoDatabaseHandler();

        public void Run()
        {
            Console.WriteLine("Would you like to continue from your last saved game? (Y/N)");
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                databaseHandler.LoadGame(levelData);
            }
            else
            {
                levelData.Load("Level1.txt");
            }

            while (_isRunning)
            {
                Draw();
                PlayerInput();
                Update();
            }

        }


        private void Draw()
        {
            Console.Clear();

            PlayerStats();

            Console.WriteLine(battleText);

            battleText = string.Empty;


            foreach (var element in levelData.Elements)
            {
                double distance = Math.Sqrt(Math.Pow(levelData.player.X - element.X, 2) + Math.Pow(levelData.player.Y - element.Y, 2));

                if (element is Enemy enemy && !enemy.ShouldDraw)
                {
                    levelData.RemoveEnemy(enemy);
                    continue;
                }

                if (distance <= 5)
                {
                    element.Draw();

                    if (element is Wall wall)
                    {
                        wall.IsDiscovered = true;
                    }
                }
                else if (element is Wall wall && wall.IsDiscovered)
                {
                    wall.Draw();
                }
            }
        }
        public void PlayerInput()
        {
            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.Escape)
            {
                Console.WriteLine("\nDo you want to save the game before exiting? (Y/N)");


                var saveInput = Console.ReadKey(true).Key;

                if (saveInput == ConsoleKey.Y)
                {
                    Console.WriteLine("\nSaving the game...");
                    databaseHandler.SaveGame(levelData);
                    Console.WriteLine("Game saved. Exiting...");
                }
                else
                {
                    Console.WriteLine("\nExiting without saving...");

                }

                _isRunning = false;
                return;
            }

            levelData.player.Input(key, levelData);
        }


        public void Update()
        {
            var elementsCopy = new List<LevelElement>(levelData.Elements);

            foreach (var element in elementsCopy)
            {
                if (element is Enemy enemy)
                {
                    enemy.Update(levelData.player, levelData);

                    if (Math.Abs(levelData.player.X - enemy.X) + Math.Abs(levelData.player.Y - enemy.Y) == 1)
                    {
                        BattleEngine(levelData.player, enemy, levelData);

                        if (!enemy.IsAlive)
                        {
                            levelData.RemoveEnemy(enemy);
                        }
                    }
                }
            }
        }

        public void PlayerStats()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Name: {levelData.player.Name} ||| ");
            Console.Write($"Health: {levelData.player.Health} ||| ");
            Console.WriteLine($"Moves: {levelData.player.Moves} ||| ");
            Console.ResetColor();
        }

        public void BattleEngine(Player player, Enemy enemy, LevelData levelData)
        {
            string battleOutcome = "";

            int playerX = player.X;
            int playerY = player.Y;
            int enemyX = enemy.X;
            int enemyY = enemy.Y;

            var elementAtEnemyPosition = levelData.GetElementAt(enemyX, enemyY);
            var elementAtPlayerPosition = levelData.GetElementAt(playerX, playerY);

            if (elementAtEnemyPosition == enemy)
            {
                int playerAttackPoints = player.AttackDice.Throw();
                int enemyDefencePoints = enemy.DefenceDice.Throw();
                int damageToEnemy = playerAttackPoints - enemyDefencePoints;

                battleOutcome += $"You (ATK: {player.AttackDice} => {playerAttackPoints}) attacked the {enemy.Name} (DEF: {enemy.DefenceDice} => {enemyDefencePoints}), {DescribeEnemyDamage(damageToEnemy)}\n";

                if (damageToEnemy > 0)
                {
                    enemy.Health -= damageToEnemy;
                }

                if (enemy.Health <= 0)
                {
                    battleOutcome += $"The {enemy.Name} has been defeated!\n";
                    enemy.ShouldDraw = false;
                    levelData.RemoveEnemy(enemy);
                    battleText = battleOutcome;
                    return;
                }

                int enemyAttackPoints = enemy.AttackDice.Throw();
                int playerDefencePoints = player.DefenceDice.Throw();
                int damageToPlayer = enemyAttackPoints - playerDefencePoints;

                battleOutcome += $"The {enemy.Name} (ATK: {enemy.AttackDice} => {enemyAttackPoints}) attacked you (DEF: {player.DefenceDice} => {playerDefencePoints}), {DescribePlayerDamage(damageToPlayer)}\n";

                if (damageToPlayer > 0)
                {
                    player.Health -= damageToPlayer;

                    if (player.Health <= 0)
                    {
                        Console.Clear();
                        Console.WriteLine("You have been defeated!");
                        Console.WriteLine("Would you like to continue from your last saved game? (Y/N)");


                        var key = Console.ReadKey(true).Key;

                        if (key == ConsoleKey.Y)
                        {
                            Console.WriteLine("Loading last saved game...");
                            databaseHandler.LoadGame(levelData);
                            return;
                        }
                        else if (key == ConsoleKey.N)
                        {
                            Console.Clear();
                            Console.WriteLine("GAME OVER!");
                            Console.WriteLine("Exiting the game...");
                            Environment.Exit(0);
                        }

                    }
                }
            }
            else if (elementAtPlayerPosition == player)
            {

                int enemyAttackPoints = enemy.AttackDice.Throw();
                int playerDefencePoints = player.DefenceDice.Throw();
                int damageToPlayer = enemyAttackPoints - playerDefencePoints;

                battleOutcome += $"The {enemy.Name} (ATK: {enemy.AttackDice} => {enemyAttackPoints}) attacked you (DEF: {player.DefenceDice} => {playerDefencePoints}), {DescribePlayerDamage(damageToPlayer)}\n";

                if (damageToPlayer > 0)
                {
                    player.Health -= damageToPlayer;

                    if (player.Health <= 0)
                    {
                        battleOutcome += "You have been defeated! Game Over.\n";
                        Console.ReadKey();
                        Environment.Exit(0);
                        return;
                    }
                }

                int playerAttackPoints = player.AttackDice.Throw();
                int enemyDefencePoints = enemy.DefenceDice.Throw();
                int damageToEnemy = playerAttackPoints - enemyDefencePoints;

                battleOutcome += $"You (ATK: {player.AttackDice} => {playerAttackPoints}) attacked the {enemy.Name} (DEF: {enemy.DefenceDice} => {enemyDefencePoints}), {DescribeEnemyDamage(damageToEnemy)}\n";

                if (damageToEnemy > 0)
                {
                    enemy.Health -= damageToEnemy;
                }

                if (enemy.Health <= 0)
                {
                    battleOutcome += $"The {enemy.Name} has been defeated!\n";
                    enemy.ShouldDraw = false;
                    levelData.RemoveEnemy(enemy);
                }
            }

            battleText = battleOutcome;
        }

        private string DescribeEnemyDamage(int damageToEnemy)
        {
            if (damageToEnemy > 10)
            {
                return "severely wounding it.";
            }
            else if (damageToEnemy > 5)
            {
                return "moderately wounding it.";
            }
            else if (damageToEnemy > 0)
            {
                return "slightly wounding it.";
            }
            else
            {
                return "but it was not effective.";
            }
        }

        private string DescribePlayerDamage(int damageToPlayer)
        {
            if (damageToPlayer > 10)
            {
                return "severely wounding you.";
            }
            else if (damageToPlayer > 5)
            {
                return "moderately wounding you.";
            }
            else if (damageToPlayer > 0)
            {
                return "slightly wounding you.";
            }
            else
            {
                return "but it was not effective.";
            }
        }
    }
}
