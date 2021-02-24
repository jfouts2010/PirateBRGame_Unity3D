﻿//Copyright 2018, Davin Carten, All rights reserved

using UnityEngine;
using emotitron.Compression;
using emotitron.Utilities.SmartVars;
using emotitron.Utilities.GUIUtilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace emotitron.NST
{

	[System.Serializable]
	public class ScaleElement : TransformElement, IScaleElement
	{
		
		// Constructor
		public ScaleElement()
		{
			crusher = new ElementCrusher(TRSType.Scale, false) { local = true, uniformAxes = ElementCrusher.UniformAxes.XYZ };
		}

		public override GenericX Localized
		{
			// Can modify this in the future to handle local/global scale if need be.
			get { return new GenericX(gameobject.transform.localScale, (XType)IncludedAxes ); }
			set { Apply(value, gameobject); }
		}

		public override GenericX Extrapolate(GenericX curr, GenericX prev)
		{
			// This should only happen at startup as it is now.
			if (curr.type == XType.NULL)
			{
				return Localized;
			}

			return
				(extrapolation == 0) ? curr :
				curr + (curr - prev) * extrapolation;
		}
	}

#if UNITY_EDITOR

	[CustomPropertyDrawer(typeof(ScaleElement))]
	[CanEditMultipleObjects]

	public class ScaleElementEditor : TransformElementDrawer
	{
		
	}
#endif
}

