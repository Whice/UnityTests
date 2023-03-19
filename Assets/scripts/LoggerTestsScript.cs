using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LoggerTestsScript : MonoBehaviour
{
    [Inject]
    private Logger logger;
    void Update()
    {
        logger.Message("Hello!!!");
    }
}
