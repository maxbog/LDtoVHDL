using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using LDtoVHDL.CompilerPhases;

namespace LDtoVHDL
{
	public class Compiler
	{
		private readonly List<IPhase> m_phases = new List<IPhase>();
		
		public Compiler()
		{
			var types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (var phaseType in types.Where(type => typeof(IPhase).IsAssignableFrom(type) && !type.IsAbstract).Except(Enumerable.Repeat(typeof(IPhase), 1)))
			{
				var constructorInfo = phaseType.GetConstructor(new Type[] { });
				Debug.Assert(constructorInfo != null, "constructorInfo != null");
				var phase = (IPhase)constructorInfo.Invoke(new object[] { });
				m_phases.Add(phase);
			}
		}

		public void Transform(Environment env)
		{
			foreach (var phase in m_phases.OrderBy(phase => phase.Priority))
				phase.Go(env);
		}
	}
}