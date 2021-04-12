using System;
using System.IO;
using System.Net;
using Domino;
using Geomancer.Model;
using GeomancerServer;
using SimpleJSON;

namespace Geomancer {
  public class HttpServer {
    public static void Main(string[] args) {
      SimpleListenerExample(new string[] {"http://127.0.0.1:8080/"});
    }
    
    // This example requires the System and System.Net namespaces.
    public static void SimpleListenerExample(string[] prefixes) {
      if (!HttpListener.IsSupported) {
        Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
        return;
      }
      // URI prefixes are required,
      // for example "http://contoso.com:8080/index/".
      if (prefixes == null || prefixes.Length == 0)
        throw new ArgumentException("prefixes");

      // Create a listener.
      HttpListener listener = new HttpListener();
      // Add the prefixes.
      foreach (string s in prefixes) {
        listener.Prefixes.Add(s);
      }
      listener.Start();

      Console.WriteLine("Listening...");

      var (editor, gameToDominoConnection) = HandleStartRequest(listener);
      
      while (HandleRequest(listener, editor, gameToDominoConnection)) { }
      
      listener.Stop();
    }

    private static void Respond(
        HttpListenerContext context,
        GameToDominoConnection gameToDominoConnection) {
      
      var responseMessages = gameToDominoConnection.TakeMessages();
      JSONObject responseObj = new JSONObject();
      JSONArray commandsArray = new JSONArray();
      responseObj.Add("commands", commandsArray);
      foreach (var message in responseMessages) {
        var json = message.ToJson();
        Console.WriteLine("Sending: " + json.ToString());
        commandsArray.Add(json);
      }
      
      // Obtain a response object.
      HttpListenerResponse response = context.Response;
      // Construct a response.
      string responseString = responseObj.ToString();
      byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
      // Get a response stream and write the response to it.
      response.ContentLength64 = buffer.Length;
      System.IO.Stream output = response.OutputStream;
      output.Write(buffer, 0, buffer.Length);
      // You must close the output stream.
      output.Close();
    }
    
    private static (EditorServer, GameToDominoConnection) HandleStartRequest(HttpListener listener) {
      // Note: The GetContext method blocks while waiting for a request.
      HttpListenerContext startRequestContext = listener.GetContext();
      HttpListenerRequest startRequest = startRequestContext.Request;
      var startRequestStr = new StreamReader(startRequest.InputStream).ReadToEnd();
      var startRequestNode = JSONObject.Parse(startRequestStr);
      var startRequestObj = JsonHarvester.ExpectObject(startRequestNode, "Request must be an object!");
      var startRequestType = JsonHarvester.ExpectMemberString(startRequestObj, "request");
      if (startRequestType != "start") {
        throw new Exception("First request must be start!");
      }
      var gameToDominoConnection = new GameToDominoConnection();
      var editor =
          new EditorServer(
              System.IO.Directory.GetCurrentDirectory(),
              gameToDominoConnection,
              JsonHarvester.ExpectMemberInteger(startRequestObj, "screenGW"),
              JsonHarvester.ExpectMemberInteger(startRequestObj, "screenGH"));
      Respond(startRequestContext, gameToDominoConnection);
      return (editor, gameToDominoConnection);
    }

    private static bool HandleRequest(
        HttpListener listener,
        EditorServer server,
        GameToDominoConnection gameToDominoConnection) {
      // Note: The GetContext method blocks while waiting for a request.
      HttpListenerContext requestContext = listener.GetContext();
      HttpListenerRequest request = requestContext.Request;
      var requestStr = new StreamReader(request.InputStream).ReadToEnd();
      var requestNode = JSONObject.Parse(requestStr);
      var requestObj = JsonHarvester.ExpectObject(requestNode, "Request must be an object!");
      var requestType = JsonHarvester.ExpectMemberString(requestObj, "request");
      bool keepRunning = true;
      switch (requestType) {
        case "setHoveredLocation":
          HandleSetHoveredLocation(server, requestObj);
          break;
        case "locationMouseDown":
          HandleLocationMouseDown(server, requestObj);
          break;
        case "keyDown":
          HandleKeyDown(server, requestObj);
          break;
        default:
          Asserts.Assert(false, "Unknown request: " + requestType);
          keepRunning = false;
          break;
      }
      Respond(requestContext, gameToDominoConnection);
      return keepRunning;
    }

    private static void HandleSetHoveredLocation(
        EditorServer server,
        JSONObject requestObj) {
      server.SetHoveredLocation(
          JsonHarvester.ExpectMemberULong(requestObj, "tileId"),
          JsonHarvester.GetMaybeMemberLocation(requestObj, "location", out var loc) ? loc : null);
    }

    private static void HandleLocationMouseDown(
        EditorServer server,
        JSONObject requestObj) {
      server.LocationMouseDown(
          JsonHarvester.ExpectMemberULong(requestObj, "tileId"),
          JsonHarvester.ExpectMemberLocation(requestObj, "location"));
    }
    
    private static void HandleKeyDown(
        EditorServer server,
        JSONObject requestObj) {
      server.KeyDown(
          JsonHarvester.ExpectMemberInteger(requestObj, "unicode"),
          JsonHarvester.ExpectMemberBoolean(requestObj, "leftShiftDown"),
          JsonHarvester.ExpectMemberBoolean(requestObj, "rightShiftDown"),
          JsonHarvester.ExpectMemberBoolean(requestObj, "ctrlDown"),
          JsonHarvester.ExpectMemberBoolean(requestObj, "leftAltDown"),
          JsonHarvester.ExpectMemberBoolean(requestObj, "rightAltDown"));
    }
  }
}
