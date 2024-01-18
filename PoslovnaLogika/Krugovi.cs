using System.Diagnostics.CodeAnalysis;


  public enum KrugTip {
      Igrac,Hrana,Zamka
     }
  public struct Point 
{
    public Point(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float X {get;set;}
   public float Y {get;set;}
}

public class Krug  {
    public Krug(long entityID, Point position, int r, uint boja, KrugTip tip,string label = "")
    {
        EntityID = entityID;
        Position = position;
        R = r;
        Boja = boja;
        this.tip = tip;
        Label = label;
    }

    public long EntityID {get;set;}
      public  Point Position {get;set;}
      
    public  int R {get;set;}

     public uint Boja {get;set;}

     public KrugTip tip {get;set;}

     public string Label {get;set;}
   
};


   


