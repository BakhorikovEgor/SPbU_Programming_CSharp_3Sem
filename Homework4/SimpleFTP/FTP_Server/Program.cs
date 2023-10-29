using System.Net;
using SimpleFTP.FtpServer;

var endPoint = new IPEndPoint(IPAddress.Loopback, 8888);
var server = new FtpServer(endPoint);
await server.StartAsync();

