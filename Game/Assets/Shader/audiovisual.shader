Shader "Spectrum/Visualizer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            float4x4 _Trans;
            sampler2D _MainTex;
			float _Value[128];
			int _BarAmount = 128;
			float4 _Color;
			float _Flip=1;
			fixed4 frag(v2f i) : SV_Target
			{
//            return col;
            fixed2 dpos = mul(_Trans,(i.uv - (0.5, 0.5)));
            float dist = sqrt(dpos.x * dpos.x + dpos.y * dpos.y);
            dist = dist + 0.05*sin(dpos.y*10+_Time.y) + 0.05 * sin(dpos.x*10+ _Time.y);
            dist = max(dist, 0);
//            int s;
            float val = dist*2/sqrt(2)*127;
            int Q ;
            float R = modf(val, Q);
            fixed4 col = tex2D(_MainTex, i.uv+dpos);
            col=col*lerp(_Value[Q],_Value[Q+1],R);
				return (col.x,col.y,col.z);
            }
            ENDCG
        }
    }
}
