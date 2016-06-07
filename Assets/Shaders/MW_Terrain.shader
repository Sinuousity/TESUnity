Shader "TES Unity/MW_Terrain"
{
	Properties
	{
		[HideInInspector] _Control("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector] _Normal3("Normal 3 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal2("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal1("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal0("Normal 0 (R)", 2D) = "bump" {}

		[HideInInspector] _MainTex("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector] _Color("Main Color", Color) = (1,1,1,1)
	}

	CGINCLUDE
	#pragma surface surf Lambert vertex:SplatmapVert finalcolor:myfinal exclude_path:prepass exclude_path:deferred
	#pragma multi_compile_fog
	#include "MWTerrainCG.cginc"

	void surf(Input IN, inout SurfaceOutput o)
	{
		half4 splat_control;
		half weight;
		fixed4 mixedDiffuse;
		SplatmapMix(IN, splat_control, weight, mixedDiffuse,o.Normal);
		o.Albedo = mixedDiffuse.rgb;
		o.Alpha = weight;
	}

	ENDCG

	Category
	{
		Tags
		{
			"SplatCount" = "4"
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}
		SubShader
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma multi_compile __ _TERRAIN_NORMAL_MAP
			ENDCG
		}
		SubShader
		{ 
			CGPROGRAM
			ENDCG
		}
	}

	Dependency "AddPassShader" = "TES Unity/MW_Terrain_Add"
	Dependency "BaseMapShader" = "Diffuse"
	Fallback "Diffuse"
}