Shader "Custom/FadeMaterial"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _t ("t", Range(0,1)) = 0
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
            float _t;
            fixed4 frag (v2f i) : SV_Target
            {
            float cell = 0.005;
                
                // just invert the colors
                fixed2 pivot = fixed2(0.5,0.5);
                fixed2 p = i.uv-pivot;
                fixed rr = (p.y*p.y+p.x*p.x);
                fixed m = (sin(rr/0.05+_t)*_t+10*_t)/2*_t;
                fixed4 col = tex2D(_MainTex, i.uv*m);
                return col*m;
            }
            ENDCG
        }
    }
}
