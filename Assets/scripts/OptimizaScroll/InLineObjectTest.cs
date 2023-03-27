using TMPro;
using UnityEngine;

namespace OptimazeScroll
{
    public class InLineObjectTest : MonoBehaviour, IScrollInitializable
    {
        [SerializeField] private TextMeshProUGUI itemNumber = null;
        public IScrollInitializable Clone(Transform parent)
        {
            return Instantiate(this, parent);
        }

        public RectTransform GetRectTransform()
        {
            return GetComponent<RectTransform>();
        }

        public void InScrollInitialize(IScrollInitializeObject objectForInitialize)
        {
            if (objectForInitialize is InitObjectTest initObject)
                itemNumber.text = initObject.itemNumber.ToString();
            else
                itemNumber.text = "-1";
        }
    }
}