#if SILVERLIGHT
namespace System
{
	/// <summary>         
	/// Attribute marker to make code compatible with Silverlight         
	/// </summary>  
	/// [AttributeUsage(AttributeTargets.Class,AllowMultiple = false)] 
	/// 
	public sealed class SerializableAttribute : Attribute
	{
	}
}
#endif