namespace SingalRTest.service
{
    public class CountService
    {
        private int _count = 0;
        public int GetLatestCount()
        {
            return _count++;
        }
    }
}
