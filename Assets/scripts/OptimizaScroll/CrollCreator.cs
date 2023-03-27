using UnityEngine;

namespace OptimazeScroll
{
    public class CrollCreator : MonoBehaviour
    {
        [SerializeField] private InLineObjectTest template = null;
        [SerializeField] private OptimaizedScroll scroll = null;
        public int itemsCount = 190;
        private InitObjectTest[] items;

        private void Awake()
        {
            items = new InitObjectTest[itemsCount];
            for(int i=0;i<itemsCount; i++)
            {
                items[i] = new InitObjectTest(i + 1);
            }
            scroll.Initialize(template, items);
        }
    }
}