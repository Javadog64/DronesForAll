using System;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using RWCustom;
using BepInEx;
using Debug = UnityEngine.Debug;
using On;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace DronesForAll;

[BepInPlugin("javadog.dronesforall", "Javadog", "1.1.3")]
public partial class DronesForAll : BaseUnityPlugin
{
    public static OptionInterface LoadOI() => new DroneOptions();

    private void OnEnable()
    {
        On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
    }

    private bool IsInit;
    private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        if (IsInit) return;

        try
        {
            IsInit = true;

            //Your hooks go here
            Hooks.Apply();

            MachineConnector.SetRegisteredOI("javadog.dronesforall", DroneOptions.instance);
            DroneOptions.help();
            Futile.atlasManager.LoadAtlas("atlases/drone");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

}
