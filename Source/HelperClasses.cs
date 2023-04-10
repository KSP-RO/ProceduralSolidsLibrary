using UnityEngine;

namespace ProceduralSolidsLibrary
{
	public class PropellantConfig : IConfigNode
	{
		public const string nodeName = "SRBLIB_PROPELLANT_DEFINITION";
		private const float R = 8.31446261815324f; // J / K mol

		[Persistent] public string name;
		[Persistent] public float burnRateCoeff;
		[Persistent] public float burnRateExponent;
		[Persistent] public float heatCapacityRatio;
		[Persistent] public float density;
		[Persistent] public float characVel;

		// public float CharacVel => Mathf.Sqrt(heatCapacityRatio * R * combustionTemp / molarMass) / (heatCapacityRatio * Mathf.Sqrt(Mathf.Pow(2 / (heatCapacityRatio + 1), (heatCapacityRatio + 1)/(heatCapacityRatio - 1))));

		/// <summary>
		/// <para><paramref name="molarMass"></paramref> is in kg / mol</para>
		/// <para><paramref name="density"></paramref> is in kg / m^3</para>
		/// </summary>
		public PropellantConfig(float burnRateCoeff, float burnRateExponent, float heatCapacityRatio, float density, float characVel)//, float combustionTemp, float heatCapacityRatio, float molarMass)
		{
			this.burnRateCoeff = burnRateCoeff;
			this.burnRateExponent = burnRateExponent;
			this.heatCapacityRatio = heatCapacityRatio;
			this.density = density;
			this.characVel = characVel;
			// this.combustionTemp = combustionTemp;
			// this.molarMass = molarMass;
		}
		public PropellantConfig() {}
		public PropellantConfig(ConfigNode node)
		{
			Load(node);
		}

		public void Load(ConfigNode node)
		{
			if (! (node.name.Equals(nodeName) && node.HasValue("name")))
				return;

			ConfigNode.LoadObjectFromConfig(this, node);
		}

		public void Save(ConfigNode node)
		{
			if (name == null) return;
			ConfigNode.CreateConfigFromObject(this, node);
		}
	}

	public class GrainGeometryConfig : IConfigNode
	{
		public const string nodeName = "SRBLIB_GRAINGEOMETRY_DEFINITION";
		[Persistent] public string name;
		[Persistent] public FloatCurve thrustCurve;
		[Persistent] public float burnAreaScale;
		[Persistent] public float propellantFraction;

		public GrainGeometryConfig() {}
		public GrainGeometryConfig(ConfigNode node)
		{
			Load(node);
		}

		public void Load(ConfigNode node)
		{
			if (! (node.name.Equals(nodeName) && node.HasValue("name")))
				return;

			ConfigNode.LoadObjectFromConfig(this, node);
		}

		public void Save(ConfigNode node)
		{
			if (name == null) return;
			ConfigNode.CreateConfigFromObject(this, node);
		}
	}

	public class CasingMaterialConfig : IConfigNode
	{
		public const string nodeName = "SRBLIB_CASINGMATERIAL_DEFINITION";

		[Persistent] public string name;
		[Persistent] public float density;
		[Persistent] public float tensileStrength;
		[Persistent] public float corrosionSafety = 0f;
		[Persistent] public float weldEff = 1f;

		public CasingMaterialConfig(float density, float tensileStrength, float corrosionSafety = 0f, float weldEff = 1f)
		{
			this.density = density;
			this.tensileStrength = tensileStrength;
			this.corrosionSafety = corrosionSafety;
			this.weldEff = weldEff;
		}
		public CasingMaterialConfig() {}
		public CasingMaterialConfig(ConfigNode node)
		{
			Load(node);
		}

		public void Load(ConfigNode node)
		{
			if (! (node.name.Equals(nodeName) && node.HasValue("name")))
				return;

			ConfigNode.LoadObjectFromConfig(this, node);
		}

		public void Save(ConfigNode node)
		{
			if (name == null) return;
			ConfigNode.CreateConfigFromObject(this, node);
		}
	}

	public class NozzleConfig : IConfigNode
	{
		public const string nodeName = "SRBLIB_NOZZLE_DEFINITION";

		[Persistent] public string name;
		[Persistent] public float nozzleCoeff; // Is this needed with atmo curve?
		[Persistent] public float designPressurePa;
		[Persistent] public FloatCurve atmosphereCurve; // Currently normalized to 1 and nozzleCoeff is used to scale
		[Persistent] public float gimbalRange = 0f;

		public NozzleConfig() { nozzleCoeff = 1f; }
		public NozzleConfig(float nozzleCoeff)
		{
			this.nozzleCoeff = nozzleCoeff;
		}
		public NozzleConfig(ConfigNode node)
		{
			Load(node);
		}

		public void Load(ConfigNode node)
		{
			if (! (node.name.Equals(nodeName) && node.HasValue("name")))
				return;

			ConfigNode.LoadObjectFromConfig(this, node);
		}

		public void Save(ConfigNode node)
		{
			if (name == null) return;
			ConfigNode.CreateConfigFromObject(this, node);
		}
	}
}
