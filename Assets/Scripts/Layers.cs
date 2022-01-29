namespace DefaultNamespace
{
    public class Layers
    {
        public static int ATOMS = 6;
        public static int PHOTONS = 7;
    }

    public class Masks
    {
        public static int ATOMS = 1 << Layers.ATOMS;
        public static int PHOTONS = 1 << Layers.PHOTONS;
    }
}