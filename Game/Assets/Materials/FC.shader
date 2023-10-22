Shader "Custom/Ring"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _radius("Radius",Float) = 0.1
        _alpha("Alpha", Float) = 1
        _thick("Thickness", Float) = 1
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
            float _radius;
            float _alpha;
            float _thick;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float r = length(fixed2(0.5, 0.5) - i.uv);
                float l = step(_radius*0.01-0.01* _thick,r)- step(_radius * 0.01 + 0.01* _thick, r);
                // just invert the colors
                return col*l*_alpha;
            }
            ENDCG
        }
    }
}
