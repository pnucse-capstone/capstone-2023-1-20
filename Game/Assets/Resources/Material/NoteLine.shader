Shader "Custom/NoteLine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Boundary ("Boundary", Range(0.0,0.5)) = 1 
        _Thick ("Thick", Range(0.0,1)) = 0.5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
            fixed _Boundary;
            fixed _Thick;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed m = 4*_Boundary*_Thick-2*_Boundary-2*_Thick+2;
                fixed2 uv = i.uv;
                uv.y = m*clamp(0.5-abs((uv.y-0.5)),0,0.5);
                fixed4 col = tex2D(_MainTex, uv);
                
                return col;
            }
            ENDCG
        }
    }
}
