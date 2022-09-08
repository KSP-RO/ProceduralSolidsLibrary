using UnityEngine;

namespace ProceduralSolidsLibrary
{
	public class SolidsSolver
	{
		const float g0 = 9.81f;

		public PropellantConfig propellant;
		public GrainGeometryConfig grainGeometry;
		public Casing casing;
		public NozzleConfig nozzle;

		public SolidsSolver(PropellantConfig propellant, GrainGeometryConfig grainGeometry, CasingMaterialConfig casingMaterial, NozzleConfig nozzle, float length, float diameter, float combustionPressure)
		{
			this.propellant = propellant;
			this.grainGeometry = grainGeometry;
			this.nozzle = nozzle;
			casing = new Casing(casingMaterial, length, diameter);
			Length = length;
			Diameter = diameter;
			CombustionPressure = combustionPressure;
		}

		private float _length;
		public float Length
		{
			get => _length;
			set
			{
				_length = value;
				casing.pillLength = _length;
			}
		}

		private float _diameter;
		public float Diameter
		{
			get => _diameter;
			set
			{
				_diameter = value;
				casing.diameter = _diameter;
			}
		}

		public float Thrust => g0 * Isp * MassFlow;

		public float ThroatArea => BurnArea * propellant.density * propellant.burnRateCoeff * propellant.characVel / Mathf.Pow(CombustionPressure, 1f - propellant.burnRateExponent);

		public float Isp => nozzle.nozzleCoeff * propellant.characVel / g0;
		public FloatCurve AtmosphereCurve => nozzle.atmosphereCurve;
		public float MassFlow => CombustionPressure * ThroatArea / propellant.characVel;

		private float _combustionPressure;
		public float CombustionPressure
		{
			get => _combustionPressure;
			set
			{
				_combustionPressure = value;
				casing.mawp = _combustionPressure;
			}
		}
		public float DryMass => casing.Mass;
		public float WetVolume => casing.InnerVolume * grainGeometry.propellantFraction;
		public FloatCurve ThrustCurve => grainGeometry.thrustCurve;
		public float BurnArea => Mathf.PI * Diameter * Length * grainGeometry.burnAreaScale;

		#region Helper Outputs
		public float FuelMass => WetVolume * propellant.density;
		public float Mass => DryMass + FuelMass;
		public float Twr => Thrust/g0/Mass;
		public float BurnTime => FuelMass/MassFlow;

		#endregion

		#region Limits
		// free:
		// diam, presets
		// has limits:
		// throatArea max
		// thickness min/max based on length for min
		// pressure based on thickness and throatArea
		// length limited by diameter as min and maxThickness

		float MaxThroatArea => Diameter * Diameter * Mathf.PI / 4f * 0.25f; //FIXME: just 1/4 of the bottom area currently
		float MinPressureFromArea => Mathf.Pow(BurnArea / MaxThroatArea * propellant.density * propellant.burnRateCoeff * propellant.characVel, 1f / (1f - propellant.burnRateExponent));
		float MaxPressure => casing.MaxPressure;
		float MinPressure => Mathf.Max(casing.MinPressure, MinPressureFromArea);
		float MaxLength => casing.MaxLength;
		float MinLength => casing.MinLength;
		float MaxDiameter => casing.MaxDiameter;
		#endregion
	}

	public class Casing
	{
		public CasingMaterialConfig material;
		public float pillLength;
		public float diameter;
		public float mawp;

		public Casing(CasingMaterialConfig material, float pillLength, float diameter)
		{
			this.material = material;
			this.pillLength = pillLength;
			this.diameter = diameter;
		}

		private float Thickness => ThicknessFromPressure(mawp);
		public float InnerVolume => PillVolume(pillLength - diameter, diameter - 2 * Thickness);
		private float Volume => PillVolume(pillLength - diameter, diameter) - InnerVolume;
		public float Mass => material.density * Volume;
		// Limits
		private float MaxThickness => diameter / 2f * material.maxThicknessFraction;
		public float MaxPressure => PressureFromThickness(MaxThickness);
		private float MinThickness => pillLength * 0.001f; // FIXME: based on diameter too? A material constant?
		public float MaxLength => MaxThickness / 0.001f; // FIXME: inverse of above
		public float MinLength => diameter; // Assumes domes at the end, so has to be at least diameter long
		public float MaxDiameter => MaxLength; // TODO: Needed?
		public float MinPressure => PressureFromThickness(MinThickness);

		private float PressureFromThickness(float thickness)
		{
			return material.tensileStrength * thickness * material.weldEff / (diameter / 2f * material.safetyFactor);
		}

		private float ThicknessFromPressure(float pressure)
		{
			return pressure * diameter / 2f * material.safetyFactor / (material.tensileStrength * material.weldEff);
		}

		private static float PillVolume(float cylinderLength, float diameter)
		{
			float sphereVol = diameter * diameter * diameter * Mathf.PI / 6f;
			float cylinderVol = diameter * diameter * cylinderLength * Mathf.PI / 4;
			return sphereVol + cylinderVol;
		}
	}
}
