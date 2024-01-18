public class AccountManager : IDisposable {
    private RabbitMQKom rabbit;
    private DBKom db;

    private object _registerHandlerFunc(object[] args) {

        string username = args[0].ToString();
        string password = args[1].ToString();
        return Register(username,password);
       // return Register(username,password);
    }

    private object _login(object[] args) {
        
        string username = args[0].ToString();
        string password = args[1].ToString();
    
        return Login(username,password);
    }
    private object _logout(object[] args) {
         string token = args[0].ToString();
         if(token == null) {
            throw new Exception("Losa deserializacija");
         }
         db.Logout(token);
         return null;

    }
    private object Register(string username,string password) {
 
        if(username == null || password == null) {
            return "501 Losa deserializacija";
        }
        return db.Register(username,password);


    }

    private object Login(string username,string password) {
        if(username == null || password == null) {
            return "501 Losa deserializacija";
        }
        return db.Login(username,password);


    }
    public AccountManager(RabbitMQKom r, DBKom d) {
        rabbit = r;
        db = d;
    }

    public void Dispose()
    {
        rabbit.Dispose();
        db.Dispose();
    }

    public Task ManageAccounts() {
        return Task.Run(() => {
            bool a = rabbit.RegisterRpc("Register",_registerHandlerFunc);
            bool b = rabbit.RegisterRpc("Login",_login);
            bool c = rabbit.RegisterRpc("logout",_logout);

        });
    }
}