// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:32716,y:32678,varname:node_4795,prsc:2|normal-649-OUT,alpha-1114-OUT,refract-4138-OUT;n:type:ShaderForge.SFN_Tex2d,id:42,x:31877,y:32485,ptovrint:False,ptlb:Reflection map,ptin:_Reflectionmap,varname:node_42,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:f1ca1ad1f3c1f2e429b8ce2612f1ad9c,ntxv:3,isnm:False|UVIN-1682-UVOUT;n:type:ShaderForge.SFN_Rotator,id:1682,x:31738,y:32446,varname:node_1682,prsc:2|UVIN-6430-UVOUT,SPD-7664-OUT;n:type:ShaderForge.SFN_TexCoord,id:6430,x:31548,y:32361,varname:node_6430,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:7664,x:31391,y:32567,ptovrint:False,ptlb:Rotating Speed,ptin:_RotatingSpeed,varname:node_7664,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:2925,x:31774,y:32694,ptovrint:False,ptlb:Normal Intensity,ptin:_NormalIntensity,varname:node_2925,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0.4273478,max:5;n:type:ShaderForge.SFN_Vector3,id:9123,x:31931,y:32370,varname:node_9123,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Lerp,id:649,x:32076,y:32485,varname:node_649,prsc:2|A-9123-OUT,B-42-RGB,T-2925-OUT;n:type:ShaderForge.SFN_Slider,id:2006,x:31560,y:32930,ptovrint:False,ptlb:Reflection Amount,ptin:_ReflectionAmount,varname:node_2006,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-0.5,cur:0,max:0.5;n:type:ShaderForge.SFN_Multiply,id:8430,x:31827,y:32832,varname:node_8430,prsc:2|A-2925-OUT,B-2006-OUT;n:type:ShaderForge.SFN_ComponentMask,id:4032,x:32151,y:32662,varname:node_4032,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-42-RGB;n:type:ShaderForge.SFN_Multiply,id:3814,x:32203,y:32881,varname:node_3814,prsc:2|A-4032-OUT,B-8430-OUT;n:type:ShaderForge.SFN_Tex2d,id:2347,x:31835,y:33202,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_2347,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0031ea87100f3604c913cebdcbcedd8b,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:1030,x:31457,y:33439,ptovrint:False,ptlb:Opcaity Amount,ptin:_OpcaityAmount,varname:node_1030,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:1114,x:31872,y:33430,varname:node_1114,prsc:2|A-2347-R,B-1030-OUT;n:type:ShaderForge.SFN_ComponentMask,id:4611,x:32006,y:33145,varname:node_4611,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-2347-RGB;n:type:ShaderForge.SFN_Multiply,id:1771,x:32223,y:33090,varname:node_1771,prsc:2|A-3814-OUT,B-4611-OUT;n:type:ShaderForge.SFN_VertexColor,id:1495,x:32191,y:33248,varname:node_1495,prsc:2;n:type:ShaderForge.SFN_Color,id:1832,x:32076,y:33390,ptovrint:False,ptlb:alpha,ptin:_alpha,varname:node_1832,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:4553,x:32292,y:33400,varname:node_4553,prsc:2|A-1495-A,B-1832-A;n:type:ShaderForge.SFN_Multiply,id:4138,x:32465,y:33146,varname:node_4138,prsc:2|A-1771-OUT,B-4553-OUT;proporder:42-7664-2925-2347-1030-2006-1832;pass:END;sub:END;*/

Shader "ParticleEffects/Distortion" {
    Properties {
        _Reflectionmap ("Reflection map", 2D) = "bump" {}
        _RotatingSpeed ("Rotating Speed", Range(-5, 5)) = 0
        _NormalIntensity ("Normal Intensity", Range(-5, 5)) = 0.4273478
        _Opacity ("Opacity", 2D) = "white" {}
        _OpcaityAmount ("Opcaity Amount", Range(0, 1)) = 0
        _ReflectionAmount ("Reflection Amount", Range(-0.5, 0.5)) = 0
        _alpha ("alpha", Color) = (1,1,1,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Reflectionmap; uniform float4 _Reflectionmap_ST;
            uniform float _RotatingSpeed;
            uniform float _NormalIntensity;
            uniform float _ReflectionAmount;
            uniform sampler2D _Opacity; uniform float4 _Opacity_ST;
            uniform float _OpcaityAmount;
            uniform float4 _alpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD5;
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_1835 = _Time;
                float node_1682_ang = node_1835.g;
                float node_1682_spd = _RotatingSpeed;
                float node_1682_cos = cos(node_1682_spd*node_1682_ang);
                float node_1682_sin = sin(node_1682_spd*node_1682_ang);
                float2 node_1682_piv = float2(0.5,0.5);
                float2 node_1682 = (mul(i.uv0-node_1682_piv,float2x2( node_1682_cos, -node_1682_sin, node_1682_sin, node_1682_cos))+node_1682_piv);
                float4 _Reflectionmap_var = tex2D(_Reflectionmap,TRANSFORM_TEX(node_1682, _Reflectionmap));
                float3 normalLocal = lerp(float3(0,0,1),_Reflectionmap_var.rgb,_NormalIntensity);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float4 _Opacity_var = tex2D(_Opacity,TRANSFORM_TEX(i.uv0, _Opacity));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (((_Reflectionmap_var.rgb.rg*(_NormalIntensity*_ReflectionAmount))*_Opacity_var.rgb.r)*(i.vertexColor.a*_alpha.a));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
                float3 finalColor = 0;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,(_Opacity_var.r*_OpcaityAmount)),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
