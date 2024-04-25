using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "ScriptableObjectInstaller/ScriptableObjectInstaller")]
public class ScriptableObjectInstaller : ScriptableObjectInstaller<ScriptableObjectInstaller>
{
    public List<ScriptableObject> ScriptableObjects;

    public override void InstallBindings()
    {
        foreach (ScriptableObject so in ScriptableObjects)
        {
            Container.BindInterfacesAndSelfTo(so.GetType()).FromInstance(so);
        }
    }
}