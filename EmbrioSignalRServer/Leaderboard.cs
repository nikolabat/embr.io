
public static class Leaderboard {


      private static Dictionary<string,int> leaderboard;
      private  static object objlock = new object();
      public static void Set(Dictionary<string,int> lb) {
        lock(objlock) {
        leaderboard = lb;
        }

      }
      public static Dictionary<string,int> Get() {
      
        return  leaderboard;
        

    
    }

   
}