Shader "Custom/NoteHeadOutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutTex ("Outline Texture", 2D) = "white" {}
        _Invert ("Invert", Range(0,1)) = 0
        _Tint ("Tint", Color) = (1,1,1,1)
		_Outline("Outline", Float) = 0.1
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		_OutlineColor2("Outline Color2", Color) = (1,1,1,1)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
		
		// 외곽선 그리기
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			half _Outline;
			half4 _OutlineColor2;
			sampler2D  _OutTex;
            sampler2D _MainTex;
			struct vertexInput
			{
				float4 vertex: POSITION;
                float2 uv : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos: SV_POSITION;
                float2 uv : TEXCOORD0;
			};

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;
				o.uv = v.uv ; 
				o.pos = UnityObjectToClipPos(v.vertex + fixed4(sign(v.uv.x-0.5)*_Outline*3,sign(v.uv.y-0.5)*_Outline*3,0,0));
				return o;
			}

			half4 frag(vertexOutput i) : COLOR
			{
				fixed4 rgba = fixed4(1,1,1,tex2D(_MainTex,i.uv).a)*tex2D(_OutTex,i.uv);
				return _OutlineColor2*rgba;
			}

			ENDCG
		}
    }
}
