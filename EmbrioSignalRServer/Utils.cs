using System.Net;

public static class Utils {
    public static double tickrate;
    public static uint napraviBoju(byte r,byte g, byte b, byte a = 1) {
       return (uint)((r << 24) + (g << 16) + (b << 8) + (a));
    }
    public static bool SeceSe(Krug a,Krug b) {
        float r0 = a.R / 2f;
        float r1 = b.R / 2f;
        float x0 = a.Position.X;
        float y0 = a.Position.Y;
        float x1 = b.Position.X;
        float y1 = b.Position.Y;

        float rrk = (r0 - r1)*(r0 - r1);
        float xrk = (x0 - x1)*(x0 - x1);
        float yrk = (y0 - y1)*(y0 - y1);
        float d = xrk + yrk;

        float rzk = (r0+r1)*(r0+r1);


        return rrk <= d && d <= rzk;
    }

}

