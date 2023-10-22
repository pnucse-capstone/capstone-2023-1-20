// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Gradient/Dither"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

		SubShader
		{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull front
			LOD 100

			Pass
			{
				CGPROGRAM

				#pragma vertex vert alpha
				#pragma fragment frag alpha

				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex  : SV_POSITION;
					half2 uv : TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				v2f vert(appdata_t v)
				{
					v2f o;

					o.vertex = UnityObjectToClipPos(v.vertex);
					v.uv.x = 1 - v.uv.x;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_MainTex, sin(i.uv.x));
				return col2;
				}

				ENDCG
			}
		}
}