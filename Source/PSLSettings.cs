using System.Collections.Generic;

namespace ProceduralSolidsLibrary
{
	public class PSLSettings
	{

		public static void ModuleManagerPostLoad()
		{
			LoadPropellants();
			LoadCasingMaterials();
		}

		public static readonly Dictionary<string, PropellantConfig> propellantConfigs = new Dictionary<string, PropellantConfig>();
		public static void LoadPropellants()
		{
			propellantConfigs.Clear();
			foreach (ConfigNode propellantNode in GameDatabase.Instance.GetConfigNodes(PropellantConfig.nodeName))
			{
				PropellantConfig conf = ConfigNode.CreateObjectFromConfig<PropellantConfig>(propellantNode);
				propellantConfigs.Add(conf.name, conf);
			}
		}

		public static readonly Dictionary<string, CasingMaterialConfig> casingMaterialConfigs = new Dictionary<string, CasingMaterialConfig>();
		public static void LoadCasingMaterials()
		{
			casingMaterialConfigs.Clear();
			foreach (ConfigNode propellantNode in GameDatabase.Instance.GetConfigNodes(CasingMaterialConfig.nodeName))
			{
				CasingMaterialConfig conf = ConfigNode.CreateObjectFromConfig<CasingMaterialConfig>(propellantNode);
				casingMaterialConfigs.Add(conf.name, conf);
			}
		}
	}
}
