public static class IgraSingleton {
     private static Igra _igra;
     private static object lockobj = new object();
    public static void Set(Igra i) {
        lock(lockobj) {
    _igra = i;
        }
    }
    public static Igra Get() {
        lock(lockobj) {
            return _igra;
        }

    }
}