using UnityEngine;
using System;

/// <summary>
/// Adding this attribute restricts the Objects assigned to the field via inspector only to those which implements the InterfaceType
/// </summary>
public class InterfaceTypeAttribute : PropertyAttribute
{
	public Type type;

	public InterfaceTypeAttribute(Type type)
	{
		this.type = type;
	}
}
