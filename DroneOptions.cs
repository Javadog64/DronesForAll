using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using System.Collections.Generic;
using System.Linq;


//using System.Drawing;
using UnityEngine;

namespace DronesForAll
{
    internal class DroneOptions : OptionInterface
    {

        //have pebbles kill drone users?
        //open metrogates?
        public static List<Configurable<bool>> usingDrone = new List<Configurable<bool>>();
        public static List<Configurable<bool>> kingDroneVars = new List<Configurable<bool>>();
        public static List<Configurable<Color>> eyeColors = new List<Configurable<Color>>();
        public static List<Configurable<Color>> antColors = new List<Configurable<Color>>();
        public static List<Configurable<Color>> bottomColors = new List<Configurable<Color>>();
        public static List<Configurable<Color>> bodyColors = new List<Configurable<Color>>();
        


        public static void help()
        {
            for (int i = 0; i < 12; i++)
            {
                kingDroneVars.Add(instance.config.Bind(i.ToString() + "_king", false, new ConfigurableInfo("Use the Cheiftain Scavenger's drone design?")));
                usingDrone.Add(instance.config.Bind(i.ToString() + "_droneUse", true, new ConfigurableInfo("Use drone?")));
                eyeColors.Add(instance.config.Bind(i.ToString() + "_color", new Color(1f, 0f, 0f), new ConfigurableInfo("What color to use as the eye and glow color")));
                antColors.Add(instance.config.Bind(i.ToString() + "_ant", new Color(1f, 0.6549f, 0.2863f), new ConfigurableInfo("What color to use for the edges of the antenna")));
                bottomColors.Add(instance.config.Bind(i.ToString() + "_bottom", new Color(0.945f, 0.3765f, 0f), new ConfigurableInfo("What color to use for the bottom of the drone")));
                bodyColors.Add(instance.config.Bind(i.ToString() + "_body", new Color(1f, 0.8902f, 0.451f), new ConfigurableInfo("What color to use for the main body and ends of the antenna")));
            }
        }


        public override void Initialize()
        {
            base.Initialize();
            if (ModManager.Watcher)
            {
                Tabs = new OpTab[12];
            }
            else
            {
                Tabs = new OpTab[11];
            }
            
            Tabs[0] = new OpTab(this, "Options"); //slugcats beside the main 10 or could be used for options like have pebbles kill the player with a drone?
            Tabs[1] = new OpTab(this, "Monk") { colorButton = new Color(1f, 1f, 0.4509804f) };
            Tabs[2] = new OpTab(this, "Survivor") { colorButton = new Color(1f, 1f, 1f) };
            Tabs[3] = new OpTab(this, "Hunter") { colorButton = new Color(1f, 0.4509804f, 0.4509804f) };
            Tabs[4] = new OpTab(this, "Gourmand") { colorButton = new Color(0.94118f, 0.75686f, 0.59216f) };
            Tabs[5] = new OpTab(this, "Artificer") { colorButton = new Color(0.43922f, 0.13725f, 0.23529f) };
            Tabs[6] = new OpTab(this, "Rivulet") { colorButton = new Color(0.56863f, 0.8f, 0.94118f) };
            Tabs[7] = new OpTab(this, "Spearmaster") { colorButton = new Color(0.31f, 0.18f, 0.41f) };
            Tabs[8] = new OpTab(this, "Saint") { colorButton = new Color(0.66667f, 0.9451f, 0.33725f) };
            Tabs[9] = new OpTab(this, "Inv") { colorButton = new Color(0.09f, 0.14f, 0.31f) };
            if (ModManager.Watcher)
            {
                Tabs[10] = new OpTab(this, "Watcher") { colorButton = new Color(0.109803922f, 0.0705882353f, 0.529411765f) };
                Tabs[11] = new OpTab(this, " Other") { colorButton = new Color(1f, 1f, 1f) };
            }
            else
            {
                Tabs[10] = new OpTab(this, " Other") { colorButton = new Color(1f, 1f, 1f) };
            }
            

            Tabs[0].AddItems(new UIelement[]
            {
                new OpCheckBox(noPebblesKill, new UnityEngine.Vector2(0f, 500f)) { description = noPebblesKill.info.description},
                new OpLabel(30f, 503, "Stop Pebbles from killing Slugcats with a Drone?") { description =  noPebblesKill.info.description},

                new OpCheckBox(openMetro, new UnityEngine.Vector2(0f, 400f)) { description = openMetro.info.description},
                new OpLabel(30f, 403, "Open the gate to Metropolis for Slugcats with a drone?") { description =  openMetro.info.description},

                new OpCheckBox(openBitter, new UnityEngine.Vector2(0f, 300f)) { description = "Open the gate to Bitter Aerie for Slugcats with a drone?"},
                new OpLabel(30f, 303f, "Open the gate to Bitter Aerie for Slugcats with a drone?") { description =  "Open the gate to Bitter Aerie for Slugcats with a drone?"},
                new OpLabel(30f, 273f, "WARNING MAY CAUSE PROBLEMS WITH SLUGCATS BEFORE HUNTER") { description =  "WARNING MAY CAUSE PROBLEMS WITH SLUGCATS BEFORE HUNTER", color = Color.red},
            });


            for (int i = 1; i < Tabs.Length; i++)
            {
                Tabs[i].AddItems(new UIelement[]
            {
                new OpCheckBox(kingDroneVars[i], new UnityEngine.Vector2(306f, 522f)) { description = kingDroneVars[i].info.description},
                new OpLabel(336f, 522f, "Use Cheiftain Drone?") { description =  kingDroneVars[i].info.description},

                new OpCheckBox(usingDrone[i], new UnityEngine.Vector2(306f, 482f)) { description = "Use Drone for " + ((i == 11) ? "modded slugcats" : Tabs[i].name) + "?"},
                new OpLabel(336f, 482f, "Use Drone for " + ((i == 11) ? "modded slugcats" : Tabs[i].name) + "?") { description =  "Use Drone for " + ((i == 11) ? "modded slugcats" : Tabs[i].name) + "?"},

                new OpColorPicker(eyeColors[i], new UnityEngine.Vector2(76f, 373)) {description = eyeColors[i].info.description},
                new OpLabel(76f, 529f, "Eye color and glow") {description = eyeColors[i].info.description},

                new OpColorPicker(antColors[i], new UnityEngine.Vector2(76f, 133)) {description = antColors[i].info.description},
                new OpLabel(77f, 287f, "Antenna bottom color") {description = antColors[i].info.description},

                new OpColorPicker(bottomColors[i], new UnityEngine.Vector2(252, 133)) {description = bottomColors[i].info.description},
                new OpLabel(252f, 287f, "Bottom color") {description = bottomColors[i].info.description},

                new OpColorPicker(bodyColors[i], new UnityEngine.Vector2(424, 133)) {description = bodyColors[i].info.description},
                new OpLabel(424f, 287f, "Main body and antenna color") {description = bodyColors[i].info.description},

                new OpImage(new Vector2(306f,408f), "eye") {color = eyeColors[i].Value, scale = new Vector2(2f,2f)},
                new OpImage(new Vector2(306f,408f), "edge") {color = antColors[i].Value, scale = new Vector2(2f,2f)},
                new OpImage(new Vector2(306f,408f), "bottom") {color = bottomColors[i].Value, scale = new Vector2(2f,2f)},
                new OpImage(new Vector2(306f,408f), "body") {color = bodyColors[i].Value, scale = new Vector2(2f,2f)},
                new OpImage(new Vector2(306f,408f), "kingeye") {color = eyeColors[i].Value, scale = new Vector2(2f,2f)},
                new OpImage(new Vector2(306f,408f), "kingedge") {color = antColors[i].Value, scale = new Vector2(2f,2f)},
                new OpImage(new Vector2(306f,408f), "kingbottom") {color = bottomColors[i].Value, scale = new Vector2(2f,2f)},
                new OpImage(new Vector2(306f,408f), "kingbody") {color = bodyColors[i].Value, scale = new Vector2(2f,2f)},
            });
            }
            
            
        }

        public override void Update()
        {
            foreach (var tab in Tabs)
            {
                var colorPickers = tab.items.OfType<OpColorPicker>().ToList();
                var kingUsing = tab.items.OfType<OpCheckBox>().Where(x => x.Key.EndsWith("_king")).ToList();
                var sprites = tab.items.OfType<OpImage>().ToList();

                for (int i = 0; i < colorPickers.Count; i++)
                {
                    sprites[i].color = colorPickers[i].valueColor;
                    sprites[i+4].color = colorPickers[i].valueColor;

                    if (kingUsing[0].GetValueBool())
                    {
                        sprites[i].Hide();
                        sprites[i+4].Show();

                    }
                    else
                    {
                        sprites[i].Show();
                        sprites[i + 4].Hide();
                    }
                    
                }

            }
            



        }


        public static DroneOptions instance = new DroneOptions();

        public static Configurable<bool> noPebblesKill = instance.config.Bind("stopPebblesKill", false, new ConfigurableInfo("Stop pebbles from killing slugcats with a drone?"));
        public static Configurable<bool> openMetro = instance.config.Bind("unlockMetro", false, new ConfigurableInfo("Open the gate to Metropolis for slugcats with a drone?"));
        public static Configurable<bool> openBitter = instance.config.Bind("unlockBitter", false, new ConfigurableInfo("Open the gate to Bitter Aerie for slugcats with a drone?<LINE>WARNING MAY CAUSE PROBLEMS WITH SLUGCASTS BEFORE HUNTER"));

        //public static Configurable<bool> isKingDrone = instance.config.Bind("isKingDrone", false, new ConfigurableInfo("Use the Cheiftain Scavenger's drone design?"));
        //public static Configurable<Color> red = instance.config.Bind("color", new Color(1f, 0f, 0f), new ConfigurableInfo("What color to use as the eye and glow color"));
        //public static Configurable<Color> antColor = instance.config.Bind("ant", new Color(1f, 0.6549f, 0.2863f), new ConfigurableInfo("What color to use for the edges of the antenna"));
        //public static Configurable<Color> bodyColor = instance.config.Bind("body", new Color(0.945f, 0.3765f, 0f), new ConfigurableInfo("What color to use for the bottom of the drone"));
        //public static Configurable<Color> color3 = instance.config.Bind("color3", new Color(1f, 0.8902f, 0.451f), new ConfigurableInfo("What color to use for the main body and ends of the antenna"));


        //TODO: add a use drone? option for all slugcats




    }
}
