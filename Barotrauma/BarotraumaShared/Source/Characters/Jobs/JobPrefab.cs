﻿using System.Collections.Generic;
using System.Xml.Linq;

namespace Barotrauma
{
    partial class JobPrefab
    {
        public static List<JobPrefab> List;
                
        public readonly XElement Items;
        public readonly List<string> ItemNames;

        public List<SkillPrefab> Skills;
        
        //the number of these characters in the crew the player starts with
        public readonly int InitialCount;

        public string Identifier
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        //if set to true, a client that has chosen this as their preferred job will get it no matter what
        public bool AllowAlways
        {
            get;
            private set;
        }

        //how many crew members can have the job (only one captain etc)    
        public int MaxNumber
        {
            get;
            private set;
        }

        //how many crew members are REQUIRED to have the job 
        //(i.e. if one captain is required, one captain is chosen even if all the players have set captain to lowest preference)
        public int MinNumber
        {
            get;
            private set;
        }

        public float MinKarma
        {
            get;
            private set;
        }

        public float Commonness
        {
            get;
            private set;
        }

        //how much the vitality of the character is increased/reduced from the default value
        public float VitalityModifier
        {
            get;
            private set;
        }

        public JobPrefab(XElement element)
        {
            Identifier = element.GetAttributeString("identifier", "notfound");

            string translatedName = TextManager.Get("JobName."+Identifier, true);
            Name = string.IsNullOrEmpty(translatedName) ? element.GetAttributeString("name", "name not found") : translatedName;
            
            string translatedDescription = TextManager.Get("JobDescription." + Identifier, true);
            Description = string.IsNullOrEmpty(translatedDescription) ? element.GetAttributeString("description", "") : translatedDescription;

            VitalityModifier = element.GetAttributeFloat("vitalitymodifier", 0.0f);

            MinNumber = element.GetAttributeInt("minnumber", 0);
            MaxNumber = element.GetAttributeInt("maxnumber", 10);
            MinKarma = element.GetAttributeFloat("minkarma", 0.0f);

            InitialCount = element.GetAttributeInt("initialcount", 0);

            Commonness = element.GetAttributeInt("commonness", 10);

            AllowAlways = element.GetAttributeBool("allowalways", false);

            ItemNames = new List<string>();

            Skills = new List<SkillPrefab>();

            foreach (XElement subElement in element.Elements())
            {
                switch (subElement.Name.ToString().ToLowerInvariant())
                {
                    case "items":
                        Items = subElement;
                        foreach (XElement itemElement in subElement.Elements())
                        {
                            string itemName = itemElement.GetAttributeString("name", "");
                            if (!string.IsNullOrWhiteSpace(itemName)) ItemNames.Add(itemName);
                        }
                        break;
                    case "skills":
                        foreach (XElement skillElement in subElement.Elements())
                        {
                            Skills.Add(new SkillPrefab(skillElement));
                        } 
                        break;
                }
            }

            Skills.Sort((x,y) => y.LevelRange.X.CompareTo(x.LevelRange.X));
        }

        public static JobPrefab Random()
        {
            return List[Rand.Int(List.Count)];
        }

        public static void LoadAll(IEnumerable<string> filePaths)
        {
            List = new List<JobPrefab>();

            foreach (string filePath in filePaths)
            {
                XDocument doc = XMLExtensions.TryLoadXml(filePath);
                if (doc == null || doc.Root == null) return;

                foreach (XElement element in doc.Root.Elements())
                {
                    JobPrefab job = new JobPrefab(element);
                    List.Add(job);
                }
            }
        }
    }
}
