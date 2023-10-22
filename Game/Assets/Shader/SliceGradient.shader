Shader "Custom/Gradient slice"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Start("Start",Range(0.0,1.0)) = 0
        _End("End",Range(0.0,1.0)) = 1
        _ColorStart("Color Start",Color) = (1,1,1)
        _ColorEnd("Color End",Color) = (1,1,1)

        _Boundary ("Boundary", Range(0.0,0.5)) = 1 
        _Thick ("Thick", Range(0.01,1)) = 0.5
        _Invert ("Invert", Range(0,1)) = 0
        
        _Length ("Length", float) = 0
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
            float _Start;
            float _End;
            fixed4 _ColorStart;
            fixed4 _ColorEnd;
            fixed _Boundary;
            fixed _Thick;
            fixed _Invert;
            fixed _Length;
            fixed4 frag(v2f i) : SV_Target
            {
                fixed2 uv = i.uv;
                fixed B = _Boundary;
                fixed T = _Thick/2;
                fixed m = B/T;
                uv.y = 
                m*i.uv.y*(step(0,i.uv.y)-step(T,i.uv.y)) // 赣府
                +B*(step(T,i.uv.y)-step(1-T,i.uv.y)) // 个烹
                +((B-m*(T-1+i.uv.y))*(step(1-T,i.uv.y)-step(1,i.uv.y))); // 部府

                uv.x = (uv.x)*_End + (1-uv.x)*_Start;
                fixed4 col = tex2D(_MainTex, uv);
                col =  col*((uv.x)* _ColorEnd + (1 - uv.x) * _ColorStart);

                col.rgb = saturate(1-_Invert)*col.rgb+(saturate(_Invert))*(1-col.rgb);

//                col.rgb *= step(T/2/1.5,i.uv.y);
//                col.rgb *= 1-step(1-T/2/1.5,i.uv.y);
                
                col.rgba -= 0.2*(1-step(T/4*1.5,i.uv.y));
                col.rgba -= 0.2*step(1-T/4*1.5,i.uv.y);

                float cap = 1-B/4*2/_Length*8;
                col.rgb += 0.4*step(cap,i.uv.x)*(0.3/(1-cap)*(i.uv.x-cap));

                col.rgb -= 0.4*step(1-B/2/_Length,i.uv.x);
                return col;
            }
            ENDCG
        }
    }
}
