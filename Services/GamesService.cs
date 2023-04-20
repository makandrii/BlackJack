using BlackJackApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BlackJackApi.Services;

public class GamesService
{
    private readonly IMongoCollection<Game> _gamesCollection;

    public GamesService()
    {
        var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));

        var mongoDatabase = mongoClient.GetDatabase("BlackJack");

        _gamesCollection = mongoDatabase.GetCollection<Game>("Games");
    }

    public async Task<List<Game>> GetAsync() =>
        await _gamesCollection.Find(_ => true).ToListAsync();

    public async Task<Game?> GetAsync(string id) =>
        await _gamesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Game newGame) =>
        await _gamesCollection.InsertOneAsync(newGame);

    public async Task UpdateAsync(string id, Game updatedGame) =>
        await _gamesCollection.ReplaceOneAsync(x => x.Id == id, updatedGame);

    public async Task RemoveAsync(string id) =>
        await _gamesCollection.DeleteOneAsync(x => x.Id == id);
}