namespace FTPTests;

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using homework4;

[TestFixture]
public class Tests
{
    private Server? server;

    private Client? client;

    private readonly IPEndPoint endPoint = new (IPAddress.Loopback, 8888);

    [OneTimeSetUp]
    public void Setup()
    {
        server = new Server(endPoint.Port);
        client = new Client(endPoint);

        _ = Task.Run(() => server.Run());
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        server!.Stop();
        server.Dispose();
    }

    [Test]
    public async Task List_ValidPath_ReturnsCorrectData()
    {
        var expected = "4 ../../../TestFiles/AlgebraPractice.pdf False ../../../TestFiles/empty.txt False ../../../TestFiles/lol.txt False ../../../TestFiles/sos True\n";


        var actual = await client!.List("../../../TestFiles");


        Assert.That(actual, Is.EqualTo(expected));
    }


    [Test]
    public async Task Get_ValidPath_ReturnsCorrectData()
    {
        var expected = "10 Sasha loh\n";


        var actual = await client!.Get("../../../TestFiles/lol.txt");


        Assert.That(actual, Is.EqualTo(expected));
    }




    [Test]
    public async Task ListAsync_InvalidPath_ReturnsMinusOne()
    {
        var expected = "-1";


        var actual = await client!.List("../../../InvalidPath");


        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetAsync_InvalidPath_ReturnsMinusOne()
    {
        var expected = "-1";


        var actual = await client!.Get("../../../InvalidPath");


        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetAsync_EmptyFile_ReturnsZero()
    {

        var expected = "0 ";


        var actual = await client!.Get("../../../TestFiles/empty.txt");


        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void ManyClients_WithCorrectRequests_ShouldReturnSameResult()
    {
        const string listPath = "../../../TestFiles";
        const string getPath = "../../..//TestFiles/AlgebraPractice.pdf";
        const int clientsNumber = 5;
        const int millisecondsWait = 2000;
        
        var listResults = new string[clientsNumber];
        var getResults = new string[clientsNumber];
        var tasks = new Task[clientsNumber];
        var manualResetEvent = new ManualResetEvent(false);

        
        for (var i = 0; i < clientsNumber; ++i)
        {
            var locali = i;
            tasks[i] = Task.Run(async () =>
            {
                manualResetEvent.WaitOne();
                
                await Task.Delay(millisecondsWait);
                
                var newClient = new Client(endPoint);
                listResults[locali] = await newClient.List(listPath) ?? "Error";
                using var stream = new MemoryStream();
        
                getResults[locali] = await client!.Get(getPath) ?? "Error";
            });
        }
        
        var stopwatch = new Stopwatch();
        
        manualResetEvent.Set();
        stopwatch.Start();
        
        Task.WaitAll(tasks);
        stopwatch.Stop();

        
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(millisecondsWait * clientsNumber));
        
        for (var i = 1; i < clientsNumber; ++i)
        {
            Assert.Multiple(() =>
            {
                Assert.That(listResults[i - 1], Is.EqualTo(listResults[i]));
                Assert.That(getResults[i - 1], Is.EqualTo(getResults[i]));
            });
        }
    }

    [Test]
    public async Task ServerList_WithCorrectRequests_ShouldReturnExpectedResult()
    {
        var request = "1 ../../../TestFiles";
        var expected = "4 ../../../TestFiles/AlgebraPractice.pdf False ../../../TestFiles/empty.txt False ../../../TestFiles/lol.txt False ../../../TestFiles/sos True\n";
        using var newClient = new TcpClient("127.0.0.2", 8888);
        using var stream = newClient.GetStream();
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        using var reader = new StreamReader(stream);


        await writer.WriteLineAsync(request);
        var response = await reader.ReadToEndAsync();


        Assert.That(response, Is.EqualTo(expected));
    }

    [Test]
    public async Task ServerGet_WithCorrectRequests_ShouldReturnExpectedResult()
    {
        var request = "2 ../../../TestFiles/lol.txt";
        var expected = "10 Sasha loh\n";
        using var newClient = new TcpClient("127.0.0.2", 8888);
        using var stream = newClient.GetStream();
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        using var reader = new StreamReader(stream);


        await writer.WriteLineAsync(request);
        var response = await reader.ReadToEndAsync();


        Assert.That(response, Is.EqualTo(expected));
    }
}