using UnityEngine;

namespace ProceduralSolidsLibrary
{
	public class SolidsSolver
	{
		const float g0 = 9.81f;

		public PropellantConfig propellant;
		public Casing casing;
		public Nozzle nozzle;
		private float _length;
		public float length { get => _length; set {_length = value; casing.cylinderLength = _length-_diameter;} }
		private float _diameter;
		public float diameter { get => _diameter; set {_diameter = value; casing.diameter = _diameter;} }

		public SolidsSolver() {casing = new Casing();}
		public SolidsSolver(PropellantConfig propellant, CasingMaterialConfig casingMaterial, Nozzle nozzle, float length, float diameter)
		{
			this.propellant = propellant;
			this.nozzle = nozzle;
			casing = new Casing(casingMaterial, length-diameter, diameter);
			this.length = length;
			this.diameter = diameter;
			// casing.mawp = combustionPressure;
		}

		private float _thrust;
		public float thrust
		{
			get
			{
				return (thrustIsInput, throatAreaIsInput) switch
				{
					(true, false) => _thrust,
					(false, true) => g0 * Isp * massFlow,
					_ => throw new System.Exception("Invalid boolean combination in 'Thrust'")
				};
			}
			set
			{
				_thrust = value;
			}
		}

		private float _throatArea;
		public float throatArea
		{
			get
			{
				return (thrustIsInput, throatAreaIsInput) switch
				{
					(true, false) => burnArea / (Mathf.Pow(combustionPressure, 1f - propellant.burnRateExponent) / (propellant.density * propellant.burnRateCoeff * propellant.characVel)),
					(false, true) => _throatArea,
					_ => throw new System.Exception("Invalid boolean combination in 'throatArea'")
				};
			}
			set
			{
				_throatArea = value;
			}
		}

		public float Isp => nozzle.nozzleCoeff * propellant.characVel / g0;
		public bool thrustIsInput;
		public bool throatAreaIsInput;
		public bool thrustPercentIsInput;
		public float massFlow =>
			(thrustIsInput, throatAreaIsInput) switch
			{
				(true, false) => _thrust / (g0 * Isp),
				(false, true) => combustionPressure * throatArea / propellant.characVel,
				_ => throw new System.Exception("Invalid boolean combination in 'massFlow'")
			};

		private float _combustionPressure;
		public float combustionPressure
		{
			get
			{
				_combustionPressure = (thrustIsInput, throatAreaIsInput) switch
				{
					(true, false) => Mathf.Pow(massFlow/(burnArea * propellant.density * propellant.burnRateCoeff), 1f/propellant.burnRateExponent),
					(false, true) =>Mathf.Pow(burnArea / throatArea * propellant.density * propellant.burnRateCoeff * propellant.characVel, 1f / (1f - propellant.burnRateExponent)),
					_ => throw new System.Exception("Invalid boolean combination in 'combustionPressure'")
				};
				casing.mawp = _combustionPressure;
				return _combustionPressure;
			}
		}
		private float maxThrust = 100_000_000f;
		private float _thrustPercent;
		public float thrustPercent
		{
			get
			{
				return (thrustIsInput, throatAreaIsInput, thrustPercentIsInput) switch
				{
					(false, false, true) => _thrustPercent,
					(_, _, false) => thrust / maxThrust * 100f,
					_ => throw new System.Exception("Invalid boolean combination in 'thrustPercent'")
				};
			}
			set
			{
				_thrustPercent = value;
			}
		}
		public float dryMass => casing.mass;
		public float wetVolume => casing.innerVolume;
		public float burnArea => Mathf.PI * diameter * length * 0.54f; // TODO: 0.54 is a temp value. Should it be based on thrustCurve?

		#region  Helper Outputs
		public float fuelMass => wetVolume * propellant.density;
		public float mass => dryMass + fuelMass;
		public float twr => thrust/g0/mass;
		public float burnTime => fuelMass/massFlow;

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
}
