using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;




public class RabbitMQKom : IDisposable {

   // public delegate void ConsumerCallback(Dictionary<string,object> args);
    public delegate object RPCFunc(object[] args);
   

    
    private ConnectionFactory factory;
    private IConnection connection;

    private IModel channel;
    
    public RabbitMQKom(string ip) {
        factory = new ConnectionFactory {HostName = ip};
        connection = this.factory.CreateConnection();
        channel = connection.CreateModel();

        
    }

    public Task<Dictionary<string,object>> CreateAndListen(string queueName,Action<Dictionary<string,object>> action = null,int maxlen = 1024,bool autoAck = true) {
        var queue = channel.QueueDeclare(queueName,false,false,true,new Dictionary<string,object> {{"max-length",maxlen}});
        var consumer = new EventingBasicConsumer(channel);
        var  tsk = new TaskCompletionSource<Dictionary<string,object>>();
        
        consumer.Received += (model,ea) => {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
             var args = JsonSerializer.Deserialize<Dictionary<string,object>>(json);
             if(args == null) {
                tsk.TrySetException(new Exception("Greska u deserializaciji"));
                return;
             }
             tsk.TrySetResult(args);
             if(action == null) {
                action = delegate {};
                return;
             }
             action(args);


        };
        channel.BasicConsume(queueName,autoAck,consumer);
        return tsk.Task;
        

    }
    public bool CreateAndSendTo(string queueName, object message,int maxlen = 1024) {
          var queue = channel.QueueDeclare(queueName,false,false,true,new Dictionary<string,object> {{"max-length",maxlen}});
            
           
             var buf = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
               channel.BasicPublish(exchange: string.Empty,
                     routingKey: queueName,
                     basicProperties: null,
                     body: buf);
        
            
        return true;
    }
public object RpcCall(string rpcname,params object[] args) {
    string replyTo = RandomNumberGenerator.GetInt32(int.MaxValue).ToString();
    var rpcmsg = new Dictionary<string,object> {{"replyTo", replyTo},{"rpcname",rpcname},{"args",args}};
    bool ok = CreateAndSendTo(rpcname,rpcmsg);
    if(!ok) {
        throw new Exception("RPC greska");
    }

    
   var rez = CreateAndListen(replyTo.ToString()).Result;
   return rez["result"];
    
    

}

public bool RegisterRpc(string rpcname, RPCFunc func) {
    CreateAndListen(rpcname,(Dictionary<string, object> args) => {
        
        string? replyTo = args["replyTo"].ToString();
        if(replyTo == null) {
            throw new Exception("Greska u deserializaciji prilikom obrade RPC-a");
        }
        //Console.WriteLine(args["args"].GetType());
        var recievedargs = (JsonElement) args["args"];
        var parsedargs = JsonSerializer.Deserialize<object[]>(recievedargs);
        if(parsedargs == null) {
            throw new Exception("Greska u deserializaciji prilikom obrade RPC-a");
        }

        var rez = func(parsedargs);
        //Console.WriteLine(rez);
        CreateAndSendTo(replyTo,new Dictionary<string,object>{{"result",rez}},1);

        
    });
    return true;


}
    public void Dispose()
    {
        channel.Dispose();
        connection.Dispose();
    }
}