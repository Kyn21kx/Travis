// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Tests/Tutorials/UniteShader"{
	
	//Variables
	Properties{
		_MainTexture("Main Texture in shader", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
	}
		//Allows you to use different properties (for different platforms or quality levels)
			SubShader{
			//Passes information to be drawn and equals to one more DrawCall
			Pass {
				CGPROGRAM
				//Define the function with a pragma
				#pragma vertex VertFunc
				#pragma fragment fragFunc
				//Math functions library
				#include "UnityCG.cginc"
				float4 _Color;
				sampler2D _MainTexture;
			//Retrieve the data
			struct appdata {
				//Vector 4 (x,y,z,w)
				float4 vertex : POSITION;
				//Vector 2 (x,y)
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 position : UV_POSITION;
				float2 uv : TEXCOORD0;
			};

			//"v2f" is an abreviation of "Vertex to fragment" and it's used to pass data
			//Build the object
			v2f VertFunc(appdata IN) {
				v2f OUT;
				//MVP is for model view projection
				OUT.position = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;
				return OUT;
			}
			
			fixed4 fragFunc(v2f IN) : SV_Target{

				float4 textColor = tex2D(_MainTexture, IN.uv);


				return textColor * _Color;
			}

			//Vertex
			ENDCG
		}
	}

}