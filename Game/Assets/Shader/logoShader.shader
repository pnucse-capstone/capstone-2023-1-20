Shader "Custom/logoShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseMap("Noise Map", 2D) = "white" {}
        _ColorA("ColorA",Color) = (1,1,1,1)
        _ColorB("ColorB", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
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
            sampler2D _NoiseMap;
            fixed4 _ColorA;
            fixed4 _ColorB;
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 noise = tex2D(_NoiseMap,i.uv);
                fixed4 col = tex2D(_MainTex, i.uv)*lerp(_ColorA,_ColorB,noise);
                return col;
            }
            ENDCG
        }
    }
}
