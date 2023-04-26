namespace OptimazeScroll
{
    public class InitObjectTest : IScrollInitializeObject
    {
        public readonly int itemNumber;
        public InitObjectTest(int itemNumber)
        {
            this.itemNumber = itemNumber;
        }
    }
}