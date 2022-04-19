using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using RedisProxy.Service.IntegrationTest.Configs;
using StackExchange.Redis;

namespace RedisProxy.Service.IntegrationTest;

public class Tests
{
    private TestConfig _testConfig { get; set; } = new TestConfig();
    private HttpClient _httpClient { get; set; } = new HttpClient();

    private IConnectionMultiplexer  _redis { get; set; }
    private IDatabase _db { get; set; }

    [OneTimeSetUp]
    public void Setup()
    {
        // Set up needed clients and connections
        _testConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json", false, false)
            .AddEnvironmentVariables()
            .Build()
            .Get<TestConfig>();
        _httpClient = new HttpClient { BaseAddress = new Uri(_testConfig.ServiceUri) };
        _redis = ConnectionMultiplexer.Connect($"{_testConfig.Redis.Address}:{_testConfig.Redis.Port}");
        _db = _redis.GetDatabase();

        // Seed data
        _db.StringSet("ping", "pong");
        _db.StringSet("fries", "ketchup");
        _db.StringSet("chuan", "yang");
    }

    [Test]
    public async Task ShouldReturnNotFoundWhenKeyDoesNotExist()
    {
        var response = await _httpClient.GetAsync("/data/ping1");

        Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
    }

    [Test]
    public async Task ShouldReturnTheCorrespondingValueWhenKeyExists()
    {
        var response = await _httpClient.GetAsync("/data/ping");
        var result = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();

        Assert.AreEqual("pong", result);
    }

    [Test]
    public async Task ShouldBeAbleToHandleMultipleRequests()
    {
        var response = await _httpClient.GetAsync("/data/fries");
        var otherResponse = await _httpClient.GetAsync("/data/chuan");
        var badRequest = await _httpClient.GetAsync("/data/bad");

        var result = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
        var otherResult = await otherResponse.EnsureSuccessStatusCode().Content.ReadAsStringAsync();


        Assert.AreEqual("ketchup", result);
        Assert.AreEqual("yang", otherResult);
        Assert.AreEqual(StatusCodes.Status404NotFound, (int)badRequest.StatusCode);
    }
}
