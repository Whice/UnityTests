

using UnityEngine;
using UnityEngine.UI;

public class Colorator : MonoBehaviour
{
    private void Awake()
    {
        int chance = Random.Range(0, 4);
        Image image = GetComponent<Image>();
        switch (chance)
        {
            case 0: image.color= Color.white; break;
            case 1: image.color = Color.red; break;
            case 2: image.color = Color.yellow; break;
            case 3: image.color = Color.blue; break;
        }
    }
}
