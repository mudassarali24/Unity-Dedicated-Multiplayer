namespace GameServer.Utils
{
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            float deltaX = b.x - a.x;
            float deltaY = b.y - a.y;
            float deltaZ = b.z - a.z;
            float dist = (float)Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ));
            return dist;
        }
    }

    public struct ShootPacket
    {
        public Vector3 shootPoint;
        public Vector3 hitPoint;
        public int targetId;

        public ShootPacket(Vector3 shootPt, Vector3 hitPt, int targetId = -1)
        {
            shootPoint = shootPt;
            hitPoint = hitPt;
            this.targetId = targetId;
        }
    }

    public struct Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
}