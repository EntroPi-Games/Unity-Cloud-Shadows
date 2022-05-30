Shader "Hidden/EntroPi/CloudShadow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "" {}
		_LayerTex ("Layer Texture", 2D) = "" {}
		_LayerParams ("Layer Parameters", Vector) = (0,0,0,0)
		_LayerOpacity ("Layer Opacity", Float) = 1
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	// Blend Macros
	#define BlendSubtract(base, blend)		(blend - base)
	#define BlendMultiply(base, blend)		(blend * base)
	#define BlendLightenf(base, blend)		max(blend, base)
	#define BlendDarkenf(base, blend)		min(blend, base)
	#define BlendColorDodgef(base, blend)	((blend == 1.0) ? blend : min(base / (1.0 - blend), 1.0))
	#define BlendColorBurnf(base, blend)	((blend == 0.0) ? blend : max((1.0 - ((1.0 - base) / blend)), 0.0))
	#define BlendVividLightf(base, blend)	((blend < 0.5) ? BlendColorBurnf(base, (2.0 * blend)) : BlendColorDodgef(base, (2.0 * (blend - 0.5))))
	#define BlendPinLightf(base, blend)		((blend < 0.5) ? BlendDarkenf(base, (2.0 * blend)) : BlendLightenf(base, (2.0 *(blend - 0.5))))
	#define BlendOpacity(base, blend, function, opacity)	(function(base, blend) * opacity + blend * (1.0 - opacity))

	sampler2D _MainTex;
	sampler2D _LayerTex;

	uniform float4 _LayerTex_ST;
	uniform float4 _LayerParams;
	uniform float _LayerOpacity;

	struct appdata_t
	{
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct VertexOut
	{
		float4 vertex : SV_POSITION;
		float2 texcoord : TEXCOORD0;
		float2 texcoordClouds : TEXCOORD1;
	};

	VertexOut Vert (appdata_t v)
	{
		VertexOut o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.texcoord = v.texcoord.xy;

		o.texcoordClouds = TRANSFORM_TEX(v.texcoord.xy, _LayerTex);
		o.texcoordClouds -= _LayerParams.xy;

		return o;
	}

	float CalculateLayerAlpha(VertexOut i)
	{
		float coverage = _LayerParams.z;
		float softness = _LayerParams.w;

		// Correct range
		float stepValue = - softness + (1 - coverage) * (1 + (softness * 2));

		float coverageMin = stepValue - softness;
		float coverageMax = stepValue + softness;

		float alpha = tex2D(_LayerTex, i.texcoordClouds).g;
		alpha = smoothstep(coverageMin, coverageMax, alpha);

		return alpha;
	}

	fixed4 FragSubtract (VertexOut i) : SV_Target
	{
		fixed4 color = tex2D(_MainTex, i.texcoord);
		float layerAlpha = CalculateLayerAlpha(i);

		color.rgba = BlendOpacity(layerAlpha, color.a, BlendSubtract, _LayerOpacity);

		return color;
	}

	fixed4 FragMultiply (VertexOut i) : SV_Target
	{
		fixed4 color = tex2D(_MainTex, i.texcoord);
		float layerAlpha = (1 - CalculateLayerAlpha(i));

		color.rgba = BlendOpacity(layerAlpha, color.a, BlendMultiply, _LayerOpacity);

		return color;
	}

	fixed4 FragColorBurn (VertexOut i) : SV_Target
	{
		fixed4 color = tex2D(_MainTex, i.texcoord);
		float layerAlpha = (1 - CalculateLayerAlpha(i));

		color.rgba = BlendOpacity(layerAlpha, color.a, BlendColorBurnf, _LayerOpacity);

		return color;
	}

	fixed4 FragVividLight (VertexOut i) : SV_Target
	{
		fixed4 color = tex2D(_MainTex, i.texcoord);
		float layerAlpha = (1 - CalculateLayerAlpha(i));

		color.rgba = BlendOpacity(layerAlpha, color.a, BlendVividLightf, _LayerOpacity);

		return color;
	}

	fixed4 FragPinLight (VertexOut i) : SV_Target
	{
		fixed4 color = tex2D(_MainTex, i.texcoord);
		float layerAlpha = (1 - CalculateLayerAlpha(i));

		color.rgba = BlendOpacity(layerAlpha, color.a, BlendPinLightf, _LayerOpacity);

		return color;
	}

	ENDCG

	SubShader
	{
		// Pass 0: Subtract
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment FragSubtract
			ENDCG

		}

		// Pass 1: Multiply Inverse
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment FragMultiply
			ENDCG

		}

		// Pass 2: Color Burn
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment FragColorBurn
			ENDCG

		}

		// Pass 3: Vivid Light
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment FragVividLight
			ENDCG

		}

		// Pass 4: Pin Light
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment FragPinLight
			ENDCG

		}
	}

	Fallback Off
}
