using MongoDB.Driver;
using MongoDB.Bson;
using Databas_LABB_3___Dungeon_Crawler.Enemy;
using Databas_LABB_3___Dungeon_Crawler.LevelElement;

public class MongoDatabaseHandler
{
    private readonly IMongoCollection<BsonDocument> playerCollection;
    private readonly IMongoCollection<BsonDocument> wallCollection;
    private readonly IMongoCollection<BsonDocument> snakeCollection;
    private readonly IMongoCollection<BsonDocument> ratCollection;

    public MongoDatabaseHandler()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("MagnusSöderholm");

        playerCollection = database.GetCollection<BsonDocument>("Player");
        wallCollection = database.GetCollection<BsonDocument>("Walls");
        snakeCollection = database.GetCollection<BsonDocument>("Snakes");
        ratCollection = database.GetCollection<BsonDocument>("Rats");
    }

    public bool IsGameSaved()
    {
        return playerCollection.Find(FilterDefinition<BsonDocument>.Empty).Any();
    }

    public void SaveGame(LevelData levelData)
    {
        playerCollection.DeleteMany(FilterDefinition<BsonDocument>.Empty);
        wallCollection.DeleteMany(FilterDefinition<BsonDocument>.Empty);
        snakeCollection.DeleteMany(FilterDefinition<BsonDocument>.Empty);
        ratCollection.DeleteMany(FilterDefinition<BsonDocument>.Empty);

        var playerDoc = new BsonDocument
        {
            { "Name", levelData.player.Name },
            { "X", levelData.player.X },
            { "Y", levelData.player.Y },
            { "Health", levelData.player.Health },
            { "Moves", levelData.player.Moves },
            { "AttackDice", levelData.player.AttackDice.ToString() },
            { "DefenceDice", levelData.player.DefenceDice.ToString() }
        };
        playerCollection.InsertOne(playerDoc);

        foreach (var element in levelData.Elements.OfType<Wall>())
        {
            var wallDoc = new BsonDocument
            {
                { "Type", element.GetType().Name },
                { "X", element.X },
                { "Y", element.Y },
                { "IsDiscovered", element.IsDiscovered }
            };
            wallCollection.InsertOne(wallDoc);
        }

        foreach (var snake in levelData.Elements.OfType<Snake>())
        {
            var snakeDoc = new BsonDocument
            {
                { "Name", snake.Name },
                { "X", snake.X },
                { "Y", snake.Y },
                { "Health", snake.Health },
                { "AttackDice", snake.AttackDice.ToString() },
                { "DefenceDice", snake.DefenceDice.ToString() }
            };
            snakeCollection.InsertOne(snakeDoc);
        }

        foreach (var rat in levelData.Elements.OfType<Rat>())
        {
            var ratDoc = new BsonDocument
            {
                { "Name", rat.Name },
                { "X", rat.X },
                { "Y", rat.Y },
                { "Health", rat.Health },
                { "AttackDice", rat.AttackDice.ToString() },
                { "DefenceDice", rat.DefenceDice.ToString() }
            };
            ratCollection.InsertOne(ratDoc);
        }
    }

    public void LoadGame(LevelData levelData)
    {
        var playerDoc = playerCollection.Find(FilterDefinition<BsonDocument>.Empty).FirstOrDefault();
        if (playerDoc != null)
        {
            levelData.player = new Player(playerDoc["X"].AsInt32, playerDoc["Y"].AsInt32)
            {
                Name = playerDoc["Name"].AsString,
                Health = playerDoc["Health"].AsInt32,
                Moves = playerDoc["Moves"].AsInt32
            };
            levelData.Elements.Add(levelData.player);
        }

        var wallDocs = wallCollection.Find(FilterDefinition<BsonDocument>.Empty).ToList();
        foreach (var doc in wallDocs)
        {
            var wall = new Wall(doc["X"].AsInt32, doc["Y"].AsInt32)
            {
                IsDiscovered = doc["IsDiscovered"].AsBoolean
            };
            levelData.Elements.Add(wall);
        }

        var snakeDocs = snakeCollection.Find(FilterDefinition<BsonDocument>.Empty).ToList();
        foreach (var doc in snakeDocs)
        {
            var snake = new Snake(doc["X"].AsInt32, doc["Y"].AsInt32)
            {
                Health = doc["Health"].AsInt32
            };
            levelData.Elements.Add(snake);
        }

        var ratDocs = ratCollection.Find(FilterDefinition<BsonDocument>.Empty).ToList();
        foreach (var doc in ratDocs)
        {
            var rat = new Rat(doc["X"].AsInt32, doc["Y"].AsInt32)
            {
                Health = doc["Health"].AsInt32
            };
            levelData.Elements.Add(rat);
        }
    }
}
