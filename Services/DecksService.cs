using MongoDB.Driver;
using BlackJackApi.Models;
using Microsoft.Extensions.Options;

namespace BlackJackApi.Services;

public class DecksService
{
    private readonly IMongoCollection<Deck> _decksCollection;

    public DecksService()
    {
        var mongoClient = new MongoClient(Environment.GetEnvironmentVariable("MONGODB_URI"));

        var mongoDatabase = mongoClient.GetDatabase("BlackJack");

        _decksCollection = mongoDatabase.GetCollection<Deck>("Decks");
    }

    public async Task<List<Deck>> GetAsync() =>
        await _decksCollection.Find(_ => true).ToListAsync();

    public async Task<Deck?> GetAsync(string id) =>
        await _decksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Deck newGame) =>
        await _decksCollection.InsertOneAsync(newGame);

    public async Task UpdateAsync(string id, Deck updatedGame) =>
        await _decksCollection.ReplaceOneAsync(x => x.Id == id, updatedGame);

    public async Task RemoveAsync(string id) =>
        await _decksCollection.DeleteOneAsync(x => x.Id == id);

    public async Task WipeAsync() =>
        await _decksCollection.DeleteManyAsync(_ => true);
}
