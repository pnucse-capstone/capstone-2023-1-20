Shader "Test/bgtest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Noise("Texture",2D) = "white"{}
        _Value("Value", Range(-1,1)) = 0.1
        _Value2("Value2", Range(0,1)) = 0.1
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
            sampler2D _Noise;
            float _Value;
            float _Value2;
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 noise = tex2D(_Noise, fixed2(i.uv.x ,i.uv.y +_Value*_Time.z));
                fixed2 pos = fixed2(i.uv.x + cos(noise.r*3.141592)*_Value2, i.uv.y + sin(noise.r * 3.141592)* _Value2);
                fixed4 col = tex2D(_MainTex, pos);
                return col;
            }
            ENDCG
        }
    }
}
