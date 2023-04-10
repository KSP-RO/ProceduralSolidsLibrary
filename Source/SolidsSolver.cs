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
		public bool thrustIsInput = false;
		public bool throatAreaIsInput = false;
		public bool thrustPercentIsInput = false;
		public bool combustionPressureIsInput = false;

		public SolidsSolver() { casing = new Casing(); nozzle = new NozzleConfig(); }
		// public SolidsSolver(PropellantConfig propellant, CasingMaterialConfig casingMaterial, Nozzle nozzle, float length, float diameter)
		// {
		// 	this.propellant = propellant;
		// 	this.nozzle = nozzle;
		// 	casing = new Casing(casingMaterial, length - diameter, diameter);
		// 	Length = length;
		// 	Diameter = diameter;
		// }

		public static SolidsSolver ThrustAsInput(PropellantConfig initialPropellant, CasingMaterialConfig initialCasingMaterial, float initialThrust)
		{
			var solver = new SolidsSolver();
			solver.propellant = initialPropellant;
			solver.casing.material = initialCasingMaterial;
			solver.thrustIsInput = true;
			solver.Thrust = initialThrust;
			return solver;
		}

		public static SolidsSolver ThroatAreaAsInput(PropellantConfig initialPropellant, CasingMaterialConfig initialCasingMaterial, float initialThroatArea)
		{
			var solver = new SolidsSolver();
			solver.propellant = initialPropellant;
			solver.casing.material = initialCasingMaterial;
			solver.throatAreaIsInput = true;
			solver.ThroatArea = initialThroatArea;
			return solver;
		}

		public static SolidsSolver PressureAsInput(PropellantConfig initialPropellant, CasingMaterialConfig initialCasingMaterial, float initialPressure)
		{
			var solver = new SolidsSolver();
			solver.propellant = initialPropellant;
			solver.casing.material = initialCasingMaterial;
			solver.combustionPressureIsInput = true;
			solver.CombustionPressure = initialPressure;
			return solver;
		}

		private float _length;
		public float Length
		{
			get => _length;
			set
			{
				_length = value;
				casing.cylinderLength = _length - _diameter;
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

		private float _thrust;
		public float Thrust
		{
			get
			{
				return (thrustIsInput, throatAreaIsInput, combustionPressureIsInput) switch
				{
					(true, false, false) => _thrust,
					(false, true, false) => g0 * Isp * MassFlow,
					(false, false, true) => g0 * Isp * MassFlow,
					_ => throw new System.Exception("Invalid boolean combination in 'Thrust'")
				};
			}
			set
			{
				_thrust = value;
			}
		}

		private float _throatArea;
		public float ThroatArea
		{
			get
			{
				return (thrustIsInput, throatAreaIsInput, combustionPressureIsInput) switch
				{
					(true, false, false) => BurnArea / (Mathf.Pow(CombustionPressure, 1f - propellant.burnRateExponent) / (propellant.density * propellant.burnRateCoeff * propellant.characVel)),
					(false, true, false) => _throatArea,
					(false, false, true) => BurnArea / (Mathf.Pow(CombustionPressure, 1f - propellant.burnRateExponent) / (propellant.density * propellant.burnRateCoeff * propellant.characVel)),
					_ => throw new System.Exception("Invalid boolean combination in 'throatArea'")
				};
			}
			set
			{
				_throatArea = value;
			}
		}

		public float Isp => nozzle.nozzleCoeff * propellant.characVel / g0;
		public FloatCurve AtmosphereCurve => nozzle.atmosphereCurve;
		public float MassFlow =>
			(thrustIsInput, throatAreaIsInput, combustionPressureIsInput) switch
			{
				(true, false, false) => _thrust / (g0 * Isp),
				(false, true, false) => CombustionPressure * ThroatArea / propellant.characVel,
				(false, false, true) => CombustionPressure * ThroatArea / propellant.characVel,
				_ => throw new System.Exception("Invalid boolean combination in 'massFlow'")
			};

		private float _combustionPressure;
		public float CombustionPressure
		{
			get
			{
				_combustionPressure = (thrustIsInput, throatAreaIsInput, combustionPressureIsInput) switch
				{
					(true, false, false) => Mathf.Pow(MassFlow / (BurnArea * propellant.density * propellant.burnRateCoeff), 1f / propellant.burnRateExponent),
					(false, true, false) => Mathf.Pow(BurnArea / ThroatArea * propellant.density * propellant.burnRateCoeff * propellant.characVel, 1f / (1f - propellant.burnRateExponent)),
					(false, false, true) => _combustionPressure,
					_ => throw new System.Exception("Invalid boolean combination in 'combustionPressure'")
				};
				casing.mawp = _combustionPressure;
				return _combustionPressure;
			}
			set
			{
				_combustionPressure = value;
				casing.mawp = _combustionPressure;
			}
		}

		public float ExpansionRatio => 1f / (Mathf.Pow((propellant.heatCapacityRatio + 1f) / 2f, 1f / (propellant.heatCapacityRatio - 1f))
											* Mathf.Pow(nozzle.designPressurePa / CombustionPressure, 1f / propellant.heatCapacityRatio)
											* Mathf.Sqrt((propellant.heatCapacityRatio + 1f) / (propellant.heatCapacityRatio - 1f)
											* (1f - Mathf.Pow(nozzle.designPressurePa / CombustionPressure, (propellant.heatCapacityRatio - 1f) / propellant.heatCapacityRatio))));

		private readonly float maxThrust = 100_000_000f;
		private float _thrustPercent;
		public float ThrustPercent
		{
			get
			{
				return (thrustIsInput, throatAreaIsInput, thrustPercentIsInput) switch
				{
					(false, false, true) => _thrustPercent,
					(_, _, false) => Thrust / maxThrust * 100f,
					_ => throw new System.Exception("Invalid boolean combination in 'thrustPercent'")
				};
			}
			set
			{
				_thrustPercent = value;
			}
		}
		public float DryMass => casing.Mass;
		public float WetVolume => casing.InnerVolume * grainGeometry.propellantFraction;
		public FloatCurve ThrustCurve => grainGeometry.thrustCurve;
		public float BurnArea => Mathf.PI * Diameter * Length * grainGeometry.burnAreaScale; //0.54f; // TODO: 0.54 is a temp value. Should it be based on thrustCurve?

		#region  Helper Outputs
		public float FuelMass => WetVolume * propellant.density;
		public float Mass => DryMass + FuelMass;
		public float Twr => Thrust / g0 / Mass;
		public float BurnTime => FuelMass / MassFlow;

		#endregion

		#region Limits
		// float maxThrust => g0 * Isp * maxMassFlow;
		// float minThrust => g0 * Isp * minMassFlow;

		// float maxMassFlow => Mathf.Min(maxMassFlowFromPressure, maxMassFlowFromThroat);

		// public float maxMassFlowFromPressure => maxCombustionPressure * throatArea / propellant.characVel;
		// public float minMassFlowFromPressure => minCombustionPressure * throatArea / propellant.characVel;

		// public float maxMassFlowFromThroat => combustionPressure * maxThroatArea / propellant.characVel;
		#endregion
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
		public float Thickness => Mathf.Min(diameter / 2f, (mawp * (diameter - material.corrosionSafety) + 2f * material.tensileStrength * material.weldEff * material.corrosionSafety) / (2f * material.tensileStrength * material.weldEff + mawp));

		public float InnerVolume => PillVolume(cylinderLength - diameter, diameter - 2 * Thickness);
		private float Volume => PillVolume(cylinderLength - diameter, diameter) - InnerVolume;
		public float Mass => material.density * Volume;

		private static float PillVolume(float cylinderLength, float diameter)
		{
			float sphereVol = diameter * diameter * diameter * Mathf.PI / 6f;
			float cylinderVol = diameter * diameter * cylinderLength * Mathf.PI / 4;
			return sphereVol + cylinderVol;
		}
	}
}
