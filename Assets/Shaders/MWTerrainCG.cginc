struct Input
{
	float2 uv_Splat0 : TEXCOORD0;
	float2 uv_Splat1 : TEXCOORD1;
	float2 uv_Splat2 : TEXCOORD2;
	float2 uv_Splat3 : TEXCOORD3;
	float2 tc_Control : TEXCOORD4;
	float3 worldPos : TEXCOORD5;
	UNITY_FOG_COORDS(5)
};

sampler2D _Control;
float4 _Control_ST;
sampler2D _Splat0, _Splat1, _Splat2, _Splat3;

#ifdef _TERRAIN_NORMAL_MAP
sampler2D _Normal0, _Normal1, _Normal2, _Normal3;
#endif

void SplatmapVert(inout appdata_full v, out Input data)
{
	UNITY_INITIALIZE_OUTPUT(Input, data);
	data.tc_Control = TRANSFORM_TEX(v.texcoord, _Control);	// Need to manually transform uv here, as we choose not to use 'uv' prefix for this texcoord.
	data.worldPos = mul(_Object2World, v.vertex).xyz;
	float4 pos = mul(UNITY_MATRIX_MVP, v.vertex);
	UNITY_TRANSFER_FOG(data, pos);


#ifdef _TERRAIN_NORMAL_MAP
	v.tangent.xyz = cross(v.normal, float3(0, 0, 1));
	v.tangent.w = -1;
#endif
}



half4 dTex2D( sampler2D tex , float2 uv )
{
	return lerp(tex2D(tex, uv*0.1), lerp(tex2D(tex, (uv+0.5)*0.4), tex2D(tex, uv*0.6+0.5), 0.4), 0.6);
}



#ifdef TERRAIN_STANDARD_SHADER
void SplatmapMix(Input IN, half4 defaultAlpha, out half4 splat_control, out half weight, out fixed4 mixedDiffuse, inout fixed3 mixedNormal)
#else
void SplatmapMix(Input IN, out half4 splat_control, out half weight, out fixed4 mixedDiffuse, inout fixed3 mixedNormal)
#endif
{
	splat_control = tex2D(_Control, IN.tc_Control);
	weight = dot(splat_control, half4(1, 1, 1, 1));

#ifndef UNITY_PASS_DEFERRED
	splat_control /= (weight + 1e-3f); // avoid NaNs in splat_control
#endif

#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
	clip(weight - 0.0039 /*1/255*/);
#endif

	mixedDiffuse = 0.0f;
	float2 uv = IN.worldPos.xz;

#ifdef TERRAIN_STANDARD_SHADER
	mixedDiffuse += splat_control.r * dTex2D(_Splat0, uv) * half4(1.0, 1.0, 1.0, defaultAlpha.r);
	mixedDiffuse += splat_control.g * dTex2D(_Splat1, uv) * half4(1.0, 1.0, 1.0, defaultAlpha.g);
	mixedDiffuse += splat_control.b * dTex2D(_Splat2, uv) * half4(1.0, 1.0, 1.0, defaultAlpha.b);
	mixedDiffuse += splat_control.a * dTex2D(_Splat3, uv) * half4(1.0, 1.0, 1.0, defaultAlpha.a);
#else
	mixedDiffuse += splat_control.r * dTex2D(_Splat0, uv);
	mixedDiffuse += splat_control.g * dTex2D(_Splat1, uv);
	mixedDiffuse += splat_control.b * dTex2D(_Splat2, uv);
	mixedDiffuse += splat_control.a * dTex2D(_Splat3, uv);
#endif

#ifdef _TERRAIN_NORMAL_MAP
	fixed4 nrm = 0.0f;
	nrm += splat_control.r * dTex2D(_Normal0, uv);
	nrm += splat_control.g * dTex2D(_Normal1, uv);
	nrm += splat_control.b * dTex2D(_Normal2, uv);
	nrm += splat_control.a * dTex2D(_Normal3, uv);
	mixedNormal = UnpackNormal(nrm);
#endif
}

void SplatmapApplyWeight(inout fixed4 color, fixed weight)
{
	color.rgb *= weight;
	color.a = 1.0f;
}

void SplatmapApplyFog(inout fixed4 color, Input IN)
{
#ifdef TERRAIN_SPLAT_ADDPASS
	UNITY_APPLY_FOG_COLOR(IN.fogCoord, color, fixed4(0, 0, 0, 0));
#else
	UNITY_APPLY_FOG(IN.fogCoord, color);
#endif
}

void myfinal(Input IN, SurfaceOutput o, inout fixed4 color)
{
	SplatmapApplyWeight(color, o.Alpha);
	SplatmapApplyFog(color, IN);
}