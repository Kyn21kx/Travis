Shader "Unlit/Cheese"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Albedo", Color) = (1,1,1,1)
		_DissolveTexture("Cheese", 2D) = "white" {}
		_DissolveMult ("Dissolve multiplier", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _Color;
			sampler2D _DissolveTexture;
			float _DissolveMult;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
				float4 dissolveColor = tex2D(_DissolveTexture, i.uv);
				clip(dissolveColor.rgb - (sin(_DissolveMult + _Time.y) * 0.6));
				//clip(dissolveColor.rgb - _DissolveMult);
				return col * (sin(_Color + _Time.z));
            }
            ENDCG
        }
    }
}
