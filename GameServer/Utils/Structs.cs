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
}