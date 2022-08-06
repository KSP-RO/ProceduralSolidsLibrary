using UnityEngine;

namespace ProceduralSolidsLibrary
{
	public class PropellantConfig : IConfigNode
	{
		public const string nodeName = "SRBLIB_PROPELLANT_DEFINITION";
		private const float R = 8.31446261815324f; // J / K mol

		[Persistent]
		public string name;
		[Persistent]
		public float burnRateCoeff;
		[Persistent]
		public float burnRateExponent;
		[Persistent]
		public float density;
		[Persistent]
		public float combustionTemp;
		[Persistent]
		public float heatCapacityRatio;
		[Persistent]
		public float molarMass;
		public float characVel => Mathf.Sqrt(heatCapacityRatio * R * molarMass * combustionTemp) / (heatCapacityRatio * Mathf.Sqrt(Mathf.Pow(2 / (heatCapacityRatio + 1), (heatCapacityRatio + 1)/(heatCapacityRatio - 1))));

		/// <summary>
		/// <para><paramref name="molarMass"></paramref> is in g / mol</para>
		/// <para><paramref name="density"></paramref> is in kg / m^3</para>
		/// </summary>
		public PropellantConfig(float burnRateCoeff, float burnRateExponent, float density, float combustionTemp, float heatCapacityRatio, float molarMass)
		{
			this.burnRateCoeff = burnRateCoeff / 1_000_000f; // TODO: Why is 1M needed here?
			this.burnRateExponent = burnRateExponent;
			this.density = density;
			this.combustionTemp = combustionTemp;
			this.heatCapacityRatio = heatCapacityRatio;
			this.molarMass = molarMass;
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

	public class CasingMaterialConfig : IConfigNode
	{
		public const string nodeName = "SRBLIB_CASINGMATERIAL_DEFINITION";

		[Persistent]
		public string name;
		[Persistent]
		public float density;
		[Persistent]
		public float mats;
		[Persistent]
		public float corrosionSafety = 0f;
		[Persistent]
		public float weldEff = 1f;

		public CasingMaterialConfig(float density, float mats, float corrosionSafety = 0f, float weldEff = 1f)
		{
			this.density = density;
			this.mats = mats;
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

	public class Casing
	{
		public CasingMaterialConfig material;
		public float cylinderLength;
		public float diameter;
		public float mawp;

		public Casing() {}
		public Casing(CasingMaterialConfig material, float cylinderLength, float diameter)
		{
			this.material = material;
			this.cylinderLength = cylinderLength;
			this.diameter = diameter;
		}

		// FIXME: Currently clamping thickness here.
		public float thickness => Mathf.Min(diameter / 2f, (mawp * (diameter - material.corrosionSafety) + 2f * material.mats * material.weldEff * material.corrosionSafety) / (2f * material.mats * material.weldEff + mawp));

		public float innerVolume => PillVolume(cylinderLength - diameter, diameter - 2 * thickness);
		private float volume => PillVolume(cylinderLength - diameter, diameter) - innerVolume;
		public float mass => material.density * volume;

		private static float PillVolume(float cylinderLength, float diameter)
		{
			float sphereVol = diameter * diameter * diameter * Mathf.PI / 6f;
			float cylinderVol = diameter * diameter * cylinderLength * Mathf.PI / 4;
			return sphereVol + cylinderVol;
		}
	}

	public class Nozzle
	{
		public float nozzleCoeff;

		public Nozzle(float nozzleCoeff)
		{
			this.nozzleCoeff = nozzleCoeff;
		}
	}
}
