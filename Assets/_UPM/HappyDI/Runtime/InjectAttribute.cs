using System;

namespace _UPM.HappyDI.Runtime
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class InjectAttribute : Attribute { }
}


