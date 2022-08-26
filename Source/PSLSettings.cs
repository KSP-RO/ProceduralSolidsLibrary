using System.Collections.Generic;

namespace ProceduralSolidsLibrary
{
	public class PSLSettings
	{

		public static void ModuleManagerPostLoad()
		{
			LoadPropellants();
			LoadGrainGeometries();
			LoadCasingMaterials();
			LoadNozzles();
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

		public static readonly Dictionary<string, GrainGeometryConfig> grainGeometryConfigs = new Dictionary<string, GrainGeometryConfig>();
		public static void LoadGrainGeometries()
		{
			grainGeometryConfigs.Clear();
			foreach (ConfigNode grainGeometryNode in GameDatabase.Instance.GetConfigNodes(GrainGeometryConfig.nodeName))
			{
				GrainGeometryConfig conf = ConfigNode.CreateObjectFromConfig<GrainGeometryConfig>(grainGeometryNode);
				grainGeometryConfigs.Add(conf.name, conf);
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

		public static readonly Dictionary<string, NozzleConfig> nozzleConfigs = new Dictionary<string, NozzleConfig>();
		public static void LoadNozzles()
		{
			nozzleConfigs.Clear();
			foreach (ConfigNode nozzleNode in GameDatabase.Instance.GetConfigNodes(NozzleConfig.nodeName))
			{
				NozzleConfig conf = ConfigNode.CreateObjectFromConfig<NozzleConfig>(nozzleNode);
				nozzleConfigs.Add(conf.name, conf);
			}
		}
	}
}
