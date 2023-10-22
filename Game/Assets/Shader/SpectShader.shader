Shader "Spectrum/SpectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintMap ("Texture", 2D) = "white"{}
        
        _Sigma ("Sigma", Range(0.01,1.0)) = 0.2
	}
    SubShader
    {
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		LOD 100
        ZTest Always
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _TintMap;
			float _Value[256];
            float _Sigma;
			int _BarAmount = 256;
			float4 _Color;
			//float _Flip=1;
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col;
				int t = i.uv.y * 256;
				float barLength = max(_Value[i.uv.y* _BarAmount],0.2);
				//float v = 1-step(barLength,i.uv.x);
				
                //y가 i, x가 f(i)
                float width = (1.0 / _BarAmount);
                col = (step(width/2,i.uv.y%(width)));
                col += _Color;

                fixed4 l = tex2D(_TintMap,float2(_CosTime[0],_SinTime[0]));
                fixed4 r = tex2D(_TintMap,float2(-_SinTime[0],_CosTime[0]));
                col *= r*i.uv.y+l*(1-i.uv.y);
                col.a = barLength;

                float g = exp(-(i.uv.x-0.5)*(i.uv.x-0.5)/_Sigma);
                col.a *= g;
                return col;
            }
            ENDCG
        }
    }
}
