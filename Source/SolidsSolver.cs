using UnityEngine;

namespace ProceduralSolidsLibrary
{
	public class SolidsSolver
	{
		const float g0 = 9.81f;

		public PropellantConfig propellant;
		public Casing casing;
		public Nozzle nozzle;
		public bool thrustIsInput;
		public bool throatAreaIsInput;
		public bool thrustPercentIsInput;

		public SolidsSolver() { casing = new Casing(); }
		public SolidsSolver(PropellantConfig propellant, CasingMaterialConfig casingMaterial, Nozzle nozzle, float length, float diameter)
		{
			this.propellant = propellant;
			this.nozzle = nozzle;
			casing = new Casing(casingMaterial, length - diameter, diameter);
			Length = length;
			Diameter = diameter;
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
				return (thrustIsInput, throatAreaIsInput) switch
				{
					(true, false) => _thrust,
					(false, true) => g0 * Isp * MassFlow,
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
				return (thrustIsInput, throatAreaIsInput) switch
				{
					(true, false) => BurnArea / (Mathf.Pow(CombustionPressure, 1f - propellant.burnRateExponent) / (propellant.density * propellant.burnRateCoeff * propellant.CharacVel)),
					(false, true) => _throatArea,
					_ => throw new System.Exception("Invalid boolean combination in 'throatArea'")
				};
			}
			set
			{
				_throatArea = value;
			}
		}

		public float Isp => nozzle.nozzleCoeff * propellant.CharacVel / g0;
		public float MassFlow =>
			(thrustIsInput, throatAreaIsInput) switch
			{
				(true, false) => _thrust / (g0 * Isp),
				(false, true) => CombustionPressure * ThroatArea / propellant.CharacVel,
				_ => throw new System.Exception("Invalid boolean combination in 'massFlow'")
			};

		private float _combustionPressure;
		public float CombustionPressure
		{
			get
			{
				_combustionPressure = (thrustIsInput, throatAreaIsInput) switch
				{
					(true, false) => Mathf.Pow(MassFlow / (BurnArea * propellant.density * propellant.burnRateCoeff), 1f / propellant.burnRateExponent),
					(false, true) => Mathf.Pow(BurnArea / ThroatArea * propellant.density * propellant.burnRateCoeff * propellant.CharacVel, 1f / (1f - propellant.burnRateExponent)),
					_ => throw new System.Exception("Invalid boolean combination in 'combustionPressure'")
				};
				casing.mawp = _combustionPressure;
				return _combustionPressure;
			}
		}
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
		public float WetVolume => casing.InnerVolume;
		public float BurnArea => Mathf.PI * Diameter * Length * 0.54f; // TODO: 0.54 is a temp value. Should it be based on thrustCurve?

		#region  Helper Outputs
		public float FuelMass => WetVolume * propellant.density;
		public float Mass => DryMass + FuelMass;
		public float Twr => Thrust/g0/Mass;
		public float BurnTime => FuelMass/MassFlow;

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
