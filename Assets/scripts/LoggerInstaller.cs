using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LoggerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<Logger>().FromNew().AsSingle().NonLazy();
    }
}

public class Logger
{
    public void Message(object message)
    {
        Debug.Log(message);
    }
}