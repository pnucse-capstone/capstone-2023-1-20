Shader "Hidden/EditorSpectrum 1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
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
            float[] _Max;
            float[] _Min;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a = 0.3;
                fixed up = max(_Max[floor(i.uv.x)], 0)+0.5;
                fixed down = min(_Min[floor(i.uv.x)], 0)+0.5;
                col.a = step(up, i.uv.y) - step(down, i.uv.y);
                col.rgb = col.rgb;

            /*
            float up = max(sin(i.u), 0) + 0.5;
            float down = min(sin(i.u), 0) + 0.5;
            col.a = step(up, i.v) - step(down, i.v);
            col.rgb = col.rgb;*/
                return col;
            }
            ENDCG
        }
    }
}
