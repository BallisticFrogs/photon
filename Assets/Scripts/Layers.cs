namespace DefaultNamespace
{
    public class Layers
    {
        public static int ATOMS = 6;
        public static int PHOTONS = 7;
        public static int WAVES = 8;
        public static int OBSTACLES = 9;
    }

    public class Masks
    {
        public static int ATOMS = 1 << Layers.ATOMS;
        public static int PHOTONS = 1 << Layers.PHOTONS;
        public static int WAVES = 1 << Layers.WAVES;
        public static int OBSTACLES = 1 << Layers.OBSTACLES;
    }
}