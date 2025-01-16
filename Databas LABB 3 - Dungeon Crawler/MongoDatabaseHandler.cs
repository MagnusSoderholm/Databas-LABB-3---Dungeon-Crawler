using MongoDB.Driver;
using MongoDB.Bson;
using Databas_LABB_3___Dungeon_Crawler.Enemy;
using Databas_LABB_3___Dungeon_Crawler.LevelElement;

public class MongoDatabaseHandler
{
    private readonly IMongoCollection<BsonDocument> gameCollection;

    public MongoDatabaseHandler()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("MagnusSöderholm");
        gameCollection = database.GetCollection<BsonDocument>("GameState");
    }

    public void SaveGame(LevelData levelData)
    {
        gameCollection.DeleteMany(FilterDefinition<BsonDocument>.Empty);

        var playerDoc = new BsonDocument
        {
            { "Type", "Player" },
            { "Name", levelData.player.Name },
            { "X", levelData.player.X },
            { "Y", levelData.player.Y },
            { "Health", levelData.player.Health },
            { "Moves", levelData.player.Moves },
            { "AttackDice", levelData.player.AttackDice.ToString() },
            { "DefenceDice", levelData.player.DefenceDice.ToString() }
        };

        gameCollection.InsertOne(playerDoc);

        foreach (var element in levelData.Elements)
        {
            if (element is Wall wall)
            {
                var wallDoc = new BsonDocument
                {
                    { "Type", "Wall" },
                    { "X", wall.X },
                    { "Y", wall.Y },
                    { "IsDiscovered", wall.IsDiscovered }
                };
                gameCollection.InsertOne(wallDoc);
            }
            else if (element is Enemy enemy)
            {
                var enemyDoc = new BsonDocument
                {
                    { "Type", enemy.GetType().Name },
                    { "Name", enemy.Name },
                    { "X", enemy.X },
                    { "Y", enemy.Y },
                    { "Health", enemy.Health },
                    { "AttackDice", enemy.AttackDice.ToString() },
                    { "DefenceDice", enemy.DefenceDice.ToString() }
                };
                gameCollection.InsertOne(enemyDoc);
            }
        }
    }

    public void LoadGame(LevelData levelData)
    {
        var documents = gameCollection.Find(FilterDefinition<BsonDocument>.Empty).ToList();
        levelData.Elements.Clear();

        foreach (var doc in documents)
        {
            string type = doc["Type"].AsString;

            switch (type)
            {
                case "Player":
                    levelData.player = new Player(doc["X"].AsInt32, doc["Y"].AsInt32)
                    {
                        Name = doc["Name"].AsString,
                        Health = doc["Health"].AsInt32,
                        Moves = doc["Moves"].AsInt32
                    };
                    levelData.Elements.Add(levelData.player);
                    break;

                case "Wall":
                    var wall = new Wall(doc["X"].AsInt32, doc["Y"].AsInt32)
                    {
                        IsDiscovered = doc["IsDiscovered"].AsBoolean
                    };
                    levelData.Elements.Add(wall);
                    break;

                case "Rat":
                    var rat = new Rat(doc["X"].AsInt32, doc["Y"].AsInt32)
                    {
                        Health = doc["Health"].AsInt32
                    };
                    levelData.Elements.Add(rat);
                    break;

                case "Snake":
                    var snake = new Snake(doc["X"].AsInt32, doc["Y"].AsInt32)
                    {
                        Health = doc["Health"].AsInt32
                    };
                    levelData.Elements.Add(snake);
                    break;
            }
        }
    }
}
